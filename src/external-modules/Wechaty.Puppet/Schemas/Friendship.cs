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

    public abstract class FriendshipPayload
    {
        public string Id { get; set; }
        public string ContactId { get; set; }
        public string? Hello { get; set; }
        /// <summary>
        /// Unix Timestamp, in seconds or milliseconds
        /// </summary>
        public long Timestamp { get; set; }

        public abstract FriendshipType Type { get; }
    }
    public class FriendshipPayloadConfirm : FriendshipPayload
    {
        public override FriendshipType Type => FriendshipType.Confirm;
    }
    public class FriendshipPayloadReceive : FriendshipPayload
    {
        public override FriendshipType Type => FriendshipType.Receive;
        public FriendshipSceneType? Scene { get; set; }
        public string? Stranger { get; set; }
        public string Ticket { get; set; }
    }
    public class FriendshipPayloadVerify : FriendshipPayload
    {
        public override FriendshipType Type => FriendshipType.Verify;
    }

    public class FriendshipSearchCondition
    {
        public string Phone { get; set; }
        public string Weixin { get; set; }
    }
}
