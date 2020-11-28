using System;
using Wechaty.Schemas;
using Wechaty.User;

namespace Wechaty
{
    public delegate void WechatyDongEventListener(string? data);
    public delegate void WechatyErrorEventListener(Exception error);
    public delegate void WechatyFriendshipEventListener(Friendship friendship);
    public delegate void WechatyHeartbeatEventListener(object data);
    public delegate void WechatyLoginEventListener(ContactSelf user);
    public delegate void WechatyLogoutEventListener(ContactSelf user, string? reason);
    public delegate void WechatyMessageEventListener(Message message);
    public delegate void WechatyReadyEventListener(Wechaty wechaty);
    public delegate void WechatyRoomInviteEventListener(RoomInvitation roomInvitation);
    public delegate void WechatyRoomJoinEventListener(Room room, Contact[] inviteeList, Contact inviter, DateTime? date);
    public delegate void WechatyRoomLeaveEventListener(Room room, Contact[] leaverList, Contact remover, DateTime? date);
    public delegate void WechatyRoomTopicEventListener(Room room, string newTopic, string oldTopic, Contact changer, DateTime? date);
    public delegate void WechatyScanEventListener(string qrcode, ScanStatus status, string? data);
    public delegate void WechatyStartStopEventListener(Wechaty wechaty);
}
