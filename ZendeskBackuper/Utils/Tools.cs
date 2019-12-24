using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZendeskBackuper.Backuper.Models;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ZendeskBackuper.Backuper.Utils
{
    class Tools
    {
        public static ApplicationConfig AppConfiguration { get; set; }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        static public bool FileIsAsset(string href)
        {
            string extension = Path.GetExtension(href).ToLower();
            return AppConfiguration.ValidAssetsTypesExtensions.Contains(extension);
        }

        public static string PrepareHtml(string htmlString, string baseFolder, ref List<Article> allArticles)
        {
            string articleHrefRegex = string.Format(@"({0})\d*", string.Join("|", AppConfiguration.ArticlesUrlTemplates));

            MatchCollection articlesHrefs = Regex.Matches(htmlString, articleHrefRegex, RegexOptions.IgnoreCase);

            foreach (Match m in articlesHrefs)
            {
                string href = m.Groups[0].Value;
                long articleId = long.Parse(href.Replace(m.Groups[1].Value, ""));

                Article findedArticle = allArticles.Find(article => article.Id == articleId);

                if (findedArticle != null)
                {
                    string url = Path.Combine(new string[] { "..\\..\\..\\..\\", baseFolder, findedArticle.CategoryId.ToString(), findedArticle.SectionId.ToString(), findedArticle.Id.ToString(), "index.html#" });

                    if (!IsAbsoluteUrl(href))
                    {
                        href = AppConfiguration.ZendeskEndpoint + href;
                    }

                    htmlString = htmlString.Replace(href, url);
                }
            }

            return htmlString.Replace("data-gifffer", "src");
        }

        static public List<string> GetImagesFromHtmlString(string htmlString)
        {
            List<string> links = new List<string>();
            string regexImgSrc = "<img.*[src|gifffer]=[\"'](.+?)[\"'].+?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlString, regexImgSrc, RegexOptions.IgnoreCase);
            foreach (Match m in matchesImgSrc)
            {
                string href = m.Groups[1].Value;

                if (FileIsAsset(href))
                {
                    links.Add(href);
                }
            }
            return links;
        }

        static public string ReplaceForeignImagesToLocal(string htmlString, Dictionary<string, string> savedImages)
        {
            foreach (KeyValuePair<string, string> image in savedImages)
            {
                htmlString = htmlString.Replace(image.Key, "assets/" + image.Value);
            }

            return htmlString;
        }

        static public Dictionary<string, string> SaveImagesWithNewNames(List<string> images, string savePath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int index = 0;

            Parallel.ForEach(
                images,
                new ParallelOptions { MaxDegreeOfParallelism = 1 },
                image =>
                {
                    string imageExtension = Path.GetExtension(image.ToString());
                    int hashCode = index;
                    string newFileName = hashCode + imageExtension;
                    string imageSavePath = Path.Combine(savePath, newFileName);

                    if (DownloadFile(image, imageSavePath))
                    {
                        result.TryAdd(image, newFileName);

                        lock (images)
                        {
                            index++;
                        }
                    }
                });

            return result;
        }

        static public bool DownloadFile(string sourceFile, string savePath)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + AppConfiguration.DeveloperEmailBase64 + ":" + AppConfiguration.DeveloperPasswordBase64);

                long attempts = AppConfiguration.MaxDownloadAttempts;

                while (true)
                {
                    try
                    {
                        Thread.Sleep(100);

                        if (!IsAbsoluteUrl(sourceFile))
                        {
                            sourceFile = Tools.AppConfiguration.ZendeskEndpoint + sourceFile;
                        }

                        client.DownloadFile(sourceFile, savePath);

                        return true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Exception in file download: " + sourceFile + ", attempt: " + ((AppConfiguration.MaxDownloadAttempts + 1) - attempts));
                        if (attempts == 0)
                        {
                            return false;
                        }
                        else
                        {
                            Thread.Sleep(2000);

                            attempts--;
                        }
                    }
                }
            }
        }

        static public bool CreateZipArchive(string path, string name)
        {
            ZipFile.CreateFromDirectory(path, name, CompressionLevel.Optimal, false);

            return true;
        }

        static public int RemoveOldFilesFromFolder(string folder, long maxFileLife)
        {
            int deletedCount = 0;

            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);

                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    if (fileInfo.LastWriteTime.AddSeconds(maxFileLife) < DateTime.Now)
                    {
                        fileInfo.Delete();

                        deletedCount++;
                    }
                }
            }

            return deletedCount;
        }

        public static bool WriteToFile(string filePath, string content)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(content);
                }
            }

            return true;
        }
    }
}
