using Newtonsoft.Json;

namespace Wechaty.Schemas
{
    public enum FriendshipType
    {
        Unknown = 0,
        Confirm,
        Receive,
        Verify,
    }
    public enum FriendshipSceneType
    {
        Unknown = 0,   // Huan(202003) added by myself
        QQ = 1,    // FIXME: Huan(202002) in Wechat PC, QQ = 12.
        Email = 2,
        Weixin = 3,
        QQtbd = 12,   // FIXME: confirm the two QQ number QQ号搜索
        Room = 14,
        Phone = 15,
        Card = 17,   // 名片分享
        Location = 18,
        Bottle = 25,
        Shaking = 29,
        QRCode = 30,
    }

    public  class FriendshipPayload
    {
        public string Id { get; set; }
        public string ContactId { get; set; }
        public string? Hello { get; set; }
        /// <summary>
        /// Unix Timestamp, in seconds or milliseconds
        /// </summary>
        public long Timestamp { get; set; }

        public  FriendshipType Type { get; set; }

        public double? Scene { get; set; }
        public string Stranger { get; set; }
        public string Ticket { get; set; }
    }

    public  class FriendshipPayloadConfirm
    {
        [JsonProperty("contactId", Required = Required.Always)]
        public string ContactId { get; set; }

        [JsonProperty("hello", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Hello { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("timestamp", Required = Required.Always)]
        public double Timestamp { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public double Type { get; set; }
    }

    public partial class FriendshipPayloadVerify
    {
        [JsonProperty("contactId", Required = Required.Always)]
        public string ContactId { get; set; }

        [JsonProperty("hello", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Hello { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("timestamp", Required = Required.Always)]
        public double Timestamp { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public double Type { get; set; }
    }

    public class FriendshipSearchCondition
    {
        public string Phone { get; set; }
        public string Weixin { get; set; }
    }
}
