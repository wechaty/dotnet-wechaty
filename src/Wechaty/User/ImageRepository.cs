using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    /// <summary>
    /// repository of <see cref="Image"/>
    /// </summary>
    public class ImageRepository : Accessory<ImageRepository>
    {
        private readonly ILogger<Image> _loggerForImage;
        private readonly string? _name;

        /// <summary>
        /// init <see cref="ImageRepository"/>
        /// </summary>
        /// <param name="loggerForImage"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public ImageRepository([DisallowNull] ILogger<Image> loggerForImage,
                               [DisallowNull] Wechaty wechaty,
                               [DisallowNull] ILogger<ImageRepository> logger,
                               [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            _loggerForImage = loggerForImage;
            _name = name;
        }

        /// <summary>
        /// create <see cref="Image"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Image Create(string id)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"create({id})");
            }
            return new Image(id, WechatyInstance, _loggerForImage, _name);
        }

        ///<inheritdoc/>
        public override ImageRepository ToImplement => this;
    }
}
