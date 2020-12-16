using System.Threading.Tasks;
using github.wechaty.grpc.puppet;

namespace Wechaty.Module.PuppetHostie
{
    public partial class GrpcPuppet
    {
        #region Friendship

        public override async Task FriendshipAccept(string friendshipId)
        {
            var request = new FriendshipAcceptRequest()
            { Id = friendshipId };

            await grpcClient.FriendshipAcceptAsync(request);
        }

        public override async Task FriendshipAdd(string contactId, string? hello)
        {
            var request = new FriendshipAddRequest()
            {
                ContactId = contactId,
                Hello = hello
            };
            var response = await grpcClient.FriendshipAddAsync(request);
        }

        public override async Task<string?> FriendshipSearchPhone(string phone)
        {
            var request = new FriendshipSearchPhoneRequest()
            {
                Phone = phone
            };

            var response = await grpcClient.FriendshipSearchPhoneAsync(request);
            return response?.ContactId;
        }

        public override async Task<string?> FriendshipSearchWeixin(string weixin)
        {
            var request = new FriendshipSearchWeixinRequest()
            { Weixin = weixin };

            var respnse = await grpcClient.FriendshipSearchWeixinAsync(request);
            return respnse?.ContactId;
        }
        #endregion
    }
}
