using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;

namespace Wechaty.User
{
    public class RoomRepository : Accessory<RoomRepository>
    {
        private readonly ILogger<Room> _loggerForRoom;
        private readonly string? _name;

        private readonly ConcurrentDictionary<string, Room> _pool = new ConcurrentDictionary<string, Room>();

        public RoomRepository([DisallowNull] ILogger<Room> loggerForRoom,
                              [DisallowNull] Wechaty wechaty,
                              [DisallowNull] ILogger<RoomRepository> logger,
                              [AllowNull] string? name = null) : base(wechaty, logger, name)
        {
            _loggerForRoom = loggerForRoom;
            _name = name;
        }

        public async Task<Room> Create([DisallowNull] IReadOnlyList<Contact> contacts, [AllowNull] string? topic)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"create({string.Concat(contacts)},{topic})");
            }
            if (contacts.Count < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(contacts), contacts, "contactList need at least 2 contact to create a new room");
            }
            try
            {
                var contactIdList = contacts.Select(c => c.Id);
                var roomId = await Puppet.RoomCreate(contactIdList, topic);
                var room = Load(roomId);
                return room;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"create() exception: {exception.Message}");
                throw;
            }
        }

        public async Task<IReadOnlyList<Room>> FindAll([AllowNull] RoomQueryFilter? query = default)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"findAll({JsonConvert.SerializeObject(query)})");
            }
            try
            {
                var invalid = new ConcurrentDictionary<string, bool>();
                var roomIdList = await Puppet.RoomSearch(query);
                var roomList = roomIdList.Select(Load).ToList();
                await Task.WhenAll(roomList.Select(async r =>
                {
                    try
                    {
                        await r.Ready();
                    }
                    catch (Exception exception)
                    {
                        Logger.LogError(exception, $"findAll() room.ready() rejection: {exception.Message}");
                        invalid.TryAdd(r.Id, true);
                    }
                }));
                return roomList.Where(r => !invalid.ContainsKey(r.Id)).ToImmutableList();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"findAll rejected: {exception.Message}");
                return Array.Empty<Room>();
            }
        }

        public async Task<Room?> Find([DisallowNull] RoomQueryFilter query)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"find({JsonConvert.SerializeObject(query)})");
            }
            var roomList = await FindAll(query);
            if (roomList.Count > 1)
            {
                Logger.LogWarning($"find() got more than one({roomList.Count}) result");
            }
            for (var i = 0; i < roomList.Count; i++)
            {
                var room = roomList[i];
                if (await Puppet.RoomValidate(room.Id))
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace($"find() confirm room[#{i}] with id={room.Id} is valid result, return it.");
                    }
                    return room;
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        Logger.LogTrace($"find() confirm room[#{i}] with id={room.Id} is INVALID result, try next");
                    }
                }
            }
            Logger.LogWarning($"find() got {roomList.Count} rooms but no one is valid.");
            return null;
        }

        public Room Load([DisallowNull] string id)
        {
            if (_pool.TryGetValue(id, out var room))
            {
                return room;
            }
            room = new Room(id, WechatyInstance, _loggerForRoom, _name);
            return _pool.GetOrAdd(id, room);
        }

        public override RoomRepository ToImplement => this;
    }
}
