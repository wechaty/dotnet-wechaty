using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Wechaty.Common;
using Wechaty.Filebox;
using Wechaty.Schemas;

namespace Wechaty.User
{
    /// <summary>
    /// message
    /// </summary>
    public class Message : Accessory<Message>, ISayable
    {
        /// <summary>
        /// @someone seperator character
        /// </summary>
        public const string AT_SEPARATOR = "\u2005|\u0020";

        /// <summary>
        /// mobile: \u2005, PC、mac: \u0020
        /// </summary>
        public static readonly Regex AT_SEPARATOR_REGEX = new Regex(AT_SEPARATOR, RegexOptions.Compiled);

        /// <summary>
        /// load all
        /// </summary>
        private readonly AsyncLazy<object> _loadAll;

        /// <summary>
        /// payload
        /// </summary>
        protected MessagePayload? Payload { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// init <see cref="Message"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        public Message([DisallowNull] string id,
            [DisallowNull] Wechaty wechaty,
            [DisallowNull] ILogger<Message> logger,
            [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({id}) for class {GetType().Name ?? nameof(Message)}");
            }
            _loadAll = new AsyncLazy<object>(async () =>
            {
                Payload = await Puppet.MessagePayload(Id);
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                var roomId = Payload.RoomId;
                var fromId = Payload.FromId;
                var toId = Payload.ToId;
                if (!string.IsNullOrWhiteSpace(roomId))
                {
                    await WechatyInstance.Room.Load(roomId).Ready();
                }
                if (!string.IsNullOrWhiteSpace(fromId))
                {
                    await WechatyInstance.Contact.Load(fromId).Ready();
                }
                if (!string.IsNullOrWhiteSpace(toId))
                {
                    await WechatyInstance.Contact.Load(toId).Ready();
                }
                return typeof(object);
            }, AsyncLazyFlags.ExecuteOnCallingThread | AsyncLazyFlags.RetryOnFailure);
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            if (Payload == null)
            {
                return GetType().Name ?? nameof(Message);
            }
            var stringBuilder = new StringBuilder();

            _ = stringBuilder.Append("Message")
                 .Append($"{Type}");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// talker
        /// </summary>
#pragma warning disable CS8603 // 可能的 null 引用返回。
        public Contact Talker => From;
#pragma warning restore CS8603 // 可能的 null 引用返回。

        /// <summary>
        /// this message belong to which conversation
        /// </summary>
#pragma warning disable CS8603 // 可能的 null 引用返回。
        public ICoversation Coversation => Room ?? From as ICoversation;
#pragma warning restore CS8603 // 可能的 null 引用返回。

        /// <summary>
        /// message sender
        /// </summary>
        public Contact? From
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                var id = Payload.FromId;
                if (id == null)
                {
                    return null;
                }
                return WechatyInstance.Contact.Load(id);
            }
        }

