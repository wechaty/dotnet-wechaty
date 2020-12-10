using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Common;
using Wechaty.EventEmitter;
using Wechaty.Filebox;
using Wechaty.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// room
    /// </summary>
    public class Room : Accessory<Room>, ISayable, ICoversation
    {
        /// <summary>
        /// payload
        /// </summary>
        protected RoomPayload? Payload { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// init <see cref="Room"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public Room([DisallowNull] string id,
                    [DisallowNull] Wechaty wechaty,
                    [DisallowNull] ILogger<Room> logger,
                    [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({Id})");
            }
        }

        ///<inheritdoc/>
        public override string ToString() => Payload == null ? nameof(Room) : $"Room<{Payload.Topic ?? "loading..."}>";

        /// <summary>
        /// use <see cref="Sync"/> instead
        /// </summary>
        /// <returns></returns>
        [Obsolete("use Sync() instead")]
        public Task Refresh() => Sync();

        /// <summary>
        /// Force reload data for Room, Sync data from puppet API again.
        /// </summary>
        /// <returns></returns>
        public Task Sync() => Ready(true);

        /// <summary>
        /// `ready()` is For FrameWork ONLY!
        ///
        /// Please not to use `ready()` at the user land.
        /// If you want to sync data, use `sync()` instead.
        /// </summary>
        /// <param name="forceSync"></param>
        /// <returns></returns>
        public async Task Ready(bool forceSync = false)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"ready()");
            }

            if (!forceSync && IsReady)
            {
                return;
            }

            if (forceSync)
            {
                await Puppet.RoomPayloadDirty(Id);
                await Puppet.RoomMemberPayloadDirty(Id);
            }
            Payload = await Puppet.RoomPayload(Id);

            var memberIdList = await Puppet.RoomMemberList(Id);

            await Task.WhenAll(memberIdList.Select(async id =>
            {
                var contact = WechatyInstance.Contact.Load(id);
                try
                {
                    await contact.Ready();
                }
                catch (Exception exception)
                {
                    Logger.LogError($"ready() member.ready() rejection: {exception.Message}");
                }
            }));
        }

        /// <summary>
        /// is ready?
        /// </summary>
        public bool IsReady => Payload != null;

        /// <summary>
        /// try load <see cref="Message"/> by <paramref name="msgId"/>
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        private async Task<Message?> TryLoad(string? msgId)
        {
            if (string.IsNullOrWhiteSpace(msgId))
            {
                return null;
            }
            var msg = WechatyInstance.Message.Load(msgId);
            await msg.Ready;
            return msg;
        }

        /// <summary>
        /// send <paramref name="text"/> to this <see cref="Room"/> and @<paramref name="replyTo"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replyTo"></param>
        /// <returns></returns>
        public async Task<Message?> Say(string text, params Contact[]? replyTo)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({text},{(replyTo != null ? string.Join<Contact>(",", replyTo) : "")})");
            }
            if (replyTo?.Length > 0)
            {
                var memtionAlias = await Task.WhenAll(replyTo.Select(async c => await Alias(c) ?? c.Name));
                text = '@' + string.Join('@', memtionAlias);
            }
            var msgId = await Puppet.MessageSendText(Id, text, replyTo?.Select(c => c.Id));
            return await TryLoad(msgId);
        }

        /// <summary>
        /// send <paramref name="message"/> to this <see cref="Room"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Say(Message message) => message.Forward(this);

        /// <summary>
        /// send <paramref name="file"/> to this <see cref="Room"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<Message?> Say(FileBox file)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({file})");
            }
            var msgId = await Puppet.MessageSendFile(Id, file);
            return await TryLoad(msgId);
        }

        /// <summary>
        /// send <paramref name="url"/> to this <see cref="Room"/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Message?> Say(UrlLink url)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({url})");
            }
            var msgId = await Puppet.MessageSendUrl(Id, url.Payload);
            return await TryLoad(msgId);
        }

        /// <summary>
        /// send <paramref name="mini"/> to this <see cref="Room"/>
        /// </summary>
        /// <param name="mini"></param>
        /// <returns></returns>
        public async Task<Message?> Say(MiniProgram mini)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({mini})");
            }
            var msgId = await Puppet.MessageSendMiniProgram(Id, mini.Payload);
            return await TryLoad(msgId);
        }

        /// <summary>
        /// send <paramref name="contact"/> to this <see cref="Room"/>
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<Message?> Say(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({contact})");
            }
            var msgId = await Puppet.MessageSendContact(Id, contact.Id);
            return await TryLoad(msgId);
        }

        /// <summary>
        /// emit <paramref name="invitation"/> from <paramref name="inviter"/>
        /// </summary>
        /// <param name="inviter"></param>
        /// <param name="invitation"></param>
        /// <returns></returns>
        public virtual bool EmitInvite(Contact inviter, RoomInvitation invitation) => Emit("invite", inviter, invitation);

        /// <summary>
        /// emit remove <paramref name="leaverList"/> from this <see cref="Room"/> at <paramref name="date"/>
        /// </summary>
        /// <param name="leaverList"></param>
        /// <param name="remover"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual bool EmitLeave(IReadOnlyList<Contact> leaverList, Contact remover, DateTime date) => Emit("leave", leaverList, remover, date);

        /// <summary>
        /// emit receive <paramref name="message"/> at this <see cref="Room"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool EmitMessage(Message message) => Emit("message", message);

        /// <summary>
        /// emit <paramref name="inviter"/> invite <paramref name="inviteeList"/> to this <see cref="Room"/> at <paramref name="date"/>
        /// </summary>
        /// <param name="inviteeList"></param>
        /// <param name="inviter"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual bool EmitJoin(IReadOnlyList<Contact> inviteeList, Contact inviter, DateTime date) => Emit("join", inviteeList, inviter, date);

        public virtual bool EmitTopic(string topic, string oldTopic, Contact changer, DateTime date) => Emit("topic", topic, oldTopic, changer, date);

        public virtual Room OnInvite(Action<Room, Contact, RoomInvitation> listener) => this.On("invite", listener);

        public virtual Room OnLeave(Action<Room, IReadOnlyList<Contact>, Contact, DateTime> listener) => this.On("leave", listener);

        public virtual Room OnMessage(Action<Room, Message> listener) => this.On("message", listener);

        public virtual Room OnJoin(Action<Room, IReadOnlyList<Contact>, Contact, DateTime> listener) => this.On("join", listener);

        public virtual Room OnTopic(Action<Room, string, string, Contact, DateTime> listener) => this.On("topic", listener);

        public Task Add(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"add({contact})");
            }
            return Puppet.RoomAdd(Id, contact.Id);
        }

        public Task Delete(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"del({contact})");
            }
            return Puppet.RoomDel(Id, contact.Id);
        }

        public Task Quit()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"quit() {this}");
            }
            return Puppet.RoomQuit(Id);
        }

        public async Task<string> GetTopic()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"get topic()");
            }
            if (!IsReady)
            {
                Logger.LogWarning("topic() room not ready");
                throw new InvalidOperationException("not ready");
            }
            if (Payload != null && !string.IsNullOrEmpty(Payload.Topic))
            {
                return Payload.Topic;
            }
            else
            {
                var memberIdList = await Puppet.RoomMemberList(Id);
                var memberList = memberIdList
                    .Where(id => id != Puppet.SelfId)
                    .Select(id => WechatyInstance.Contact.Load(id));

                return string.Concat(",", memberList.Take(3).Select(m => m.Name));
            }
        }

        public async Task SetTopic(string newTopic)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"get topic({newTopic})");
            }
            if (!IsReady)
            {
                Logger.LogWarning("topic() room not ready");
                throw new InvalidOperationException("not ready");
            }
            try
            {
                await Puppet.RoomTopic(Id, newTopic);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"set topic({newTopic}) exception: {exception.Message}");
            }
        }

        public Task<string> GetAnnounce()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"announce()");
            }
            return Puppet.RoomAnnounce(Id);
        }

        public Task SetAnnounce(string text)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"announce({text})");
            }
            return Puppet.RoomAnnounce(Id, text);
        }

        public async Task<string> QrCode()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"qrCode()");
            }
            return (await Puppet.RoomQRCode(Id)).GuardQrCodeValue();
        }

        public async Task<string?> Alias(Contact contact)
        {
            var memberPayload = await Puppet.RoomMemberPayload(Id, contact.Id);
            return memberPayload?.RoomAlias;
        }

        public async Task<bool> Has(Contact contact)
        {
            var memberIdList = await Puppet.RoomMemberList(Id);
            return (memberIdList?.Any(id => contact.Id == id))
                .GetValueOrDefault(false);
        }

        public Task<IReadOnlyList<Contact>> MemberAll()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"memberAll()");
            }
            return MemberList();
        }

        public async Task<IReadOnlyList<Contact>> MemberAll(string query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"memberAll({query})");
            }
            if (string.IsNullOrEmpty(query))
            {
                return await MemberList();
            }
            var contactIdList = await Puppet.RoomMemberSearch(Id, query);
            return contactIdList.Select(id => WechatyInstance.Contact.Load(id))
                .ToImmutableList();
        }

        public async Task<IReadOnlyList<Contact>> MemberAll(RoomMemberQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"memberAll({JsonConvert.SerializeObject(query)})");
            }
            var contactIdList = await Puppet.RoomMemberSearch(Id, query);
            return contactIdList.Select(id => WechatyInstance.Contact.Load(id))
                .ToImmutableList();
        }

        public async Task<Contact?> Member(RoomMemberQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"member({JsonConvert.SerializeObject(query)})");
            }
            var list = await MemberAll(query);
            return list.FirstOrDefault();
        }

        public async Task<Contact> Member(string query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"member({query})");
            }
            var list = await MemberAll(query);
            return list.FirstOrDefault();
        }

        private async Task<IReadOnlyList<Contact>> MemberList()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"memberList()");
            }
            var memberIdList = await Puppet.RoomMemberList(Id);

            return memberIdList.Select(id => WechatyInstance.Contact.Load(id))
                .ToImmutableList();
        }

        public Contact? Owner
        {
            get
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"owner()");
                }
                var id = Payload?.OwnerId;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return null;
                }
                return WechatyInstance.Contact.Load(id);
            }
        }

        public Task<FileBox> Avatar
        {
            get
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"avatar()");
                }
                return Puppet.RoomAvatar(Id);
            }
        }

        public override Room ToImplement => this;
    }
}
