using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Module.Puppet.Schemas;

namespace Wechaty.User
{
    public class RoomInvitationRepository : Accessory<RoomInvitationRepository>
    {
        private readonly ILogger<RoomInvitation> _loggerForRoomInvitation;
        private readonly string? _name;

        public RoomInvitationRepository([DisallowNull] ILogger<RoomInvitation> loggerForRoomInvitation,
                                        [DisallowNull] Wechaty wechaty,
                                        [DisallowNull] ILogger<RoomInvitationRepository> logger,
                                        [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            _loggerForRoomInvitation = loggerForRoomInvitation;
            _name = name;
        }

        public RoomInvitation Load([DisallowNull] string id) => new RoomInvitation(id, WechatyInstance, _loggerForRoomInvitation, _name);

        public Task<RoomInvitation> FromJson(string payload) => FromJson(JsonConvert.DeserializeObject<RoomInvitationPayload>(payload));

        public async Task<RoomInvitation> FromJson(RoomInvitationPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"fromJson({JsonConvert.SerializeObject(payload)})");
            }
            await Puppet.SetRoomInvitationPayload(payload.Id, payload);
            return WechatyInstance.RoomInvitation.Load(payload.Id);
        }

        public override RoomInvitationRepository ToImplement => this;
    }
}
