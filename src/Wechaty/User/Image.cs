using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wechaty.Module.FileBox;
using Wechaty.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// image
    /// </summary>
    public class Image : Accessory<Image>
    {
        /// <summary>
        /// init <see cref="Image"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public Image([DisallowNull] string id,
                     [DisallowNull] Wechaty wechaty,
                     [DisallowNull] ILogger<Image> logger,
                     [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({id})");
            }
        }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// thumbnail
        /// </summary>
        /// <returns></returns>
        public async Task<FileBox> Thumbnail()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"thumbnail() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.Thumbnail);
        }

        /// <summary>
        /// hd
        /// </summary>
        /// <returns></returns>
        public async Task<FileBox> HD()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"hd() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.HD);
        }

        /// <summary>
        /// artwork
        /// </summary>
        /// <returns></returns>
        public async Task<FileBox> Artwork()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"artwork() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.Artwork);
        }

        ///<inheritdoc/>
        public override Image ToImplement => this;
    }
}
