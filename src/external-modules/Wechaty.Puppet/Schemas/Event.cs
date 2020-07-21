using System;
using System.Collections.Generic;

namespace Wechaty.Schemas
{
    public enum ScanStatus
    {
        Unknown = 0,
        Cancel = 1,
        Waiting = 2,
        Scanned = 3,
        Confirmed = 4,
        Timeout = 5,
    }

    public class EventFriendshipPayload
    {
        public string FriendshipId { get; set; }
    }
    public class EventLoginPayload
    {
        public string ContactId { get; set; }
    }
    public class EventLogoutPayload
    {
        public string ContactId { get; set; }
        public string Data { get; set; }
    }
    public class EventMessagePayload
    {
        public string MessageId { get; set; }
    }
    public class EventRoomInvitePayload
    {
        public string RoomInvitationId { get; set; }
    }
    public class EventRoomJoinPayload
    {
        public List<string> InviteeIdList { get; set; }
        public string InviterId { get; set; }
        public string RoomId { get; set; }
        public string Timestamp { get; set; }
    }
    public class EventRoomLeavePayload
    {
        public List<string> RemoverIdList { get; set; }
        public string RemoverId { get; set; }
        public string RoomId { get; set; }
        public string Timestamp { get; set; }
    }

    public class EventRoomTopicPayload
    {
        public string ChangerId { get; set; }
        public string NewTopic { get; set; }
        public string OldTopic { get; set; }
        public string RoomId { get; set; }
        public int Timestamp { get; set; }
    }

    public class EventScanPayload
    {
        public ScanStatus Status { get; set; }
        public string? Qrcode { get; set; }
        public string? Data { get; set; }
    }

    public class EventDongPayload
    {
        public string Data { get; set; }
    }

    public class EventErrorPayload
    {
        public string Data { get; set; }

        public Exception Exception { get; set; }
    }

    public class EventReadyPayload
    {
        public string Data { get; set; }
    }

    public class EventResetPayload
    {
        public string Data { get; set; }
    }

    public class EventHeartbeatPayload
    {
        public string Data { get; set; }
    }
}
