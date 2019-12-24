using Newtonsoft.Json;

namespace ZendeskBackuper.Backuper.Models
{
    [JsonObject]
    class Article
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("section_id")]
        public long SectionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        public long CategoryId;
    }
}
