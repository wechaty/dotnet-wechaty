using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using github.wechaty.grpc.puppet;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Module.Puppet;
using Wechaty.Module.Puppet.Schemas;
using static Wechaty.Puppet;

namespace Wechaty.Module.PuppetService
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
        private PuppetClient grpcClient = null;
        private GrpcChannel channel = null;

        /// <summary>
        /// This channel argument controls the amount of time (in milliseconds) the sender of the keepalive ping waits for an acknowledgement.
        /// If it does not receive an acknowledgment within this time, it will close the connection.
        /// </summary>
        private readonly int _keepAliveTimeoutMs = 30000;

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

                if (string.IsNullOrEmpty(endPoint))
                {
                    var model = await DiscoverHostieIp(Options.Token);
                    if (model.IP == "0.0.0.0" || model.Port == "0")
                    {
                        throw new Exception("no endpoint");
                    }
                    endPoint = "https://" + model.IP + ":" + model.Port;

                }

                var credentials = CallCredentials.FromInterceptor((context, metadata) =>
                {
                    if (!string.IsNullOrEmpty(Options.Token))
                    {
                        metadata.Add("Authorization", $"Wechaty {Options.Token}");
                    }
                    return Task.CompletedTask;
                });
                var channelCredentials = ChannelCredentials.Create(new SslCredentials(), credentials);


                if (!endPoint.ToUpper().StartsWith("HTTPS://"))
                {
                    throw new ApplicationException($"endpoint please set as HTTPS protocol,eg https://localhost");
                }

                //var cert = new X509Certificate2(@"D:\coding\github\wechaty\dotnet\dotnet-wechaty\src\Wechaty.Getting.Start\Certs\client.pfx", "1111");

                //var handler = new HttpClientHandler();
                //handler.ClientCertificates.Add(cert);
                //using var httpClient = new HttpClient(handler);

                channel = GrpcChannel.ForAddress(endPoint, new GrpcChannelOptions
                {
                    //HttpClient = httpClient,
                    Credentials = channelCredentials,
                    HttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    },
                });

                grpcClient = new PuppetClient(channel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static SslCredentials? _credentials;

        private static SslCredentials GetSslCredentials()
        {
            if (_credentials == null)
            {
                _credentials = new SslCredentials(
                   File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Certs", "ca.crt")),
                    new KeyCertificatePair(
                          File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Certs", "client.crt")),
                         File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Certs", "client.key"))));
            }

            return _credentials;
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

                StopRequest st = new StopRequest()
                {

                };

                CallOptions call = new CallOptions()
                {

                };

                grpcClient.Stop(st, call);

                var eventResetPayload = new EventResetPayload()
                {
                    Data = ex.StackTrace
                };
                Emit(eventResetPayload);
                throw ex;
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

                logger.LogInformation($"dateTime:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {eventType},PayLoad:{payload}");

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
                        Emit(JsonConvert.DeserializeObject<EventLogoutPayload>(payload));
                        break;
                    default:
                        logger.LogWarning($"'eventType {eventType} unsupported! (code should not reach here)");

                        //throw new BusinessException($"'eventType {_event.Type.ToString()} unsupported! (code should not reach here)");
                        break;
                }
            }
            catch (Exception ex)
            {
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
