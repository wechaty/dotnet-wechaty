using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;
using Newtonsoft.Json;
using Wechaty.Module.Filebox;

namespace Wechaty.Module.PuppetService
{
    public partial class GrpcPuppet
    {
        #region Contact

        public override async Task<string> ContactAlias(string contactId)
        {
            var request = new ContactAliasRequest
            {
                Id = contactId
            };

            var response = await grpcClient.ContactAliasAsync(request);

            return response.Alias;
        }

        // TODO 待确认
        public override async Task ContactAlias(string contactId, string? alias)
        {
            var request = new ContactAliasRequest();
            if (!string.IsNullOrEmpty(alias))
            {
                request.Alias = alias;
            }
            request.Id = contactId;

            await grpcClient.ContactAliasAsync(request);

        }

        public override async Task<FileBox> ContactAvatar(string contactId)
        {
            var request = new ContactAvatarRequest
            {
                Id = contactId
            };

            var response = await grpcClient.ContactAvatarAsync(request);
            var filebox = response.Filebox;
            return FileBox.FromJson(filebox);
        }

        public override async Task ContactAvatar(string contactId, FileBox file)
        {
            var request = new ContactAvatarRequest
            {
                Id = contactId,
                Filebox= JsonConvert.SerializeObject(file)
            };
            await grpcClient.ContactAvatarAsync(request);
        }

        public override async Task<List<string>> ContactList()
        {
            var response = await grpcClient.ContactListAsync(new ContactListRequest());
            return response?.Ids.ToList();
        }

        public override async Task ContactSelfName(string name)
        {
            var request = new ContactSelfNameRequest();
            await grpcClient.ContactSelfNameAsync(request);
        }

        public override async Task<string> ContactSelfQRCode()
        {
            var response = await grpcClient.ContactSelfQRCodeAsync(new ContactSelfQRCodeRequest());
            return response?.Qrcode;
        }

        public override async Task ContactSelfSignature(string signature)
        {
            var request = new ContactSelfSignatureRequest
            {
                Signature = signature
            };

            await grpcClient.ContactSelfSignatureAsync(request);
        }
        #endregion
    }
}
