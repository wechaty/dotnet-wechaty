using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wechaty.Schemas;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using github.wechaty.grpc.puppet;
using Grpc.Core;
using static Wechaty.Puppet;

namespace Wechaty
{
    public class GrpcPuppet : WechatyPuppet
    {

        protected PuppetOptions options { get; }
        protected ILogger<WechatyPuppet> logger { get; }

        protected string selfId { get; set; }

        public GrpcPuppet(PuppetOptions _options, ILogger<WechatyPuppet> _logger, ILoggerFactory loggerFactory)
            : base(_options, _logger, loggerFactory)
        {
            options = _options;
            logger = _logger;
        }


        #region GRPC 连接
        protected const string CHATIE_ENDPOINT = "https://api.chatie.io/v0/hosties/";
        private Puppet.PuppetClient? grpcClient = null;
        private Grpc.Net.Client.GrpcChannel channel = null;


        /// <summary>
        /// 发现 hostie gateway 对应的服务是否能能访问
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task<HostieEndPoint> DiscoverHostieIp(string token)
        {
            try
            {
                var url = CHATIE_ENDPOINT + token;

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var model = JsonConvert.DeserializeObject<HostieEndPoint>(await response.Content.ReadAsStringAsync());
                        return model;
                    }
                }
                throw new Exception("获取hostie gateway 对应的主机信息异常");
            }
            catch (Exception ex)
            {
                throw new Exception("获取hostie gateway 对应的主机信息异常");
            }
        }

        /// <summary>
        /// 初始化 Grpc连接
        /// </summary>
        /// <returns></returns>
        protected async Task StartGrpcClient()
        {

            try
            {
                if (grpcClient != null)
                {
                    throw new Exception("puppetClient had already inited");
                }

                var endPoint = Options.Endpoint;
                if (string.IsNullOrEmpty(endPoint))
                {
                    var model = await DiscoverHostieIp(Options.Token);
                    if (model.IP == "0.0.0.0" || model.Port == "0")
                    {
                        throw new Exception("no endpoint");
                    }
                    // 方式一
                    endPoint = "http://" + model.IP + ":" + model.Port;

                    Options.Endpoint = endPoint;

                    // 方式二
                    //endPoint = model.IP + ":" + model.Port;
                }

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

                // 方式一
                channel = Grpc.Net.Client.GrpcChannel.ForAddress(endPoint);
                grpcClient = new PuppetClient(channel);

                // 方式二
                //channel = new Channel(endPoint, ChannelCredentials.Insecure);
                //grpcClient = new PuppetClient(channel);

                //try
                //{
                //    var version = grpcClient.Version(new VersionRequest()).Version;

                //    //var resonse = grpcClient.Ding(new DingRequest() { Data = "ding" });

                //    //await channel.ShutdownAsync();
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        /// <summary>
        /// 关闭Grpc连接
        /// </summary>
        /// <returns></returns>
        protected async Task StopGrpcClient()
        {
            if (channel == null || grpcClient == null)
            {
                throw new Exception("puppetClient had not initialized");
            }
            await channel.ShutdownAsync();
            grpcClient = null;
        }

        /// <summary>
        /// 双向数据流事件处理
        /// </summary>
        protected void StartGrpcStream()
        {
            try
            {
                var eventStream = grpcClient.Event(new EventRequest());

                var stream = Task.Run(async () =>
                {
                    await foreach (var resp in eventStream.ResponseStream.ReadAllAsync())
                    {
                        OnGrpcStreamEvent(resp);
                    }
                });
            }
            catch (Exception ex)
            {
                var eventResetPayload = new EventResetPayload()
                {
                    Data = ex.StackTrace
                };
                //_localEventBus.PublishAsync(eventResetPayload);
                Emit(eventResetPayload);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected void OnGrpcStreamEvent(EventResponse @event)
        {
            try
            {
                var eventType = @event.Type;
                var payload = @event.Payload;

                Console.WriteLine($"{eventType},PayLoad:{payload}");

                if (eventType != EventType.Heartbeat)
                {
                    var eventHeartbeatPayload = new EventHeartbeatPayload()
                    {
                        Data = $"onGrpcStreamEvent({eventType})"
                    };
                    //await _localEventBus.PublishAsync(eventHeartbeatPayload);
                    Emit(eventHeartbeatPayload);
                }

                switch (eventType)
                {
                    case EventType.Unspecified:
                        Logger.LogError("onGrpcStreamEvent() got an EventType.EVENT_TYPE_UNSPECIFIED ?");
                        break;
                    case EventType.Heartbeat:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventHeartbeatPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventHeartbeatPayload>(payload));
                        break;
                    case EventType.Message:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventMessagePayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventMessagePayload>(payload));
                        break;
                    case EventType.Dong:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventDongPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventDongPayload>(payload));
                        break;
                    case EventType.Error:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventErrorPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventErrorPayload>(payload));
                        break;
                    case EventType.Friendship:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<FriendshipPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventFriendshipPayload>(payload));
                        break;
                    case EventType.RoomInvite:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventRoomInvitePayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventRoomInvitePayload>(payload));
                        break;
                    case EventType.RoomJoin:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventRoomJoinPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventRoomJoinPayload>(payload));
                        break;
                    case EventType.RoomLeave:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventRoomLeavePayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventRoomLeavePayload>(payload));
                        break;
                    case EventType.RoomTopic:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventRoomTopicPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventRoomTopicPayload>(payload));
                        break;
                    case EventType.Scan:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventScanPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventScanPayload>(payload));
                        break;
                    case EventType.Ready:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventReadyPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventReadyPayload>(payload));
                        break;
                    case EventType.Reset:
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventResetPayload>(payload));
                        //log.warn('PuppetHostie', 'onGrpcStreamEvent() got an EventType.EVENT_TYPE_RESET ?')
                        // the `reset` event should be dealed not send out
                        Emit(JsonConvert.DeserializeObject<EventResetPayload>(payload));
                        Logger.LogWarning("onGrpcStreamEvent() got an EventType.EVENT_TYPE_RESET ?");
                        break;
                    case EventType.Login:
                        var loginPayload = JsonConvert.DeserializeObject<EventLoginPayload>(payload);
                        selfId = loginPayload.ContactId;
                        ////await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventLoginPayload>(payload));
                        break;
                    case EventType.Logout:
                        selfId = string.Empty;
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventLogoutPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventLogoutPayload>(payload));
                        break;
                        //default:
                        //    Console.WriteLine($"'eventType {_event.Type.ToString()} unsupported! (code should not reach here)");

                        //    //throw new BusinessException($"'eventType {_event.Type.ToString()} unsupported! (code should not reach here)");
                        //    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Logger.LogError(ex, "OnGrpcStreamEvent exception");
            }

        }
        protected void StopGrpcStream()
        {
            //_localEventBus.UnsubscribeAll(typeof(LocalEventBus));
        }

        #endregion

        #region 实现抽象类
        public override WechatyPuppet ToImplement => throw new NotImplementedException();

        public override async Task StartGrpc()
        {
            try
            {
                if (options.Token == "")
                {
                    throw new Exception("wechaty-puppet-hostie: token not found. See: <https://github.com/wechaty/wechaty-puppet-hostie#1-wechaty_puppet_hostie_token>");
                }
                StartGrpcStream();

                await grpcClient.StartAsync(new StartRequest());                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw ex;
            }
        }


        public override void Ding(string? data)
        {
            throw new NotImplementedException();
        }


        #region Contact

        public override Task<string> ContactAlias(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task ContactAlias(string ontactId, string? alias)
        {
            throw new NotImplementedException();
        }

        public override Task<FileBox> ContactAvatar(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task ContactAvatar(string contactId, FileBox file)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> ContactList()
        {
            throw new NotImplementedException();
        }

        public override Task ContactSelfName(string name)
        {
            throw new NotImplementedException();
        }

        public override Task<string> ContactSelfQRCode()
        {
            throw new NotImplementedException();
        }

        public override Task ContactSelfSignature(string signature)
        {
            throw new NotImplementedException();
        }
        #endregion



        #region Friendship

        public override Task FriendshipAccept(string friendshipId)
        {
            throw new NotImplementedException();
        }

        public override Task FriendshipAdd(string contactId, string? hello)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> FriendshipSearchPhone(string phone)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> FriendshipSearchWeixin(string weixin)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Message

        public override Task<string> MessageContact(string messageId)
        {
            throw new NotImplementedException();
        }

        public override Task<FileBox> MessageFile(string messageId)
        {
            throw new NotImplementedException();
        }

        public override Task<FileBox> MessageImage(string messageId, Schemas.ImageType imageType)
        {
            throw new NotImplementedException();
        }

        public override Task<MiniProgramPayload> MessageMiniProgram(string messageId)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> MessageRecall(string messageId)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendContact(string conversationId, string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendFile(string conversationId, FileBox file)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendMiniProgram(string conversationId, MiniProgramPayload miniProgramPayload)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendText(string conversationId, string text, params string[]? mentionIdList)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendText(string conversationId, string text, IEnumerable<string>? mentionIdList)
        {
            throw new NotImplementedException();
        }

        public override Task<string?> MessageSendUrl(string conversationId, UrlLinkPayload urlLinkPayload)
        {
            throw new NotImplementedException();
        }

        public override Task<UrlLinkPayload> MessageUrl(string messageId)
        {
            throw new NotImplementedException();
        }
        #endregion

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

        #region Tag
        public override Task TagContactAdd(string tagId, string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task TagContactDelete(string tagId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> TagContactList(string contactId)
        {
            throw new NotImplementedException();
        }

        public override Task<List<string>> TagContactList()
        {
            throw new NotImplementedException();
        }

        public override Task TagContactRemove(string tagId, string contactId)
        {
            throw new NotImplementedException();
        }
        #endregion

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

        #endregion
    }
}
