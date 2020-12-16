using Newtonsoft.Json;

namespace Wechaty.Schemas
{
    public class UrlLinkPayload
    {

        [JsonProperty("description", Required = Required.Always)]
        public string? Description { get; set; }

        [JsonProperty("thumbnailUrl", Required = Required.Always)]
        public string? ThumbnailUrl { get; set; }

        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("url", Required = Required.Always)]
        public string Url { get; set; }
    }
}
