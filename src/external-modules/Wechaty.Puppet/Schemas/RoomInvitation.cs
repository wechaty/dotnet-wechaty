namespace Wechaty.Schemas
{
    public class RoomInvitationPayload
    {
        public string Id { get; set; }
        public string InviterId { get; set; }
        public string Topic { get; set; }
        public string Avatar { get; set; }
        public string Invitation { get; set; }
        public int MemberCount { get; set; }
        public string[] MemberIdList { get; set; }
        public long Timestamp { get; set; }
        public string ReceiverId { get; set; }
    }
}
