using System.Collections.Generic;
using System.IO;
using ZendeskBackuper.Backuper.Utils;
using System.Configuration;

namespace ZendeskBackuper.Backuper.Models
{
    class ArticleTask
    {
        private string savePath;
        private string assetsSavePath;
        private Article article;

        public ArticleTask(Article _article, string _savePath)
        {
            article = _article;
            savePath = Path.Combine(_savePath, article.Id.ToString());
            assetsSavePath = Path.Combine(savePath, Tools.AppConfiguration.AssetsFolderName);
        }

        public bool Run()
        {
            Directory.CreateDirectory(savePath);
            Directory.CreateDirectory(assetsSavePath);

            List<string> imagesUrls = Tools.GetImagesFromHtmlString(article.Body);

            Dictionary<string, string> savedImages = Tools.SaveImagesWithNewNames(imagesUrls, assetsSavePath);

            string localBody = Tools.ReplaceForeignImagesToLocal(article.Body, savedImages);

            string savePathIndexFile = Path.Combine(savePath, Tools.AppConfiguration.IndexFileName);

            Tools.WriteToFile(savePathIndexFile, localBody);

            return true;
        }
    }
}
