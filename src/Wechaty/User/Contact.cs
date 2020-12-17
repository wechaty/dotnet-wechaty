using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wechaty.Module.Filebox;
using Wechaty.Module.Puppet.Schemas;

namespace Wechaty.User
{
    public class Contact : Accessory<Contact>, ISayable, ICoversation
    {
        protected ContactPayload? Payload { get; set; }

        public Contact([DisallowNull] string id,
            [DisallowNull] Wechaty wechaty,
            [DisallowNull] ILogger<Contact> logger,
            [AllowNull] string? name = default) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor(${id})");
            }
        }

        public override string ToString()
        {
            if (Payload == null)
            {
                return GetType().Name ?? nameof(Contact);
            }
            var identity = Payload.Alias ?? Payload.Name ?? Id ?? "loading...";
            return $"Contact<{identity}>";
        }


        public async Task<Message?> Say(string text, params Contact[]? replyTo)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({text})");
            }
            var msgId = await Puppet.MessageSendText(Id, text, replyTo?.Select(c => c.Id));
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }
        public async Task<Message?> Say(Message message)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({message})");
            }
            var msgId = await Puppet.MessageForward(Id, message.Id);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }
        public async Task<Message?> Say(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({contact})");
            }
            var msgId = await Puppet.MessageSendContact(Id, contact.Id);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }
        public async Task<Message?> Say(FileBox fileBox)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({fileBox})");
            }
            var msgId = await Puppet.MessageSendFile(Id, fileBox);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }
        public async Task<Message?> Say(UrlLink urlLink)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({urlLink})");
            }
            var msgId = await Puppet.MessageSendUrl(Id, urlLink.Payload);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }
        public async Task<Message?> Say(MiniProgram miniProgram)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({miniProgram})");
            }
            var msgId = await Puppet.MessageSendMiniProgram(Id, miniProgram.Payload);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        public string Name => Payload?.Name ?? "";

        public string? GetAlias()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("GetAlias()");
            }
            if (Payload == null)
            {
                throw new InvalidOperationException("no payload");
            }
            return Payload.Alias;
        }

        public async Task SetAlias(string? newAlias = default)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"SetAlias({newAlias})");
            }
            try
            {
                await Puppet.ContactAlias(Id, newAlias);
                await Puppet.ContactPayloadDirty(Id);
                Payload = await Puppet.ContactPayload(Id);
                if (newAlias != Payload.Alias)
                {
                    Logger.LogWarning($"alias({newAlias}) sync with server fail: set({newAlias}) is not equal to get({Payload.Alias})");
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"alias({newAlias}) rejected");
            }
        }

        /// <summary>
        /// use <see cref="Friend"/> instead.
        /// </summary>
        [Obsolete("use Friend instead")]
        public bool? Stranger
        {
            get
            {
                Logger.LogWarning("stranger() DEPRECATED. use friend() instead.");
                if (Payload == null)
                {
                    return null;
                }
                return !Friend;
            }
        }

        public bool? Friend
        {
            get
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace("friend()");
                }
                return Payload?.Friend;
            }
        }

        /// <summary>
        /// Check if it's a official account, should use <see cref="Type"/> instead
        /// </summary>
        [Obsolete("use Type instead")]
        public bool Official
        {
            get
            {
                Logger.LogWarning("official() DEPRECATED. use type() instead");
                return Payload?.Type == ContactType.Official;
            }
        }

        /// <summary>
        /// Check if it's a personal account, should use <see cref="Type"/> instead
        /// </summary>
        [Obsolete("use Type instead")]
        public bool Personal
        {
            get
            {
                Logger.LogWarning("official() DEPRECATED. use type() instead");
                return Payload?.Type == ContactType.Individual;
            }
        }

        public ContactType Type
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                return Payload.Type;
            }
        }

        public bool? Star => Payload?.Star;

        public ContactGender Gender => Payload?.Gender ?? ContactGender.Unknown;

        public string? Province => Payload?.Province;

        public string? City => Payload?.City;

        public Task<FileBox> Avatar()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace("avatar()");
            }
            try
            {
                return Puppet.ContactAvatar(Id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "avatar() failed.");
                return Task.FromResult(FileBox.FromQRCode("https://u.wechat.com/EJ7pw_ug6XdWRdko3nortP0"));
            }
        }

        public async Task<IReadOnlyList<Tag>> Tags()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"tags() for {this}");
            }
            try
            {
                var tagIdList = await Puppet.TagContactList(Id);
                return tagIdList.Select(id => WechatyInstance.Tag.Load(id))
                    .ToImmutableList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "tags() failed.");
                return Array.Empty<Tag>();
            }
        }

        public Task Sync() => Ready(true);

        public string Id { get; }

        public async Task Ready(bool forceSync = false)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"ready() @ {Puppet} with id=\"{Id}\"");
            }
            if (!forceSync && IsReady)
            {
                Logger.LogTrace("ready() isReady() true");
                return;
            }
            try
            {
                await Puppet.ContactPayloadDirty(Id);
                Payload = await Puppet.ContactPayload(Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ready() failed.");
                throw ex;
            }
        }

        public bool IsReady => !string.IsNullOrWhiteSpace(Payload?.Name);

        public bool Self => Puppet?.SelfId == Id;

        public string? WeiXin => Payload?.Weixin;

        public override Contact ToImplement => this;
    }
}
