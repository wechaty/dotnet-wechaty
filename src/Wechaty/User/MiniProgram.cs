using Wechaty.Schemas;

namespace Wechaty.User
{
    public class MiniProgram
    {
        public MiniProgramPayload Payload { get; }

        public MiniProgram(MiniProgramPayload payload) => Payload = payload;

        public string? Appid => Payload.Appid;

        public string? Title => Payload.Title;

        public string? PagePath => Payload.PagePath;

        public string? Username => Payload.Username;

        public string? Description => Payload.Description;

        public string? ThumbUrl => Payload.ThumbUrl;

        public string? ThumbKey => Payload.ThumbKey;
    }
}
