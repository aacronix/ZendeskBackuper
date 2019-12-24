using System;
using System.Collections.Generic;
using System.Net.Http;
using ZendeskBackuper.Backuper.Models;
using ZendeskBackuper.Backuper.Utils;
using System.Configuration;

namespace ZendeskBackuper.Backuper.Resources
{
    class ArticlesResource
    {
        private static string articlesEndpoint = $"{Tools.AppConfiguration.ZendeskEndpoint}/api/v2/help_center/{Tools.AppConfiguration.Locale}/articles";
        private static string articlesByCategoryEndpoint = "{0}/api/v2/help_center/{1}/categories/{2}/articles";
        private static string articlesBySectionEndpoint = "{0}/api/v2/help_center/{1}/sections/{2}/articles";
        private static HttpClient client = HttpClientInstance.GetInstance();

        public static List<Article> GetAllArticles()
        {
            List<Article> result = new List<Article>();

            string startEndpoint = articlesEndpoint;

            while (startEndpoint != null)
            {
                ArticlesPage articlePage = GetArticlesPage(new Uri(startEndpoint));

                result.AddRange(articlePage.Articles);

                startEndpoint = articlePage.NextPage;
            }

            return result;
        }

        public static List<Article> GetArticles(Uri endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            ArticlesPage articlePage = response.Content.ReadAsAsync<ArticlesPage>().Result;
            return articlePage.Articles;
        }

        public static ArticlesPage GetArticlesPage(Uri endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            ArticlesPage articlePage = response.Content.ReadAsAsync<ArticlesPage>().Result;
            return articlePage;
        }

        public static List<Article> GetArticlesByCategory(long categoryId)
        {
            List<Article> result = new List<Article>();

            string startEndpoint = string.Format(articlesByCategoryEndpoint, Tools.AppConfiguration.ZendeskEndpoint, Tools.AppConfiguration.Locale, categoryId);

            while (startEndpoint != null)
            {
                ArticlesPage articlePage = GetArticlesPage(new Uri(startEndpoint));

                result.AddRange(articlePage.Articles);

                startEndpoint = articlePage.NextPage;
            }

            return result;
        }

        public static List<Article> GetArticlesBySection(long sectionId)
        {
            List<Article> result = new List<Article>();

            string startEndpoint = string.Format(articlesBySectionEndpoint, Tools.AppConfiguration.ZendeskEndpoint, Tools.AppConfiguration.Locale, sectionId);

            while (startEndpoint != null)
            {
                ArticlesPage articlePage = GetArticlesPage(new Uri(startEndpoint));

                result.AddRange(articlePage.Articles);

                startEndpoint = articlePage.NextPage;
            }

            return result;
        }
    }
}
