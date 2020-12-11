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
using System.Threading;

namespace Wechaty.PuppetHostie
{
    public partial class GrpcPuppet : WechatyPuppet
    {

        protected PuppetOptions options { get; }
        protected ILogger<WechatyPuppet> logger { get; }

        public GrpcPuppet(PuppetOptions _options, ILogger<WechatyPuppet> _logger, ILoggerFactory loggerFactory)
            : base(_options, _logger, loggerFactory)
        {
            options = _options;
            logger = _logger;
        }


        #region GRPC 连接
        protected const string CHATIE_ENDPOINT = "https://api.chatie.io/v0/hosties/";
        private Puppet.PuppetClient grpcClient = null;
        private Channel channel = null;

        /// <summary>
        /// GRPC  重连次数，超过该次数则放弃重连
        /// </summary>
        private int GRPCReconnectionCount = 3;

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

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

                if (string.IsNullOrEmpty(endPoint))
                {
                    var model = await DiscoverHostieIp(Options.Token);
                    if (model.IP == "0.0.0.0" || model.Port == "0")
                    {
                        throw new Exception("no endpoint");
                    }
                    endPoint = model.IP + ":" + model.Port;
                }

                var options = new List<ChannelOption>()
                {
                    new ChannelOption("grpc.default_authority", Options.Token)
                };
                channel = new Channel(endPoint, ChannelCredentials.Insecure, options);

                grpcClient = new PuppetClient(channel);

            }
            catch (Exception ex)
            {
                throw ex;
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
        protected async Task StartGrpcStreamAsync()
        {
            try
            {
                var eventStream = grpcClient.Event(new EventRequest());

                while (await eventStream.ResponseStream.MoveNext())
                {
                    OnGrpcStreamEvent(eventStream.ResponseStream.Current);
                }
            }
            catch (Exception ex)
            {
                var eventResetPayload = new EventResetPayload()
                {
                    Data = ex.StackTrace
                };
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

                //Console.WriteLine($"dateTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {eventType},PayLoad:{payload}");

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
                        Emit(JsonConvert.DeserializeObject<EventHeartbeatPayload>(payload));
                        break;
                    case EventType.Message:
                        Emit(JsonConvert.DeserializeObject<EventMessagePayload>(payload));
                        break;
                    case EventType.Dong:
                        Emit(JsonConvert.DeserializeObject<EventDongPayload>(payload));
                        break;
                    case EventType.Error:
                        Emit(JsonConvert.DeserializeObject<EventErrorPayload>(payload));
                        break;
                    case EventType.Friendship:
                        Emit(JsonConvert.DeserializeObject<EventFriendshipPayload>(payload));
                        break;
                    case EventType.RoomInvite:
                        Emit(JsonConvert.DeserializeObject<EventRoomInvitePayload>(payload));
                        break;
                    case EventType.RoomJoin:
                        Emit(JsonConvert.DeserializeObject<EventRoomJoinPayload>(payload));
                        break;
                    case EventType.RoomLeave:
                        Emit(JsonConvert.DeserializeObject<EventRoomLeavePayload>(payload));
                        break;
                    case EventType.RoomTopic:
                        Emit(JsonConvert.DeserializeObject<EventRoomTopicPayload>(payload));
                        break;
                    case EventType.Scan:
                        Emit(JsonConvert.DeserializeObject<EventScanPayload>(payload));
                        break;
                    case EventType.Ready:
                        Emit(JsonConvert.DeserializeObject<EventReadyPayload>(payload));
                        break;
                    case EventType.Reset:
                        //log.warn('PuppetHostie', 'onGrpcStreamEvent() got an EventType.EVENT_TYPE_RESET ?')
                        // the `reset` event should be dealed not send out
                        Emit(JsonConvert.DeserializeObject<EventResetPayload>(payload));
                        Logger.LogWarning("onGrpcStreamEvent() got an EventType.EVENT_TYPE_RESET ?");
                        break;
                    case EventType.Login:
                        var loginPayload = JsonConvert.DeserializeObject<EventLoginPayload>(payload);
                        SelfId = loginPayload.ContactId;
                        break;
                    case EventType.Logout:
                        SelfId = string.Empty;
                        //await _localEventBus.PublishAsync(JsonConvert.DeserializeObject<EventLogoutPayload>(payload));
                        Emit(JsonConvert.DeserializeObject<EventLogoutPayload>(payload));
                        break;
                    default:
                        Console.WriteLine($"'eventType {eventType} unsupported! (code should not reach here)");

                        //    //throw new BusinessException($"'eventType {_event.Type.ToString()} unsupported! (code should not reach here)");
                        break;
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
        public override WechatyPuppet ToImplement => this;

        public override async Task StartGrpc()
        {
            try
            {
                if (options.Token == "")
                {
                    throw new ArgumentException("wechaty-puppet-hostie: token not found. See: <https://github.com/wechaty/wechaty-puppet-hostie#1-wechaty_puppet_hostie_token>");
                }

                await StartGrpcClient();

                await grpcClient.StartAsync(new StartRequest());

                _ = StartGrpcStreamAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StartGrpcClient() exception,Grpc Retry Surplus Count {GRPCReconnectionCount}");
                if (GRPCReconnectionCount == 0)
                {
                    throw new Exception(ex.StackTrace);
                }
                GRPCReconnectionCount -= 1;
                Thread.Sleep(3000);
                await StopGrpcClient();
                Thread.Sleep(2000);
                await StartGrpc();
            }
        }


        public override void Ding(string? data)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
