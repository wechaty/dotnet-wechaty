using System.Runtime.Serialization;

namespace Wechaty.Schemas
{
    /// <summary>
    /// snake case
    /// string to enum
    /// </summary>
    public enum ChatEventName
    {
        /// <summary>
        /// receive a friend request
        /// </summary>
        Friendship,
        /// <summary>
        /// puppet had logined
        /// </summary>
        Login,
        /// <summary>
        /// puppet had logouted
        /// </summary>
        Logout,
        /// <summary>
        /// received a new message
        /// </summary>
        Message,
        /// <summary>
        /// received a room invitation
        /// </summary>
        RoomInvite,
        /// <summary>
        /// be added to a room
        /// </summary>
        RoomJoin,
        /// <summary>
        /// leave or be removed from a room
        /// </summary>
        RoomLeave,
        /// <summary>
        /// room topic had been changed
        /// </summary>
        RoomTopic,
        /// <summary>
        /// a QR Code scan is required
        /// </summary>
        Scan
    }

    /// <summary>
    /// snake case
    /// string to enum
    /// </summary>
    public enum PuppetEvent
    {
        /// <summary>
        /// emit this event if you received a ding() call
        /// </summary>
        [EnumMember(Value = "dong")]
        Dong,
        /// <summary>
        /// emit an Error instance when there's any Error need to report to Wechaty
        /// </summary>
        [EnumMember(Value = "error")]
        Error,
        /// <summary>
        /// receive a friend request
        /// </summary>
        [EnumMember(Value = "friendship")]
        Friendship,
        /// <summary>
        /// Huan(202003): rename `watchdog` to `heartbeat`
        /// watchdog  : 'feed the watchdog by emit this event',
        /// feed the watchdog by emit this event
        /// </summary>
        [EnumMember(Value = "heartbeat")]
        Heartbeat,
        /// <summary>
        /// puppet had logined
        /// </summary>
        [EnumMember(Value = "login")]
        Login,
        /// <summary>
        /// puppet had logouted
        /// </summary>
        [EnumMember(Value = "logout")]
        Logout,
        /// <summary>
        /// received a new message
        /// </summary>
        [EnumMember(Value = "message")]
        Message,
        /// <summary>
        /// emit this event after the puppet is ready(you define it)
        /// </summary>
        [EnumMember(Value = "ready")]
        Ready,
        /// <summary>
        /// reset the puppet by emit this event
        /// </summary>
        [EnumMember(Value = "reset")]
        Reset,
        /// <summary>
        /// received a room invitation
        /// </summary>
        [EnumMember(Value = "room-invite")]
        RoomInvite,
        /// <summary>
        /// be added to a room
        /// </summary>
        [EnumMember(Value = "room-join")]
        RoomJoin,
        /// <summary>
        /// leave or be removed from a room
        /// </summary>
        [EnumMember(Value = "room-leave")]
        RoomLeave,
        /// <summary>
        /// room topic had been changed
        /// </summary>
        [EnumMember(Value = "room-topic")]
        RoomTopic,
        /// <summary>
        /// a QR Code scan is required
        /// </summary>
        [EnumMember(Value = "scan")]
        Scan
    }
}
