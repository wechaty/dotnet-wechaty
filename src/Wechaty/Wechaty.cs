using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Wechaty.EventEmitter;
using Wechaty.Schemas;
using Wechaty.User;

namespace Wechaty
{
    /// <summary>
    /// wechaty
    /// </summary>
    public class Wechaty : EventEmitter<Wechaty>, ISayable
    {



        private const string PUPPET_MEMORY_NAME = "puppet";

        /// <summary>
        /// state
        /// </summary>
        public StateSwitch State { get; }

        private readonly StateSwitch _readyState;

        private static readonly List<IWechatPlugin> GlobalPlugins = new List<IWechatPlugin>();

        private MemoryCard? _memory;



        private Timer? _lifeTimer;
        //not supported yet.
        //private io?        : Io

        /// <summary>
        /// puppet of wechaty
        /// </summary>
        [DisallowNull]
        public WechatyPuppet Puppet { get; set; }

        /// <summary>
        /// repository of <see cref="User.Contact"/>
        /// </summary>
        public ContactRepository Contact { get; }

        /// <summary>
        /// repository of <see cref="User.Tag"/>
        /// </summary>
        public TagRepository Tag { get; }

        /// <summary>
        /// repository of <see cref="User.ContactSelf"/>
        /// </summary>
        public ContactSelfRepository ContactSelf { get; }

        /// <summary>
        /// repository of <see cref="User.Friendship"/>
        /// </summary>
        public FriendshipRepository Friendship { get; }

        /// <summary>
        /// repository of <see cref="User.Message"/>
        /// </summary>
        public MessageRepository Message { get; }

        /// <summary>
        /// repository of <see cref="User.Image"/>
        /// </summary>
        public ImageRepository Image { get; }

        /// <summary>
        /// repository of <see cref="User.RoomInvitation"/>
        /// </summary>
        public RoomInvitationRepository RoomInvitation { get; }

        /// <summary>
        /// repository of <see cref="User.Room"/>
        /// </summary>
        public RoomRepository Room { get; }

        /// <summary>
        /// repository of <see cref="User.UrlLink"/>
        /// </summary>
        public UrlLinkRepository UrlLink { get; }

        /// <summary>
        /// repository of <see cref="User.MiniProgram"/>
        /// </summary>
        public MiniProgramRepository MiniProgram { get; }

        /// <summary>
        /// add plugin
        /// </summary>
        /// <param name="plugins"></param>
        public static void GloabalAdd(params IWechatPlugin[] plugins) => GlobalPlugins.AddRange(plugins);


        private readonly WechatyPuppetOptions _options;
        private readonly ILogger<Wechaty> _logger;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();

        ///<inheritdoc/>
        public Wechaty WechatyInstance => this;

