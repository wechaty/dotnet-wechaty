namespace Wechaty.Schemas
{
    public class MiniProgramPayload
    {
        /// <summary>
        /// optional, appid, get from wechat (mp.weixin.qq.com)
        /// </summary>
        public string? Appid { get; set; }
        /// <summary>
        /// optional, mini program title
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// optional, mini program page path
        /// </summary>
        public string? PagePath { get; set; }
        /// <summary>
        /// optional, mini program icon url
        /// </summary>
        public string? IconUrl { get; set; }
        /// <summary>
        /// optional, the unique userId for who share this mini program
        /// </summary>
        public string? ShareId { get; set; }
        /// <summary>
        /// optional, default picture, convert to thumbnail
        /// </summary>
        public string? ThumbUrl { get; set; }
        /// <summary>
        /// optional, mini program title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// original ID, get from wechat (mp.weixin.qq.com)
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// original, thumbnailurl and thumbkey will make the headphoto of mini-program better
        /// </summary>
        public string? ThumbKey { get; set; }
    }

}
