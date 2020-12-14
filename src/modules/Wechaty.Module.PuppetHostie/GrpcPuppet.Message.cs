using System.Collections.Generic;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;
using Newtonsoft.Json;
using Wechaty.Module.FileBox;
using Wechaty.Schemas;

namespace Wechaty.Module.PuppetHostie
{
    public partial class GrpcPuppet
    {
        #region Message
        public override async Task<string> MessageContact(string messageId)
        {
            var request = new MessageContactRequest
            {
                Id = messageId
            };

            var response = await grpcClient.MessageContactAsync(request);
            return response.Id;
        }

        public override async Task<FileBox> MessageFile(string messageId)
        {
            var request = new MessageFileRequest
            {
                Id = messageId
            };

            var response = await grpcClient.MessageFileAsync(request);
            var filebox = response.Filebox;
            return FileBox.FromJson(filebox);

        }

        public override async Task<FileBox> MessageImage(string messageId, Schemas.ImageType imageType)
        {
            var request = new MessageImageRequest
            {
                Id = messageId,
                Type = (github.wechaty.grpc.puppet.ImageType)imageType
            };

            var response = await grpcClient.MessageImageAsync(request);
            var fileBox = response.Filebox;
            return FileBox.FromJson(fileBox);
        }

        public override async Task<MiniProgramPayload> MessageMiniProgram(string messageId)
        {
            var request = new MessageMiniProgramRequest
            {
                Id = messageId
            };

            var response = await grpcClient.MessageMiniProgramAsync(request);
            var payload = JsonConvert.DeserializeObject<MiniProgramPayload>(response.MiniProgram);
            return payload;
        }

        public override async Task<bool> MessageRecall(string messageId)
        {
            var request = new MessageRecallRequest
            {
                Id = messageId
            };

            var response = await grpcClient.MessageRecallAsync(request);
            if (response == null)
            {
                return false;
            }
            return response.Success;
        }

        public override async Task<string?> MessageSendContact(string conversationId, string contactId)
        {
            var request = new MessageSendContactRequest()
            {
                ConversationId = conversationId,
                ContactId = contactId
            };

            var response = await grpcClient.MessageSendContactAsync(request);
            return response?.Id;
        }

        public override async Task<string?> MessageSendFile(string conversationId, FileBox file)
        {
            var request = new MessageSendFileRequest
            {
                ConversationId = conversationId,
                Filebox = JsonConvert.SerializeObject(file.ToJson())
            };

            var response = await grpcClient.MessageSendFileAsync(request);
            return response?.Id;
        }

        public override async Task<string?> MessageSendMiniProgram(string conversationId, MiniProgramPayload miniProgramPayload)
        {
            var request = new MessageSendMiniProgramRequest
            {
                ConversationId = conversationId,
                MiniProgram = JsonConvert.SerializeObject(miniProgramPayload)
            };

            var response = await grpcClient.MessageSendMiniProgramAsync(request);
            return response?.Id;
        }

        public override async Task<string?> MessageSendText(string conversationId, string text, params string[]? mentionIdList)
        {
            var request = new MessageSendTextRequest()
            {
                ConversationId = conversationId,
                Text = text,
                //MentonalIds = mentonalIds
            };

            var response = await grpcClient.MessageSendTextAsync(request);
            return response?.Id;
        }

        public override async Task<string?> MessageSendText(string conversationId, string text, IEnumerable<string>? mentionIdList)
        {
            var request = new MessageSendTextRequest()
            {
                ConversationId = conversationId,
                Text = text,
                //MentonalIds = mentonalIds
            };

            var response = await grpcClient.MessageSendTextAsync(request);
            return response?.Id;
        }

        public override async Task<string?> MessageSendUrl(string conversationId, UrlLinkPayload urlLinkPayload)
        {
            var request = new MessageSendUrlRequest()
            {
                ConversationId = conversationId,
                UrlLink = JsonConvert.SerializeObject(urlLinkPayload)
            };

            var response = await grpcClient.MessageSendUrlAsync(request);
            return response?.Id;
        }

        public override async Task<UrlLinkPayload> MessageUrl(string messageId)
        {
            var request = new MessageUrlRequest()
            {
                Id = messageId
            };

            var response = await grpcClient.MessageUrlAsync(request);
            var payload = JsonConvert.DeserializeObject<UrlLinkPayload>(response.UrlLink);
            return payload;
        }
        #endregion
    }
}
