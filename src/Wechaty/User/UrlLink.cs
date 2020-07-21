using Wechaty.Schemas;

namespace Wechaty.User
{
    public class UrlLink
    {
        public UrlLink(UrlLinkPayload payload) => Payload = payload;

        public UrlLinkPayload Payload { get; }

        public override string ToString() => $"UrlLink<${Payload.Url}>";

        public string Title => Payload.Title;

        public string? ThumbnailUrl => Payload.ThumbnailUrl;

        public string? Description => Payload.Description;
    }
}
