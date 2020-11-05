using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wechaty.Schemas;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region RawPayload
        protected override Task<object> ContactRawPayload(string contactId)
        {
            throw new NotImplementedException();
        }

        protected override Task<ContactPayload> ContactRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }

        protected override Task<object> FriendshipRawPayload(string friendshipId)
        {
            throw new NotImplementedException();
        }

        protected override Task<FriendshipPayload> FriendshipRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }

        protected override Task<object> MessageRawPayload(string messageId)
        {
            throw new NotImplementedException();
        }

        protected override Task<MessagePayload> MessageRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }

        protected override Task<object> RoomInvitationRawPayload(string roomInvitationId)
        {
            throw new NotImplementedException();
        }

        protected override Task<RoomInvitationPayload> RoomInvitationRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }

        protected override Task<object> RoomMemberRawPayload(string roomId, string contactId)
        {
            throw new NotImplementedException();
        }

        protected override Task<RoomMemberPayload> RoomMemberRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }

        protected override Task<object> RoomRawPayload(string roomId)
        {
            throw new NotImplementedException();
        }

        protected override Task<RoomPayload> RoomRawPayloadParser(object rawPayload)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
