using System;
using System.Collections.Generic;
using System.Net.Http;
using ZendeskBackuper.Backuper.Models;
using ZendeskBackuper.Backuper.Utils;
using System.Configuration;

namespace ZendeskBackuper.Backuper.Resources
{
    class SectionsResource
    {
        private static string sectionsEndpoint = $"{Tools.AppConfiguration.ZendeskEndpoint}/api/v2/help_center/{Tools.AppConfiguration.Locale}/sections";
        private static string sectionsByCategoryEndpoint = "{0}/api/v2/help_center/{1}/categories/{2}/sections";
        private static HttpClient client = HttpClientInstance.GetInstance();

        public static List<Section> GetAllSections()
        {
            List<Section> result = new List<Section>();

            string startEndpoint = sectionsEndpoint;

            while (startEndpoint != null)
            {
                SectionsPage sectionPage = GetSectionsPage(new Uri(startEndpoint));

                result.AddRange(sectionPage.Sections);

                startEndpoint = sectionPage.NextPage;
            }

            return result;
        }
        public static SectionsPage GetSectionsPage(Uri endpoint)
        {
            HttpResponseMessage response = client.GetAsync(endpoint).Result;
            SectionsPage sectionsPage = response.Content.ReadAsAsync<SectionsPage>().Result;
            return sectionsPage;
        }

        public static List<Section> GetSectionsByCategories(long categoryId)
        {
            SectionsPage result = new SectionsPage();

            string startEndpoint = String.Format(sectionsByCategoryEndpoint, Tools.AppConfiguration.ZendeskEndpoint, Tools.AppConfiguration.Locale, categoryId);

            HttpResponseMessage response = client.GetAsync(startEndpoint).Result;

            result = response.Content.ReadAsAsync<SectionsPage>().Result;

            return result.Sections;
        }
    }
}
