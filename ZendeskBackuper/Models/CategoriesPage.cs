using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZendeskBackuper.Backuper.Models
{
    [JsonObject]
    class CategoriesPage
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
        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }
    }
}
