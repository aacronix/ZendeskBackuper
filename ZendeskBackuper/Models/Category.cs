using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZendeskBackuper.Backuper.Models
{
    class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        public List<Section> sections = new List<Section>();
    }
}
