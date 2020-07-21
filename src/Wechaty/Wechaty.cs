using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Wechaty.Schemas;
using Wechaty.User;

namespace Wechaty
{

    public delegate IDisposable WechatPlugin(Wechaty bot);

    public class Wechaty : EventEmitter<Wechaty>, ISayable
    {
        public StateSwitch State { get; }

        private readonly StateSwitch _readyState;

        public Wechaty Instance { get; }

        private static readonly List<IWechatPlugin> GlobalPlugins = new List<IWechatPlugin>();

        private MemoryCard? _memory;

        private Timer? _lifeTimer;
        //not supported yet.
        //private io?        : Io

        [DisallowNull]
        public Puppet Puppet { get; set; }
        public ContactRepository Contact { get; }
        public TagRepository Tag { get; }
        public ContactSelfRepository ContactSelf { get; }
        public FriendshipRepository Friendship { get; }
        public MessageRepository Message { get; }
        public ImageRepository Image { get; }
        public RoomInvitationRepository RoomInvitation { get; }
        public RoomRepository Room { get; }
        public UrlLinkRepository UrlLink { get; }
        public MiniProgramRepository MiniProgram { get; }

        public static void Use(params IWechatPlugin[] plugins) => GlobalPlugins.AddRange(plugins);


        private WechatyOptions _options;
        private readonly ILogger<Wechaty> _logger;

        public string Id { get; } = Guid.NewGuid().ToString();

        public Wechaty WechatyInstance => this;

        public Wechaty(WechatyOptions options, ILogger<Wechaty> logger)
        {
            _options = options;
            _logger = logger;
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("constructor() WechatyOptions.profile DEPRECATED. use WechatyOptions.name instead.");
            }
            if (string.IsNullOrWhiteSpace(options.Name) && !string.IsNullOrWhiteSpace(options.Profile))
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("constructor()");
                }
            }
            _memory = _options.Memory;
            Id = Guid.NewGuid().ToString();
            Instance = this;
            InstallGlobalPlugin();
        }

        public override string ToString()
        {
            if (_options == null)
            {
                return GetType().FullName ?? ("Unknow" + nameof(Wechaty));
            }
            return string.Concat("Wechaty#", Id, $"<{_options?.Puppet}>", $"{_memory?.Name}");
        }

        public string Name => _options?.Name ?? "Wechaty";

        public bool Emit(string? data) => Emit("dong", data);
        public bool Emit(Exception error) => Emit("error", error);
        public bool Emit(Friendship friendship) => Emit("friendship", friendship);
        public bool Emit(object data) => Emit("heartbeat", data);
        public bool Emit(ContactSelf user) => Emit("login", user);
        public bool Emit(ContactSelf user, string? reason) => Emit("logout", user, reason);
        public bool Emit(Message message) => Emit("message", message);
        public bool Emit(RoomInvitation roomInvitation) => Emit("room-invite", roomInvitation);
        public bool EmitRoomJoin(Room room, Contact[] inviteeList, Contact inviter, DateTime date) => Emit("room-join", room, inviteeList, inviter, date);
        public bool EmitRoomLeave(Room room, Contact[] leaverList, Contact remover, DateTime date) => Emit("room-leave", room, leaverList, remover, date);
        public bool Emit(Room room, string newTopic, string oldTopic, Contact changer, DateTime date) => Emit("room-topic", room, newTopic, oldTopic, changer, date);
        public bool Emit(string qrcode, ScanStatus status, string data) => Emit("scan", qrcode, status, data);

        private void InstallGlobalPlugin() => GlobalPlugins.ForEach(plugin => plugin.Install(this));

        public Task<Message> Say(string text, Contact replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Say(string text, Contact[] replyTo)
        {
            throw new NotImplementedException();
        }

        public override Wechaty ToImplement() => this;
    }
}
