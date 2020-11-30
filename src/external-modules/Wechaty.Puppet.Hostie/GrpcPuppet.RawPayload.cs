using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override async Task<FriendshipPayload> FriendshipRawPayload(string friendshipId)
        {
            var payload = new FriendshipPayload();

            var request = new FriendshipPayloadRequest()
            {
                Id = friendshipId
            };

            var response = await grpcClient.FriendshipPayloadAsync(request);
            if (response != null)
            {
                payload = new FriendshipPayload()
                {
                    ContactId = response.ContactId,
                    Hello = response.Hello,
                    Id = response.Id,
                    Scene = (int)response.Scene,
                    Stranger = response.Stranger,
                    Ticket = response.Ticket,
                    Type = (Schemas.FriendshipType)response.Type
                };
            }
            return payload;
        }

        protected override async Task<FriendshipPayload> FriendshipRawPayloadParser(FriendshipPayload rawPayload)
        {
            return _= rawPayload;
        }

        protected override async Task<MessagePayload> MessageRawPayload(string messageId)
        {
            MessagePayload payload = new MessagePayload();

            var request = new MessagePayloadRequest()
            {
                Id = messageId
            };
            var response = await grpcClient.MessagePayloadAsync(request);

            if (response != null)
            {
                payload = new MessagePayload()
                {
                    Id = messageId,
                    Filename = response.Filename,
                    FromId = response.FromId,
                    Text = response.Text,
                    MentionIdList = response.MentionIds.ToList(),
                    RoomId = response.RoomId,
                    Timestamp = (long)response.Timestamp,
                    Type = (Schemas.MessageType)response.Type,
                    ToId = response.ToId
                };
            }
            return payload;
        }

        protected override MessagePayload MessageRawPayloadParser(MessagePayload rawPayload)
        {
            return rawPayload;
        }

        protected override async Task<RoomInvitationPayload> RoomInvitationRawPayload(string roomInvitationId)
        {
            var payload = new RoomInvitationPayload();
            var request = new RoomInvitationPayloadRequest()
            {
                Id = roomInvitationId
            };
            var response = await grpcClient.RoomInvitationPayloadAsync(request);

            if (response == null)
            {
                payload = new RoomInvitationPayload()
                {
                    Avatar = response.Avatar,
                    Id = response.Id,
                    Invitation = response.Invitation,
                    InviterId = response.InviterId,
                    MemberCount = (int)response.MemberCount,
                    MemberIdList = response.MemberIds.ToList(),
                    ReceiverId = response.ReceiverId,
                    Timestamp = (long)response.Timestamp,
                    Topic = response.Topic
                };
            }
            return payload;
        }

        protected override async Task<RoomInvitationPayload> RoomInvitationRawPayloadParser(RoomInvitationPayload rawPayload)
        {
            return _ = rawPayload;
        }

        protected override async Task<RoomMemberPayload> RoomMemberRawPayload(string roomId, string contactId)
        {
            var payload = new RoomMemberPayload();

            var request = new RoomMemberPayloadRequest()
            {
                Id = roomId,
                MemberId = contactId
            };
            var response = await grpcClient.RoomMemberPayloadAsync(request);
            if (response != null)
            {
                payload = new RoomMemberPayload()
                {
                    Avatar = response.Avatar,
                    Id = response.Id,
                    InviterId = response.InviterId,
                    Name = response.Name,
                    RoomAlias = response.RoomAlias
                };
            }
            return payload;
        }

        protected override async Task<RoomMemberPayload> RoomMemberRawPayloadParser(RoomMemberPayload rawPayload)
        {
            return _= rawPayload;
        }

        protected override async Task<RoomPayload> RoomRawPayload(string roomId)
        {
            var roomPayload = new RoomPayload();

            var request = new RoomPayloadRequest()
            {
                Id = roomId
            };

            var response = await grpcClient.RoomPayloadAsync(request);


            if (response != null)
            {
                roomPayload = new RoomPayload
                {
                    AdminIdList = response.AdminIds.ToList(),
                    Avatar = response.Avatar,
                    Id = response.Id,
                    MemberIdList = response.MemberIds.ToList(),
                    OwnerId = response.OwnerId,
                    Topic = response.Topic
                };
            }
            return roomPayload;
        }

        protected override async Task<RoomPayload> RoomRawPayloadParser(RoomPayload rawPayload)
        {
            return _= rawPayload;
        }


        #endregion
    }
}
