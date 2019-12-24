using System;
using System.Collections.Generic;

namespace ZendeskBackuper.Backuper.Models
{
    public class ApplicationConfig
    {
        public string TempBackupFolderName { get; set; }
        public string BackupsFolder { get; set; }
        public long MaxFileLifeInSeconds { get; set; }
        public string DeveloperEmailBase64 { get; set; }
        public string DeveloperPasswordBase64 { get; set; }
        public string Locale { get; set; }
        public Uri ZendeskEndpoint { get; set; }
        public string AssetsFolderName { get; set; }
        public List<string> ValidAssetsTypesExtensions { get; set; }
        public string IndexFileName { get; set; }
        public long MaxDownloadAttempts { get; set; }
        public int MaxThreads { get; set; }
        public List<string> ArticlesUrlTemplates { get; set; }
    }
}
