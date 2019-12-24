using System.Net.Http;
using System.Net.Http.Headers;

namespace ZendeskBackuper.Backuper.Utils
{
    class HttpClientInstance
    {
        private HttpClientInstance() { }

        private static HttpClient _instance;

        public static HttpClient GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HttpClient();
                _instance.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            return _instance;
        }
    }
}
