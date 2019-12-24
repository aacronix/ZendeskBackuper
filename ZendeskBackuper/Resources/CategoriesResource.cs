using System;
using System.Collections.Generic;
using System.Net.Http;
using ZendeskBackuper.Backuper.Models;
using ZendeskBackuper.Backuper.Utils;
using System.Configuration;

namespace ZendeskBackuper.Backuper.Resources
{
    class CategoriesResource
    {
        private static string categoriesEndpoint = $"{Tools.AppConfiguration.ZendeskEndpoint}/api/v2/help_center/{Tools.AppConfiguration.Locale}/categories";
        private static HttpClient client = HttpClientInstance.GetInstance();

        public static List<Category> GetAllCategories()
        {
            List<Category> result = new List<Category>();

            string startEndpoint = categoriesEndpoint;

            while (startEndpoint != null)
            {
                CategoriesPage categoryPage = GetCategoriesPage(new Uri(startEndpoint));

                result.AddRange(categoryPage.Categories);

                startEndpoint = categoryPage.NextPage;
            }

            return result;
        }
        public static CategoriesPage GetCategoriesPage(Uri endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            CategoriesPage categoriesPage = response.Content.ReadAsAsync<CategoriesPage>().Result;
            return categoriesPage;
        }
    }
}
