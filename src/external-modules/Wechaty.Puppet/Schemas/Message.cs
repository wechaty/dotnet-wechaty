using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Wechaty.Schemas
{
    public enum MessageType
    {
        Unknown = 0,

        Attachment,     // Attach(6),
        Audio,          // Audio(1), Voice(34)
        Contact,        // ShareCard(42)
        ChatHistory,    // ChatHistory(19)
        Emoticon,       // Sticker: Emoticon(15), Emoticon(47)
        Image,          // Img(2), Image(3)
        Text,           // Text(1)
        Location,       // Location(48)
        MiniProgram,    // MiniProgram(33)
        GroupNote,      // GroupNote(53)
        Transfer,       // Transfers(2000)
        RedEnvelope,    // RedEnvelopes(2001)
        Recalled,       // Recalled(10002)
        Url,            // Url(5)
        Video,          // Video(4), Video(43)
    }
    public enum WechatAppMessageType
    {
        Text = 1,
        Img = 2,
        Audio = 3,
        Video = 4,
        Url = 5,
        Attach = 6,
        Open = 7,
        Emoji = 8,
        VoiceRemind = 9,
        ScanGood = 10,
        Good = 13,
        Emotion = 15,
        CardTicket = 16,
        RealtimeShareLocation = 17,
        ChatHistory = 19,
        MiniProgram = 33,
        Transfers = 2000,
        RedEnvelopes = 2001,
        ReaderType = 100001,
    }
    public enum WechatMessageType
    {
        Text = 1,
        Image = 3,
        Voice = 34,
        VerifyMsg = 37,
        PossibleFriendMsg = 40,
        ShareCard = 42,
        Video = 43,
        Emoticon = 47,
        Location = 48,
        App = 49,
        VoipMsg = 50,
        StatusNotify = 51,
        VoipNotify = 52,
        VoipInvite = 53,
        MicroVideo = 62,
        /// <summary>
        /// 转账
        /// </summary>
        Transfer = 2000,
        /// <summary>
        /// 红包
        /// </summary>
        RedEnvelope = 2001,
        /// <summary>
        /// 小程序
        /// </summary>
        MiniProgram = 2002,
        /// <summary>
        /// 群邀请
        /// </summary>
        GroupInvite = 2003,
        /// <summary>
        /// 文件消息
        /// </summary>
        File = 2004,
        SysNotice = 9999,
        Sys = 10000,
        /// <summary>
        /// NOTIFY 服务通知
        /// </summary>
        Recalled = 10002,
    }
    public class MessagePayloadRoom : MessagePayload
    {
        public IReadOnlyList<string>? MentionIdList { get; set; }
    }

    public  class MessagePayload
    {
        /// <summary>
        /// use message id to get rawPayload to get those information when needed
        /// contactId?    : string,        // Contact ShareCard
        /// </summary>
        public string Id { get; set; }
        public string? Filename { get; set; }
        public string? Text { get; set; }

        public List<string> MentionIdList { get; set; }
        /// <summary>
        /// Huan(202001): we support both seconds and milliseconds in Wechaty now.
        /// </summary>
        public long Timestamp { get; set; }
        public MessageType Type { get; set; }
        public string? FromId { get; set; }
        public string? RoomId { get; set; }
        public string? ToId { get; set; }
    }
    public class MessageQueryFilter : IFilter
    {
        StringOrRegex? IFilter.this[string key]
        {
            get
            {
                switch (key)
                {
                    case nameof(FromId):
                        return FromId;
                    case nameof(Id):
                        return Id;
                    case nameof(RoomId):
                        return RoomId;
                    case nameof(Text):
                        return Text;
                    case nameof(ToId):
                        return ToId;
                    case nameof(Type):
                        return Type.ToString();
                    default:
                        throw new MissingMemberException(GetType().FullName, key);
                }
            }
        }

        private readonly ImmutableList<string> _keys = new List<string>
        {
            nameof(FromId),
            nameof(Id),
            nameof(RoomId),
            nameof(Text),
            nameof(ToId),
            nameof(Type)
        }.ToImmutableList();

        IReadOnlyList<string> IFilter.Keys => _keys;

        public string? FromId { get; set; }
        public string? Id { get; set; }
        public string? RoomId { get; set; }
        public StringOrRegex? Text { get; set; }
        public string? ToId { get; set; }
        public MessageType? Type { get; set; }
    }

    public delegate bool MessagePayloadFilterFunction(MessagePayload payload);

    public delegate MessagePayloadFilterFunction MessagePayloadFilterFactory(MessageQueryFilter query);

    public static class MessagePayloadFilterFunctionExtensions
    {
        public static MessagePayloadFilterFunction Every(this IEnumerable<MessagePayloadFilterFunction> functions) => payload => !functions.Any(f => !f(payload));
    }
}
