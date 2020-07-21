using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class MessageRepository : Accessory<MessageRepository>
    {
        private readonly ILogger<Message> _loggerForMessage;

        private readonly string? _name;

        public MessageRepository([DisallowNull] ILogger<Message> loggerForMessage,
                                 [DisallowNull] Wechaty wechaty,
                                 [DisallowNull] Puppet puppet,
                                 [DisallowNull] ILogger<MessageRepository> logger,
                                 [AllowNull] string? name = null) : base(wechaty, puppet, logger, name)
        {
            _loggerForMessage = loggerForMessage;
            _name = name;
        }

        public Task<Message?> Find([DisallowNull] string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return Find(new MessageQueryFilter { Text = query });
        }

        public async Task<Message?> Find([DisallowNull] MessageQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"find({JsonConvert.SerializeObject(query)})");
            }
            var messageList = await FindAll(query);
            if (messageList.Count > 1)
            {
                Logger.LogWarning($"findAll() got more than one({messageList.Count}) result");
            }
            return messageList.FirstOrDefault();
        }
        public async Task<IReadOnlyList<Message>> FindAll([AllowNull] MessageQueryFilter? query = default)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"findAll({JsonConvert.SerializeObject(query)})");
            }
            var invalid = new Dictionary<string, bool>();
            try
            {
                var messageIdList = await Puppet.MessageSearch(query);
                var messageList = messageIdList.Select(id => Load(id));
                await Task.WhenAll(messageList.Select(async m =>
                {
                    try
                    {
                        await m.Ready;
                    }
                    catch (Exception exception)
                    {
                        Logger.LogWarning(exception, "findAll() message.ready() rejection");
                        invalid[m.Id] = true;
                    }
                }));
                return messageList.Where(m => !invalid.ContainsKey(m.Id))
                    .ToImmutableList();
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, $"findAll() rejected: {exception.Message}");
                return Array.Empty<Message>();
            }
        }

        public Message Load(string id)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"static load({id})");
            }
            return new Message(id, WechatyInstance, Puppet, _loggerForMessage, _name);
        }

        /// <summary>
        /// use <see cref="Load(string)"/> instead.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete]
        public Message Create(string id)
        {
            Logger.LogWarning($"static create({id}) DEPRECATED. Use load() instead");
            return Load(id);
        }

        public override MessageRepository ToImplement() => this;
    }
}
