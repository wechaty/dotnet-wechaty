using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty.User
{
    /// <summary>
    /// accept room invitation
    /// </summary>
    public class RoomInvitation : Accessory<RoomInvitation>, IAcceptable
    {
        public string Id { get; }

        public RoomInvitation([DisallowNull] string id,
                              [DisallowNull] Wechaty wechaty,
                              [DisallowNull] ILogger<RoomInvitation> logger,
                              [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            Id = id;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({id})");
            }
        }

        ///<inheritdoc/>
        public override string ToString() => $"RoomInvitation#{Id ?? "loading"}";

        /// <summary>
        /// to string async
        /// </summary>
        /// <returns></returns>
        public async Task<string> ToStringAsync()
        {
            var payload = await Puppet.GetRoomInvitationPayload(Id);
            return $"RoomInvitation#{Id}<{payload.Topic},{payload.InviterId}>";
        }

        /// <summary>
        /// Accept Room Invitation
        /// </summary>
        /// <returns></returns>
        public async Task Accept()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"accept()");
            }
            await Puppet.RoomInvitationAccept(Id);
            var inviter = await Inviter();
            var topic = await Topic();
            try
            {
                await inviter.Ready();
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"accept() with room({topic}) & inviter({inviter}) ready()");
                }
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, $"accept() inviter({inviter}) is not ready because of {exception.Message}");
            }
        }

        /// <summary>
        /// Get the inviter from room invitation
        /// </summary>
        /// <returns></returns>
        public async Task<Contact> Inviter()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"inviter()");
            }
            var payload = await Puppet.GetRoomInvitationPayload(Id);
            var inviter = WechatyInstance.Contact.Load(payload.InviterId);
            return inviter;
        }

        /// <summary>
        /// Get the room topic from room invitation
        /// </summary>
        /// <returns></returns>
        public async Task<string> Topic() => (await Puppet.GetRoomInvitationPayload(Id)).Topic ?? "";

        /// <summary>
        /// use topic() instead
        /// </summary>
        [Obsolete("use topic() instead")]
        public Task<string> RoomTopic => Topic();

        /// <summary>
        /// member count of current <see cref="RoomInvitation"/>
        /// </summary>
        /// <returns></returns>
        public async Task<double> MemberCount()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"memberCount()");
            }
            var payload = await Puppet.GetRoomInvitationPayload(Id);
            return payload.MemberCount;
        }

        [Obsolete("roomMemberCount() DEPRECATED. use memberCount() instead.")]
        public Task<double> RoomMemberCount()
        {
            Logger.LogWarning($"roomMemberCount() DEPRECATED. use memberCount() instead.");
            return MemberCount();
        }

        public async Task<IReadOnlyList<Contact>> MemberList()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"roomMemberList()");
            }
            var payload = await Puppet.GetRoomInvitationPayload(Id);

            var list = payload.MemberIdList.Select(id => WechatyInstance.Contact.Load(id));
            await Task.WhenAll(list.Select(c => c.Ready()));
            return list.ToImmutableList();
        }

        public Task<IReadOnlyList<Contact>> RoomMemberList()
        {
            Logger.LogWarning($"roomMemberList() DEPRECATED. use memberList() instead.");
            return MemberList();
        }

        public async Task<DateTime> Date()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"date()");
            }
            var payload = await Puppet.GetRoomInvitationPayload(Id);
            return payload.Timestamp.TimestampToDateTime();
        }

        /// <summary>
        /// Returns the roopm invitation age in seconds.
        /// For example, the invitation is sent at time `8:43:01`,
        /// and when we received it in Wechaty, the time is `8:43:15`,
        /// then the age() will return `8:43:15 - 8:43:01 = 14 (seconds)`
        /// </summary>
        /// <returns></returns>
        public async Task<long> Age()
        {
            var receiveDate = await Date();
            var timstamp = DateTime.Now - receiveDate;
            return (long)timstamp.TotalSeconds;
        }

        public async Task<string> ToJson()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"toJSON()");
            }
            var payload = await Puppet.GetRoomInvitationPayload(Id);
            return JsonConvert.SerializeObject(payload);
        }

        public override RoomInvitation ToImplement => this;
    }
}
