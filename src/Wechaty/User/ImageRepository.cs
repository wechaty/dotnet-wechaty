using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    public class ImageRepository : Accessory<ImageRepository>
    {
        private readonly ILogger<Image> _loggerForImage;
        private readonly string? _name;

        public ImageRepository([DisallowNull] ILogger<Image> loggerForImage,
                               [DisallowNull] Wechaty wechaty,
                               [DisallowNull] Puppet puppet,
                               [DisallowNull] ILogger<ImageRepository> logger,
                               [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            _loggerForImage = loggerForImage;
            _name = name;
        }

        public Image Create(string id)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"create({id})");
            }
            return new Image(id, WechatyInstance, Puppet, _loggerForImage, _name);
        }

        public override ImageRepository ToImplement() => this;
    }
}
