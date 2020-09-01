using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    public class TagRepository : Accessory<TagRepository>
    {
        private readonly ILogger<Tag> _loggerForTag;
        private readonly string? _name;
        private readonly ConcurrentDictionary<string, Tag> _pool = new ConcurrentDictionary<string, Tag>();

        public TagRepository([DisallowNull] ILogger<Tag> loggerForTag,
                             [DisallowNull] Wechaty wechaty,
                             [DisallowNull] ILogger<TagRepository> logger,
                             [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            _loggerForTag = loggerForTag;
            _name = name;
        }

        public Tag Load(string id) => _pool.GetOrAdd(id, key => new Tag(id, WechatyInstance, _loggerForTag, _name));

        public async Task<Tag> Get(string tag) => Load(tag);

        public async Task Delete(Tag tag)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"delete({tag})");
            }
            try
            {
                await Puppet.TagContactDelete(tag.Id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"delete() exception: {exception.Message}");
            }
        }

        public override TagRepository ToImplement => this;
    }
}
