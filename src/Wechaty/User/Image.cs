using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class Image : Accessory<Image>
    {
        public Image([DisallowNull] string id,
                     [DisallowNull] Wechaty wechaty,
                     [DisallowNull] Puppet puppet,
                     [DisallowNull] ILogger<Image> logger,
                     [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({id})");
            }
            if (puppet == null)
            {
                throw new ArgumentNullException(nameof(puppet), "Image class can not be instanciated without a puppet!");
            }
        }

        public string Id { get; }

        public async Task<FileBox> Thumbnail()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"thumbnail() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.Thumbnail);
        }

        public async Task<FileBox> HD()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"hd() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.HD);
        }

        public async Task<FileBox> Artwork()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"artwork() for id: \"{Id}\"");
            }
            return await Puppet.MessageImage(Id, ImageType.Artwork);
        }

        public override Image ToImplement() => this;
    }
}
