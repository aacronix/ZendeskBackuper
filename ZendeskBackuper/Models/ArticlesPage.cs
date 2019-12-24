using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZendeskBackuper.Backuper.Models
{
    [JsonObject]
    class ArticlesPage
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("next_page")]
        public string NextPage { get; set; }
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("page_count")]
        public int PageCount { get; set; }
        [JsonProperty("per_page")]
        public int PerPage { get; set; }
        [JsonProperty("previous_page")]
        public object PreviousPage { get; set; }
        [JsonProperty("articles")]
        public List<Article> Articles { get; set; }
    }
}