        /// <summary>
        /// message receiver
        /// </summary>
        public Contact? To
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                var id = Payload.ToId;
                if (id == null)
                {
                    return null;
                }
                return WechatyInstance.Contact.Load(id);
            }
        }

        /// <summary>
        /// message from room
        /// </summary>
        public Room? Room
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                var id = Payload.RoomId;
                if (id == null)
                {
                    return null;
                }
                return WechatyInstance.Room.Load(id);
            }
        }

        /// <summary>
        /// use <see cref="Text"/> instead.
        /// </summary>
        [Obsolete("content() DEPRECATED. use text() instead.")]
        public string Content
        {
            get
            {
                Logger.LogWarning("content() DEPRECATED. use text() instead.");
                return Text;
            }
        }

        /// <summary>
        /// text presentation of this message
        /// </summary>
        public string Text
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                return Payload.Text ?? "";
            }
        }

        /// <summary>
        /// get recalled message
        /// </summary>
        /// <returns></returns>
        public async Task<Message?> ToRecalled()
        {
            if (Type != MessageType.Recalled)
            {
                throw new InvalidOperationException("Can not call toRecalled() on message which is not recalled type.");
            }
            var originalMessageId = Text;
            if (string.IsNullOrWhiteSpace(originalMessageId))
            {
                throw new InvalidOperationException("Can not find recalled message");
            }
            try
            {
                var message = WechatyInstance.Message.Load(originalMessageId);
                await message.Ready;
                return message;
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, $"Can not retrieve the recalled message with id ${ originalMessageId}.");
                return null;
            }
        }

        /// <summary>
        /// send <paramref name="text"/> to <see cref="Room"/> or <see cref="From"/>
        /// and @<paramref name="replyTo"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replyTo">@someone</param>
        /// <returns></returns>
        public async Task<Message?> Say(string text, params Contact[]? replyTo)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({text})");
            }
            var room = Room;
            var from = From;
            var conversationId =string.Empty;
            if (room != null && room.Id!=string.Empty)
            {
                conversationId = room.Id;
            }
            else if (from!=null && from.Id!=string.Empty)
            {
                conversationId = from.Id;
            }

            if (conversationId == string.Empty)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageSendText(conversationId, text, replyTo?.Select(c => c.Id));
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// send <paramref name="message"/> to <see cref="Room"/> or <see cref="From"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Message?> Say(Message message)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({message})");
            }
            var room = Room;
            var from = From;
            var conversationId = room?.Id ?? from?.Id;
            if (conversationId == null)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageForward(conversationId, message.Id);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// send <paramref name="contact"/> to <see cref="Room"/> or <see cref="From"/>
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<Message?> Say(Contact contact)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({contact})");
            }
            var room = Room;
            var from = From;
            var conversationId = room?.Id ?? from?.Id;
            if (conversationId == null)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageSendContact(conversationId, contact.Id);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// send <paramref name="fileBox"/> to <see cref="Room"/> or <see cref="From"/>
        /// </summary>
        /// <param name="fileBox"></param>
        /// <returns></returns>
        public async Task<Message?> Say(FileBox fileBox)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({fileBox})");
            }
            var room = Room;
            var from = From;
            var conversationId = room?.Id ?? from?.Id;
            if (conversationId == null)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageSendFile(conversationId, fileBox);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// send <paramref name="urlLink"/> to <see cref="Room"/> or <see cref="From"/>
        /// </summary>
        /// <param name="urlLink"></param>
        /// <returns></returns>
        public async Task<Message?> Say(UrlLink urlLink)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({urlLink})");
            }
            var room = Room;
            var from = From;
            var conversationId = room?.Id ?? from?.Id;
            if (conversationId == null)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageSendUrl(conversationId, urlLink.Payload);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// send <paramref name="miniProgram"/> to <see cref="Room"/> or <see cref="From"/>
        /// </summary>
        /// <param name="miniProgram"></param>
        /// <returns></returns>
        public async Task<Message?> Say(MiniProgram miniProgram)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"say({miniProgram})");
            }
            var room = Room;
            var from = From;
            var conversationId = room?.Id ?? from?.Id;
            if (conversationId == null)
            {
                throw new InvalidOperationException("neither room nor from?");
            }
            var msgId = await Puppet.MessageSendMiniProgram(conversationId, miniProgram.Payload);
            if (msgId != null)
            {
                var result = WechatyInstance.Message.Load(msgId);
                await result.Ready;
                return result;
            }
            return null;
        }

        /// <summary>
        /// message type
        /// </summary>
        public MessageType Type
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

        /// <summary>
        /// check is from self
        /// </summary>
        public bool Self => From?.Id == Puppet.SelfId;

        /// <summary>
        /// Get message mentioned contactList.
        /// Message event table as follows
        /// |                                                                            | Web  |  Mac PC Client | iOS Mobile |  android Mobile |
        /// | :---                                                                       | :--: |     :----:     |   :---:    |     :---:       |
        /// | [You were mentioned] tip ([有人@我]的提示)                                  |  ✘   |        √       |     √      |       √         |
        /// | Identify magic code (8197) by copy and paste in mobile                       |  ✘   |        √       |     √      |       ✘         |
        /// | Identify magic code (8197) by programming                                  |  ✘   |        ✘       |     ✘      |       ✘         |
        /// | Identify two contacts with the same roomAlias by [You were  mentioned] tip |  ✘   |        ✘       |     √      |       √         |
        /// </summary>
        /// <returns>
        /// Return message mentioned contactList
        /// </returns>
        public async Task<IReadOnlyList<Contact>> MentionList()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"mentionList()");
            }
            var room = Room;
            if (Type != MessageType.Text || room == null)
            {
                return Array.Empty<Contact>();
            }
            if (Payload is MessagePayloadRoom messagePayloadRoom)
            {
                return await Task.WhenAll(messagePayloadRoom.MentionIdList.Select(async id =>
                {
                    var contact = WechatyInstance.Contact.Load(id);
                    await contact.Ready();
                    return contact;
                }));
            }
            var atList = AT_SEPARATOR_REGEX.Split(Text);
            if (atList.Length == 0)
            {
                return Array.Empty<Contact>();
            }
            var rawMentionList = atList.Where(at => at.Contains('@'))
                .Select(MultipleAt);
            var mentionNameList = new List<string>(rawMentionList.SelectMany(m => m).Where(t => !string.IsNullOrWhiteSpace(t)));

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"mentionList() text = \"{Text}\", mentionNameList = \"{JsonConvert.SerializeObject(mentionNameList)}\"");
            }

            var contactListNested = await Task.WhenAll(mentionNameList.Select(async name => await room.MemberAll(name)));

            var contactList = contactListNested.SelectMany(n => n).ToList();

            if (contactList.Count == 0)
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"message.mentionList() can not found member using room.member() from mentionList, metion string: ${JsonConvert.SerializeObject(mentionNameList)}");
                }
            }
            return contactList;

            //convert 'hello@a@b@c' to [ 'c', 'b@c', 'a@b@c' ]
            static IReadOnlyList<string> MultipleAt(string at)
            {
                at = Regex.Replace(at, "^.*?@", "@");
                var name = "";
                var nameList = new List<string>();
                foreach (var item in at.Split('@', StringSplitOptions.RemoveEmptyEntries)
                    .Reverse())
                {
                    name = item + "@" + name;
                    nameList.Add(name[0..^2]);
                }
                return nameList;
            }
        }

        /// <summary>
        /// mention() DEPRECATED. use mentionList() instead.
        /// </summary>
        /// <returns></returns>
        [Obsolete("mention() DEPRECATED. use mentionList() instead.")]
        public Task<IReadOnlyList<Contact>> Mention
        {
            get
            {
                Logger.LogWarning("mention() DEPRECATED. use mentionList() instead.");
                return MentionList();
            }
        }

        /// <summary>
        /// get metion text of this message
        /// </summary>
        /// <returns></returns>
        public async Task<string> MentionText()
        {
            var text = Text;
            var room = Room;

            var mentionList = await MentionList();

            if (room == null || mentionList == null || mentionList.Count == 0)
            {
                return text;
            }

            var mentionNameList = await Task.WhenAll(mentionList.Select(m => ToAliasName(room, m)));

            var textWithoutMention = mentionNameList.Aggregate(text, (accumulate, item) =>
            {
                var regex = new Regex($"@${Regex.Escape(item)}({AT_SEPARATOR}|$)");
                return regex.Replace(accumulate, "");
            });

            return textWithoutMention.Trim();

            static async Task<string> ToAliasName(Room room, Contact member)
            {
                var alias = await room.Alias(member);
                var name = member.Name;
                return alias ?? name;
            };
        }

        /// <summary>
        /// mentioned() DEPRECATED. use mentionList() instead.
        /// <see cref="MentionList"/>
        /// </summary>
        [Obsolete("mentioned() DEPRECATED. use mentionList() instead.")]
        public Task<IReadOnlyList<Contact>> Mentioned
        {
            get
            {
                Logger.LogWarning("mentioned() DEPRECATED. use mentionList() instead.");
                return MentionList();
            }
        }

        /// <summary>
        /// Check if a message is mention self.
        /// </summary>
        /// <returns>
        /// Return `true` for mention me.
        /// </returns>
        public async Task<bool> MentionSelf()
        {
            var selfId = Puppet.SelfId;
            var mentionList = await MentionList();
            return mentionList.Any(contact => contact.Id == selfId);
        }

        /// <summary>
        /// check <see cref="Payload"/> is ready
        /// </summary>
        public bool IsReady => Payload != null;

        /// <summary>
        /// ready task
        /// </summary>
        public Task Ready => _loadAll.Task;

        /// <summary>
        /// Forward the received message.
        /// </summary>
        /// <param name="to">
        /// to Room or Contact
        /// The recipient of the message, the room, or the contact
        /// </param>
        /// <returns></returns>
        public async Task Forward(ICoversation to)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"forward({to})");
            }
            try
            {
                _ = await Puppet.MessageForward(to.Id, Id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"forward({to}) failed.");
                throw;
            }
        }

        /// <summary>
        /// Message sent date
        /// </summary>
        public DateTime Date
        {
            get
            {
                if (Payload == null)
                {
                    throw new InvalidOperationException("no payload");
                }
                return Payload.Timestamp.TimestampToDateTime();
            }
        }

        /// <summary>
        /// Returns the message age in seconds.
        /// For example, the message is sent at time `8:43:01`,
        /// and when we received it in Wechaty, the time is `8:43:15`,
        /// then the age() will return `8:43:15 - 8:43:01 = 14 (seconds)`
        /// </summary>
        public long Age
        {
            get
            {
                var timestamp = DateTime.Now - Date;
                return (long)timestamp.TotalSeconds;
            }
        }

        /// <summary>
        /// use <see cref="ToFileBox"/> instead
        /// </summary>
        [Obsolete("file() DEPRECATED. use toFileBox() instead.")]
        public Task<FileBox> File
        {
            get
            {
                Logger.LogWarning("file() DEPRECATED. use toFileBox() instead.");
                return ToFileBox();
            }
        }

        /// <summary>
        /// Extract the Media File from the Message, and put it into the FileBox.
        /// </summary>
        /// <returns></returns>
        public Task<FileBox> ToFileBox()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toFileBox()");
            }
            if (Type == MessageType.Text)
            {
                throw new InvalidOperationException("text message no file");
            }
            return Puppet.MessageFile(Id);
        }

        /// <summary>
        /// Extract the Image File from the Message, so that we can use different image sizes.
        /// </summary>
        /// <returns></returns>
        public Image ToImage()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toImage() for message id: {Id}");
            }
            if (Type != MessageType.Image)
            {
                throw new InvalidOperationException($"not a image type message. type: {Type}");
            }
            return base.WechatyInstance.Image.Create(Id);
        }

        /// <summary>
        /// Get Share Card of the Message
        /// Extract the Contact Card from the Message, and encapsulate it into Contact class
        /// </summary>
        /// <returns></returns>
        public async Task<Contact> ToContact()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toContact()");
            }
            if (Type != MessageType.Contact)
            {
                throw new InvalidOperationException("message not a ShareCard");
            }
            var contactId = await Puppet.MessageContact(Id);
            if (string.IsNullOrWhiteSpace(contactId))
            {
                throw new InvalidOperationException($"can not get Contact id by message: {contactId}");
            }
            var contact = base.WechatyInstance.Contact.Load(contactId);
            await contact.Ready();
            return contact;
        }

        /// <summary>
        /// convert to <see cref="UrlLink"/>
        /// </summary>
        /// <returns></returns>
        public async Task<UrlLink> ToUrlLink()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toUrlLink()");
            }
            if (Payload == null)
            {
                throw new InvalidOperationException("no payload");
            }
            if (Type != MessageType.Url)
            {
                throw new InvalidOperationException("message not a Url Link");
            }
            var urlPayload = await Puppet.MessageUrl(Id);
            if (urlPayload == null)
            {
                throw new InvalidOperationException($"no url payload for message: {Id}");
            }
            return new UrlLink(urlPayload);
        }

        /// <summary>
        /// convert to <see cref="MiniProgram"/>
        /// </summary>
        /// <returns></returns>
        public async Task<MiniProgram> ToMiniProgram()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toMiniProgram()");
            }
            if (Payload == null)
            {
                throw new InvalidOperationException("no payload");
            }
            if (Type != MessageType.MiniProgram)
            {
                throw new InvalidOperationException("message not a MiniProgram");
            }
            var miniProgramPayload = await Puppet.MessageMiniProgram(Id);
            if (miniProgramPayload == null)
            {
                throw new InvalidOperationException($"no miniProgram payload for message: {Id}");
            }
            return new MiniProgram(miniProgramPayload);
        }

        ///<inheritdoc/>
        public override Message ToImplement => this;
    }
}
