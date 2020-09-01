using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenGraphNet;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class UrlLinkRepository : Accessory<UrlLinkRepository>
    {
        public UrlLinkRepository([DisallowNull] Wechaty wechaty,
                                 [DisallowNull] ILogger<UrlLinkRepository> logger,
                                 [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
        }

        public async Task<UrlLink> Create(string url)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"create({url})");
            }
            var openGraph = await OpenGraph.ParseUrlAsync(url);

            return new UrlLink(new UrlLinkPayload
            {
                Description = openGraph.Metadata["description"]?.FirstOrDefault()?.Value,
                ThumbnailUrl = openGraph.Image?.AbsoluteUri,
                Title = openGraph.Title,
                Url = openGraph.Url.AbsoluteUri
            });
        }

        public override UrlLinkRepository ToImplement => this;
    }
}
