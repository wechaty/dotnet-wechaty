using System.Threading.Tasks;

namespace Wechaty
{
    public interface IWechatyDongEventListener
    {
        void Dong(Wechaty wechaty, string data);

        Task DongAsync(Wechaty wechaty, string data);
    }

    public interface IWechatyErrorEventListener
    {
    }
    public interface IWechatyFriendshipEventListener
    {
    }
    public interface IWechatyHeartbeatEventListener
    {
    }
    public interface IWechatyLoginEventListener
    {
    }
    public interface IWechatyLogoutEventListener
    {
    }
    public interface IWechatyMessageEventListener
    {
    }
    public interface IWechatyReadyEventListener
    {
    }
    public interface IWechatyRoomInviteEventListener
    {
    }
    public interface IWechatyRoomJoinEventListener
    {
    }
    public interface IWechatyRoomLeaveEventListener
    {
    }
    public interface IWechatyRoomTopicEventListener
    {
    }
}
