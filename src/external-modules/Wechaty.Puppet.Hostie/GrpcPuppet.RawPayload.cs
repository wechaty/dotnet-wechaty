using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;
using Wechaty.Schemas;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region RawPayload
        protected override async Task<ContactPayload> ContactRawPayload(string contactId)
        {
            var request = new ContactPayloadRequest()
            { Id = contactId };
            var response = await grpcClient.ContactPayloadAsync(request);

            var payload = new ContactPayload()
            {
                Id = response.Id,
                Name = response.Name,
                Address = response.Address,
                Alias = response.Alias,
                Avatar = response.Avatar,
                City = response.City,
                Friend = response.Friend,
                Gender = (Schemas.ContactGender)response.Gender,
                Province = response.Province,
                Signature = response.Signature,
                Star = response.Star,
                Type = (Schemas.ContactType)response.Type,
                Weixin = response.Weixin,
            };
            return payload;
        }

        protected override async Task<ContactPayload> ContactRawPayloadParser(ContactPayload rawPayload)
        {
            return rawPayload;
        }

        protected override Task<FriendshipPayload> FriendshipRawPayload(string friendshipId)
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
