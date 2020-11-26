using System;
using Wechaty.Schemas;
using Wechaty.User;

namespace Wechaty
{
    public delegate void WechatyDongEventListener(Wechaty wechaty, string? data);
    public delegate void WechatyErrorEventListener(Wechaty wechaty, Exception error);
    public delegate void WechatyFriendshipEventListener(Wechaty wechaty, Friendship friendship);
    public delegate void WechatyHeartbeatEventListener(object data);
    public delegate void WechatyLoginEventListener(Wechaty wechaty, ContactSelf user);
    public delegate void WechatyLogoutEventListener(Wechaty wechaty, ContactSelf user, string? reason);
    public delegate void WechatyMessageEventListener(Message message);
    public delegate void WechatyReadyEventListener(Wechaty wechaty);
    public delegate void WechatyRoomInviteEventListener(Wechaty wechaty, RoomInvitation roomInvitation);
    public delegate void WechatyRoomJoinEventListener(Wechaty wechaty, Room room, Contact[] inviteeList, Contact inviter, DateTime? date);
    public delegate void WechatyRoomLeaveEventListener(Wechaty wechaty, Room room, Contact[] leaverList, Contact remover, DateTime? date);
    public delegate void WechatyRoomTopicEventListener(Wechaty wechaty, Room room, string newTopic, string oldTopic, Contact changer, DateTime? date);
    public delegate void WechatyScanEventListener(Wechaty wechaty, string qrcode, ScanStatus status, string? data);
    public delegate void WechatyStartStopEventListener(Wechaty wechaty);
}
