using Wechaty.Module.Puppet.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// mini program
    /// </summary>
    public class MiniProgram
    {
        /// <summary>
        /// payload
        /// </summary>
        public MiniProgramPayload Payload { get; }

        /// <summary>
        /// init <see cref="MiniProgram"/> with payload
        /// </summary>
        /// <param name="payload"></param>
        public MiniProgram(MiniProgramPayload payload) => Payload = payload;

        /// <summary>
        /// appid
        /// </summary>
        public string? Appid => Payload.Appid;

        /// <summary>
        /// title
        /// </summary>
        public string? Title => Payload.Title;

        /// <summary>
        /// page path
        /// </summary>
        public string? PagePath => Payload.PagePath;

        /// <summary>
        /// username
        /// </summary>
        public string? Username => Payload.Username;

        /// <summary>
        /// description
        /// </summary>
        public string? Description => Payload.Description;

        /// <summary>
        /// tumbnail url
        /// </summary>
        public string? ThumbUrl => Payload.ThumbUrl;

        /// <summary>
        /// thumbnal key
        /// </summary>
        public string? ThumbKey => Payload.ThumbKey;
    }
}
