using System;
using System.Collections.Generic;
using ZendeskBackuper.Backuper.Models;
using ZendeskBackuper.Backuper.Utils;
using ZendeskBackuper.Backuper.Resources;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ZendeskBackuper.Backuper
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        public static void Run()
        {
            Tools.AppConfiguration = GetConfiguration();

            CreateBackup();
        }

        public static ApplicationConfig GetConfiguration()
        {
            ApplicationConfig applicationConfig = new ApplicationConfig();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            configuration.Bind(applicationConfig);

            return applicationConfig;
        }

        static public void CreateBackup()
        {
            long runVersion = DateTime.Now.Ticks;
            string initDirectoryFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Tools.AppConfiguration.TempBackupFolderName + runVersion);
            string backupsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Tools.AppConfiguration.BackupsFolder);
#if DEBUG
            int startCount = 0;
#endif
            if (!Directory.Exists(backupsFolder))
            {
                Directory.CreateDirectory(backupsFolder);
            }

            if (Directory.Exists(initDirectoryFolder))
            {
                Directory.Delete(initDirectoryFolder, true);
            }
            Directory.CreateDirectory(initDirectoryFolder);

            Console.WriteLine("Run version: " + runVersion);
            
            List<Category> categories = CategoriesResource.GetAllCategories();
            List<Section> sections = SectionsResource.GetAllSections();
            List<Article> articles = ArticlesResource.GetAllArticles();

            List<Category> structure = new List<Category>();
            List<ArticleTask> articleTasks = new List<ArticleTask>();

            if (categories.Count > 0 && sections.Count > 0 && articles.Count > 0)
            {
                foreach (Category category in categories)
                {
                    Category structureCategory = category;

                    List<Section> findedSections = sections.Where(section => section.CategoryId == category.Id).ToList();

                    List<Section> structureSections = new List<Section>();

                    foreach (Section section in findedSections)
                    {
                        Section structureSection = section;

                        List<Article> findedArticles = articles.Where(article => article.SectionId == section.Id).ToList();

                        foreach (Article article in findedArticles)
                        {
                            article.CategoryId = category.Id;
                            structureSection.articles.Add(article);
                        }

                        structureCategory.sections.Add(structureSection);
                    }

                    structure.Add(structureCategory);
                }
            }
            else
            {
                Console.WriteLine("Resource not found or not available");
            }

            List<Article> allArticles = structure.SelectMany(category => category.sections).SelectMany(section => section.articles).ToList();

            string categoriesIndexFile = HtmlGenerator.GetHtmlForCategoriesIndex(structure);
            string categoriesIndexFilePath = Path.Combine(initDirectoryFolder, Tools.AppConfiguration.IndexFileName);

            Tools.WriteToFile(categoriesIndexFilePath, categoriesIndexFile);

            foreach (Category category in structure)
            {
                string sectionsIndexFile = HtmlGenerator.GetHtmlForSectionsIndex(category.sections);
                string sectionsIndexSavePath = Path.Combine(initDirectoryFolder, category.Id.ToString());
                Directory.CreateDirectory(sectionsIndexSavePath);
                string sectionsIndexFilePath = Path.Combine(sectionsIndexSavePath, Tools.AppConfiguration.IndexFileName);

                Tools.WriteToFile(sectionsIndexFilePath, sectionsIndexFile);

                foreach (Section section in category.sections)
                {
                    string articlesIndexFile = HtmlGenerator.GetHtmlForArticlesIndex(section.articles);
                    string articlesIndexSavePath = Path.Combine(sectionsIndexSavePath, section.Id.ToString());
                    Directory.CreateDirectory(articlesIndexSavePath);
                    string articlesIndexFilePath = Path.Combine(articlesIndexSavePath, Tools.AppConfiguration.IndexFileName);

                    Tools.WriteToFile(articlesIndexFilePath, articlesIndexFile);

                    foreach (Article article in section.articles)
                    {
                        article.Body = Tools.PrepareHtml(article.Body, Tools.AppConfiguration.TempBackupFolderName + runVersion, ref allArticles);

                        articleTasks.Add(new ArticleTask(article, articlesIndexSavePath));
                    }
                }
            }

            Stopwatch stopWatch = Stopwatch.StartNew();

            Parallel.ForEach(
                articleTasks,
                new ParallelOptions { MaxDegreeOfParallelism = Tools.AppConfiguration.MaxThreads },
                articleTask =>
                {
                    articleTask.Run();
#if DEBUG
                    lock (articleTasks)
                    {
                        startCount++;

                        Console.WriteLine("started tasks: " + startCount);
                    }
#endif
                });

            stopWatch.Stop();
            Console.WriteLine("Seconds elapsed: " + stopWatch.ElapsedMilliseconds / 1000);

            int removedFoldersCount = Tools.RemoveOldFilesFromFolder(Tools.AppConfiguration.BackupsFolder, Tools.AppConfiguration.MaxFileLifeInSeconds);

            Console.WriteLine("Count of removed backups older than 30 days: " + removedFoldersCount);

            string zipBackupFileName = "backup" + runVersion + ".zip";
            string zipBackupFilePath = Path.Combine(backupsFolder, zipBackupFileName);

            if (Tools.CreateZipArchive(initDirectoryFolder, zipBackupFilePath))
            {
                Directory.Delete(initDirectoryFolder, true);
            }
        }
    }
}
