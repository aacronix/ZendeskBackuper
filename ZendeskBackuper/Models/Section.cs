using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ZendeskBackuper.Backuper.Models
{
    class Section
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parent_section_id")]
        public object ParentSectionId { get; set; }

        public List<Article> articles = new List<Article>();
    }
}
