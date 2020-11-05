using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region Room

        public override Task RoomAdd(string roomId, string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomAnnounce(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task RoomAnnounce(string roomId, string text)
        {
            throw new NotImplementedException();
        }

        public override Task<FileBox> RoomAvatar(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomCreate(IReadOnlyList<string> contactIdList, string? topic)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomCreate(IEnumerable<string> contactIdList, string? topic)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomCreate(string[] contactIdList, string? topic)
        {
            throw new NotImplementedException();
        }

        public override Task RoomDel(string roomId, string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task RoomInvitationAccept(string roomInvitationId)
        {
            throw new NotImplementedException();
        }

        public override Task<IReadOnlyList<string>> RoomList()
        {
            throw new NotImplementedException();
        }

        public override Task<string[]> RoomMemberList(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomQRCode(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task RoomQuit(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task<string> RoomTopic(string roomId)
        {
            throw new NotImplementedException();
        }

        public override Task RoomTopic(string roomId, string topic)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
