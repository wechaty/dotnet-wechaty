using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;
using Newtonsoft.Json;

namespace Wechaty
{
    public partial class GrpcPuppet
    {
        #region Room

        public override async Task RoomAdd(string roomId, string contactId)
        {
            var request = new RoomAddRequest()
            {
                ContactId = contactId,
                Id = roomId
            };
            await grpcClient.RoomAddAsync(request);
        }

        public override async Task<string> RoomAnnounce(string roomId)
        {
            var request = new RoomAnnounceRequest
            {
                Id = roomId
            };


            var response = await grpcClient.RoomAnnounceAsync(request);
            return response?.Text;
        }

        public override async Task RoomAnnounce(string roomId, string text)
        {
            var request = new RoomAnnounceRequest
            {
                Id = roomId,
                Text = text
            };

            await grpcClient.RoomAnnounceAsync(request);

        }

        public override async Task<FileBox.FileBox> RoomAvatar(string roomId)
        {
            var request = new RoomAvatarRequest()
            { Id = roomId };

            var response = await grpcClient.RoomAvatarAsync(request);
            return JsonConvert.DeserializeObject<FileBox.FileBox>(response?.Filebox);
        }

        public override async Task<string> RoomCreate(IReadOnlyList<string> contactIdList, string? topic)
        {
            var request = new RoomCreateRequest();

            request.ContactIds.AddRange(contactIdList);
            request.Topic = topic;

            var response = await grpcClient.RoomCreateAsync(request);
            return response?.Id;
        }

        // TODO 可以合并为一个接口
        public override async Task<string> RoomCreate(IEnumerable<string> contactIdList, string? topic)
        {
            var request = new RoomCreateRequest();

            request.ContactIds.AddRange(contactIdList);
            request.Topic = topic;

            var response = await grpcClient.RoomCreateAsync(request);
            return response?.Id;
        }

        public override async Task<string> RoomCreate(string[] contactIdList, string? topic)
        {
            var request = new RoomCreateRequest();

            request.ContactIds.AddRange(contactIdList);
            if (topic != "")
            {
                request.Topic = topic;
            }

            var response = await grpcClient.RoomCreateAsync(request);
            return response?.Id;
        }

        public override async Task RoomDel(string roomId, string contactId)
        {
            var request = new RoomDelRequest()
            {
                ContactId = contactId,
                Id = roomId
            };
            await grpcClient.RoomDelAsync(request);
        }

        public override async Task RoomInvitationAccept(string roomInvitationId)
        {
            var request = new RoomInvitationAcceptRequest()
            {
                Id = roomInvitationId
            };
            await grpcClient.RoomInvitationAcceptAsync(request);
        }

        public override async Task<IReadOnlyList<string>> RoomList()
        {
            var response = await grpcClient.RoomListAsync(new RoomListRequest());
            return response?.Ids.ToList();
        }

        public override async Task<string[]> RoomMemberList(string roomId)
        {
            var request = new RoomMemberListRequest()
            {
                Id = roomId
            };

            var response = await grpcClient.RoomMemberListAsync(request);
            return response?.MemberIds.ToArray();
        }

        public override async Task<string> RoomQRCode(string roomId)
        {
            var request = new RoomQRCodeRequest()
            {
                Id = roomId
            };
            var response = await grpcClient.RoomQRCodeAsync(request);
            return response?.Qrcode;
        }

        public override async Task RoomQuit(string roomId)
        {
            var request = new RoomQuitRequest()
            {
                Id = roomId
            };
            await grpcClient.RoomQuitAsync(request);
        }

        public override async Task<string> RoomTopic(string roomId)
        {
            var request = new RoomTopicRequest()
            {
                Id = roomId
            };
            var response = await grpcClient.RoomTopicAsync(request);
            return response?.Topic;
        }

        // TODO  待确定
        public override async Task RoomTopic(string roomId, string topic)
        {
            var request = new RoomTopicRequest()
            {
                Id = roomId
            };
            if (!string.IsNullOrEmpty(topic))
            {
                request.Topic = topic;
            }

            var response = await grpcClient.RoomTopicAsync(request);
            //return response?.Topic;
        }
        #endregion
    }
}
