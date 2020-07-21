using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wechaty.User
{
    public class Tag : Accessory<Tag>
    {
        public string Id { get; }
        public Tag([DisallowNull] string id,
                   [DisallowNull] Wechaty wechaty,
                   [DisallowNull] Puppet puppet,
                   [DisallowNull] ILogger<Tag> logger,
                   [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({id})");
            }
            if (puppet == null)
            {
                throw new ArgumentNullException(nameof(puppet), "Tag class can not be instanciated without a puppet!");
            }
        }

        public async Task Add(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"add({contact}) for {Id}");
            }
            try
            {
                await Puppet.TagContactAdd(Id, contact.Id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"add() exception: {exception.Message}");
                throw new InvalidOperationException("add error", exception);
            }
        }

        public async Task Add(Favorite favorite)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"add({favorite}) for {Id}");
            }
            try
            {
                // Just like TypeScript implementation
                //TODO: await this.puppet.tagAddFavorite(this.tag, to.id)
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"add() exception: {exception.Message}");
                throw new InvalidOperationException("add error", exception);
            }
        }

        public async Task Remove(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"remove({contact}) for {Id}");
            }
            try
            {
                await Puppet.TagContactRemove(Id, contact.Id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"remove() exception: {exception.Message}");
                throw new InvalidOperationException("add error", exception);
            }
        }
        public async Task Remove(Favorite favorite)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"remove({favorite}) for {Id}");
            }
            try
            {
                // Just like TypeScript implementation
                //TODO: await this.puppet.tagRemoveFavorite(this.tag, from.id)
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"remove() exception: {exception.Message}");
                throw new InvalidOperationException("add error", exception);
            }
        }

        public override Tag ToImplement() => this;
    }
}