        /// <summary>
        /// init <see cref="Wechaty"/> with <see cref="WechatyPuppetOptions"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        public Wechaty(PuppetOptions options)
        {

            ILoggerFactory loggerFactory = new LoggerFactory();
            var logger = new Logger<WechatyPuppet>(loggerFactory);

            var grpcPuppet = new GrpcPuppet(options, logger, loggerFactory);

            var wechatyOptions = new WechatyPuppetOptions()
            {
                Name = options.Name,
                Puppet = grpcPuppet,
            };


            _options = wechatyOptions;

            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Wechaty>();


            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("constructor() WechatyOptions.profile DEPRECATED. use WechatyOptions.name instead.");
            }
            if (string.IsNullOrWhiteSpace(options.Name))
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("constructor()");
                }
            }
            _memory = _options.Memory;
            Id = Guid.NewGuid().ToString();
            State = new StateSwitch("Wechaty", loggerFactory.CreateLogger<StateSwitch>());
            _readyState = new StateSwitch("WechatyReady", loggerFactory.CreateLogger<StateSwitch>());

            Contact = new ContactRepository(loggerFactory.CreateLogger<Contact>(), this, loggerFactory.CreateLogger<ContactRepository>());
            Tag = new TagRepository(loggerFactory.CreateLogger<Tag>(), this, loggerFactory.CreateLogger<TagRepository>());
            ContactSelf = new ContactSelfRepository(loggerFactory.CreateLogger<ContactSelf>(), this, loggerFactory.CreateLogger<ContactSelfRepository>());
            Friendship = new FriendshipRepository(loggerFactory.CreateLogger<Friendship>(), this, loggerFactory.CreateLogger<FriendshipRepository>());
            Message = new MessageRepository(loggerFactory.CreateLogger<Message>(), this, loggerFactory.CreateLogger<MessageRepository>());
            Image = new ImageRepository(loggerFactory.CreateLogger<Image>(), this, loggerFactory.CreateLogger<ImageRepository>());
            RoomInvitation = new RoomInvitationRepository(loggerFactory.CreateLogger<RoomInvitation>(), this, loggerFactory.CreateLogger<RoomInvitationRepository>());
            Room = new RoomRepository(loggerFactory.CreateLogger<Room>(), this, loggerFactory.CreateLogger<RoomRepository>());
            UrlLink = new UrlLinkRepository(this, loggerFactory.CreateLogger<UrlLinkRepository>());
            MiniProgram = new MiniProgramRepository();

            InstallGlobalPlugin();
        }

        ///<inheritdoc/>
        public override string ToString()
        {
            if (_options == null)
            {
                return GetType().FullName ?? ("Unknow" + nameof(Wechaty));
            }
            return string.Concat("Wechaty#", Id, $"<{_options?.Puppet}>", $"{_memory?.Name}");
        }

        /// <summary>
        /// name
        /// </summary>
        public string Name => _options?.Name ?? "Wechaty";

        public bool EmitDong(string? data) => Emit("dong", data);
        public bool EmitError(Exception error) => Emit("error", error);
        public bool EmitFriendship(Friendship friendship) => Emit("friendship", friendship);
        public bool EmitHearbeat(object data) => Emit("heartbeat", data);
        public bool EmitLogin(ContactSelf user) => Emit("login", user);
        public bool EmitLogout(ContactSelf user, string? reason) => Emit("logout", user, reason);
        public bool EmitMessage(Message message) => Emit("message", message);
        public bool EmitRoomInvite(RoomInvitation roomInvitation) => Emit("room-invite", roomInvitation);
        public bool EmitRoomJoin(Room room, IReadOnlyList<Contact> inviteeList, Contact inviter, DateTime date) => Emit("room-join", room, inviteeList, inviter, date);
        public bool EmitRoomLeave(Room room, IReadOnlyList<Contact> leaverList, Contact remover, DateTime date) => Emit("room-leave", room, leaverList, remover, date);
        public bool EmitRoomTopic(Room room, string newTopic, string oldTopic, Contact changer, DateTime date) => Emit("room-topic", room, newTopic, oldTopic, changer, date);
        public bool EmitScan(string qrcode, ScanStatus status, string? data) => Emit("scan", qrcode, status, data);

        public bool EmitStart() => Emit("start");

        public bool EmitStop() => Emit("stop");

        public Wechaty OnDong(WechatyDongEventListener listener) => this.On("dong", listener);
        public Wechaty OnError(WechatyErrorEventListener listener) => this.On("error", listener);
        public Wechaty OnFriendship(WechatyFriendshipEventListener listener) => this.On("friendship", listener);
        public Wechaty OnHeartbeat(WechatyHeartbeatEventListener listener) => this.On("heartbeat", listener);
        public Wechaty OnLogin(WechatyLoginEventListener listener) => this.On("login", listener);
        public Wechaty OnLogout(WechatyLogoutEventListener listener) => this.On("logout", listener);
        public Wechaty OnMessage(WechatyMessageEventListener listener) => this.On("message", listener);
        public Wechaty OnReady(WechatyReadyEventListener listener) => this.On("ready", listener);
        public Wechaty OnRoomInvite(WechatyRoomInviteEventListener listener) => this.On("room-invite", listener);
        public Wechaty OnRoomJoin(WechatyRoomJoinEventListener listener) => this.On("room-join", listener);
        public Wechaty OnRoomLeave(WechatyRoomLeaveEventListener listener) => this.On("room-leave", listener);
        public Wechaty OnRoomTopic(WechatyRoomTopicEventListener listener) => this.On("room-topic", listener);
        public Wechaty OnScan(WechatyScanEventListener listener) => this.On("scan", listener);
        public Wechaty OnStart(WechatyStartStopEventListener listener) => this.On("start", listener);
        public Wechaty OnStop(WechatyStartStopEventListener listener) => this.On("stop", listener);


        public Wechaty Use(params IWechatPlugin[] plugins)
        {
            Array.ForEach(plugins, plugin =>
            {
                plugin.Install(this);
            });
            return this;
        }

        private void InstallGlobalPlugin() => GlobalPlugins.ForEach(plugin => plugin.Install(this));

        private async Task InitPuppet()
        {
            if (Puppet != null)
            {
                return;
            }
            if (_memory == null)
            {
                throw new InvalidOperationException("no memory");
            }
            //TODO: should we implement resolve puppet by name like TypeScript?

            var puppet = _options.Puppet!;
            var memory = _memory.Multiplex(PUPPET_MEMORY_NAME);

            //Plug the Memory Card to Puppet
            puppet.SetMemory(memory);

            Puppet = puppet;
            _ = Emit("puppet", puppet);


        }

        protected void InitPuppetEventBridge(WechatyPuppet puppet)
        {
            _logger.LogInformation("init puppet event bridge");
            _ = puppet.OnDong(payload => EmitDong(payload.Data))
                .OnError(payload => EmitError(payload.Exception))
                .OnHeartbeat(payload => EmitHearbeat(payload.Data))
                .OnFriendship(async payload =>
                {
                    var friendship = Friendship.Load(payload.FriendshipId);
                    await friendship.Ready();
                    _ = EmitFriendship(friendship);
                    _ = friendship.Contact.Emit("friendship", friendship);
                })
                .OnLogin(async payload =>
                {
                    var contact = ContactSelf.Load(payload.ContactId);
                    await contact.Ready();
                    _ = EmitLogin(contact);
                })
                .OnLogout(async payload =>
                {
                    var contact = ContactSelf.Load(payload.ContactId);
                    await contact.Ready();
                    _ = EmitLogout(contact, payload.Data);
                })
                .OnMessage(async payload =>
                {
                    try
                    {
                        var message = Message.Load(payload.MessageId);
                        await message.Ready;
                        _ = EmitMessage(message);
                        var room = message.Room;
                        if (room != null)
                        {
                            _ = room.EmitMessage(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        EmitError(ex);
                    }
                }).OnReady(payload =>
                {
                    _ = Emit("ready");
                    _readyState.IsOn = true;
                })
                .OnRoomInvite(payload =>
                {
                    var roomInvitation = RoomInvitation.Load(payload.RoomInvitationId);
                    _ = EmitRoomInvite(roomInvitation);
                })
                .OnRoomJoin(async payload =>
                {
                    var room = Room.Load(payload.RoomId);
                    await room.Sync();

                    var inviteeList = payload
                    .InviteeIdList
                    .Select(Contact.Load)
                    .ToList();
                    await Task.WhenAll(inviteeList.Select(c => c.Ready()));

                    var inviter = Contact.Load(payload.InviterId);
                    await inviter.Ready();
                    var date = payload.Timestamp.TimestampToDateTime();

                    _ = EmitRoomJoin(room, inviteeList, inviter, date);
                    _ = room.EmitJoin(inviteeList, inviter, date);
                })
                .OnRoomLeave(async payload =>
                {
                    var room = Room.Load(payload.RoomId);
                    await room.Sync();

                    var leaverList = payload.RemoveeIdList.Select(Contact.Load).ToList();
                    await Task.WhenAll(leaverList.Select(c => c.Ready()));

                    var remover = Contact.Load(payload.RemoverId);
                    await remover.Ready();

                    var date = payload.Timestamp.TimestampToDateTime();

                    _ = EmitRoomLeave(room, leaverList, remover, date);
                    _ = room.EmitLeave(leaverList, remover, date);

                    var selftId = Puppet.SelfId;
                    if (!string.IsNullOrEmpty(selftId) && payload.RemoveeIdList.Contains(selftId))
                    {
                        await Puppet.RoomPayloadDirty(payload.RoomId);
                        await Puppet.RoomMemberPayloadDirty(payload.RoomId);
                    }
                }).OnRoomTopic(async payload =>
                {
                    var room = Room.Load(payload.RoomId);
                    await room.Sync();

                    var changer = Contact.Load(payload.ChangerId);
                    await changer.Ready();
                    var date = payload.Timestamp.TimestampToDateTime();

                    _ = EmitRoomTopic(room, payload.NewTopic, payload.OldTopic, changer, date);
                })
                .OnScan(payload =>
                {
                    _ = EmitScan(payload.Qrcode ?? "", payload.Status, payload.Data);
                });
        }

        public async Task Start()
        {
            if (State.IsOn)
            {
                await State.Ready("on");
                return;
            }
            _readyState.IsOff = true;
            if (_lifeTimer != null)
            {
                throw new InvalidOperationException("start life timer exist");
            }

            State.IsOn = EventEmitter.State.PendingSymbol;

            try
            {
                if (_memory == null)
                {
                    _memory = new MemoryCard(_options.Name, _loggerFactory.CreateLogger<MemoryCard>(), _loggerFactory);
                }
                try
                {
                    await _memory.Load();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "start memory load failed.");
                }
                await InitPuppet();
                await Puppet.Start();
                InitPuppetEventBridge(Puppet);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "start failed.");
                EmitError(exception);
                try
                {
                    await Stop();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "start stop failed.");
                    EmitError(e);
                }
            }
            OnHeartbeat((data) => MemoryCheck());

            _readyState.IsOn = true;
            EmitStart();
        }

        public async Task Stop()
        {
            //todo: implement stop
        }

        public Task Ready() => _readyState.Ready(EventEmitter.State.On);

        public async Task Logout()
        {
            //todo: implement logout;
        }

        public async Task<bool> Logonoff()
        {
            //todo: implement logonoff;
            throw new NotImplementedException();
        }

        public ContactSelf UserSelf()
        {
            //todo: implement userSelf;
            throw new NotImplementedException();
        }

        public Task<Message> Say(string text, params Contact[]? replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Say(Contact contact, params Contact[]? replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Say(FileBox fileBox, params Contact[]? replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Say(MiniProgram miniProgram, params Contact[]? replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Say(UrlLink urlLink, params Contact[]? replyTo)
        {
            throw new NotImplementedException();
        }

        public void Ding()
        {
            throw new NotImplementedException();
        }

        private void MemoryCheck()
        {
            //ignore
        }

        public async Task Reset()
        {
            await Puppet.Stop();
            await Puppet.Start();
        }

        public override Wechaty ToImplement => this;
    }
}
