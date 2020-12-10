using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;

namespace Wechaty.PuppetHostie
{
    public partial class GrpcPuppet
    {
        #region Tag
        public override async Task TagContactAdd(string tagId, string contactId)
        {
            var request = new TagContactAddRequest()
            {
                Id = tagId,
                ContactId = contactId
            };
            await grpcClient.TagContactAddAsync(request);
        }

        public override async Task TagContactDelete(string tagId)
        {
            var request = new TagContactDeleteRequest()
            {
                Id = tagId
            };

            await grpcClient.TagContactDeleteAsync(request);
        }

        public override async Task<List<string>> TagContactList(string contactId)
        {
            var request = new TagContactListRequest();

            var response = await grpcClient.TagContactListAsync(request);
            return response.Ids.ToList();
        }

        public override async Task<List<string>> TagContactList()
        {
            var request = new TagContactListRequest();

            var response = await grpcClient.TagContactListAsync(request);
            return response.Ids.ToList();
        }

        public override async Task TagContactRemove(string tagId, string contactId)
        {
            var request = new TagContactRemoveRequest()
            {
                Id = tagId,
                ContactId = contactId
            };

            await grpcClient.TagContactRemoveAsync(request);
        }
        #endregion
    }
}
