using Newtonsoft.Json;

namespace Wechaty.Schemas
{
    public class MiniProgramPayload
    {
        /// <summary>
        /// optional, appid, get from wechat (mp.weixin.qq.com)
        /// </summary>
        [JsonProperty("appid", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? Appid { get; set; }
        /// <summary>
        /// optional, mini program title
        /// </summary>
        [JsonProperty("description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }
        /// <summary>
        /// optional, mini program page path
        /// </summary>
        [JsonProperty("pagePath", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? PagePath { get; set; }
        /// <summary>
        /// optional, mini program icon url
        /// </summary>
        [JsonProperty("iconUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? IconUrl { get; set; }
        /// <summary>
        /// optional, the unique userId for who share this mini program
        /// </summary>
        [JsonProperty("shareId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? ShareId { get; set; }
        /// <summary>
        /// optional, default picture, convert to thumbnail
        /// </summary>
        [JsonProperty("thumbUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? ThumbUrl { get; set; }
        /// <summary>
        /// optional, mini program title
        /// </summary>
        [JsonProperty("title", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }
        /// <summary>
        /// original ID, get from wechat (mp.weixin.qq.com)
        /// </summary>
        [JsonProperty("username", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? Username { get; set; }
        /// <summary>
        /// original, thumbnailurl and thumbkey will make the headphoto of mini-program better
        /// </summary>
        [JsonProperty("thumbKey", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string? ThumbKey { get; set; }
    }

}
