using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Wechaty.Module.Puppet.Schemas
{
    public class RoomMemberQueryFilter : IFilter
    {
        StringOrRegex? IFilter.this[string key]
        {
            get
            {
                switch (key)
                {
                    case nameof(Name):
                        return Name;
                    case nameof(RoomAlias):
                        return RoomAlias;
                    case nameof(ContactAlias):
                        return ContactAlias;
                    default:
                        throw new MissingMemberException(GetType().FullName, key);
                }
            }
        }

        private readonly ImmutableList<string> _keys = new List<string>
        {
            nameof(Name),
            nameof(RoomAlias),
            nameof(ContactAlias)
        }.ToImmutableList();

        IReadOnlyList<string> IFilter.Keys => _keys;

        public string? Name { get; set; }
        public string? RoomAlias { get; set; }
        public string? ContactAlias { get; set; }
    }

    public class RoomQueryFilter : IFilter
    {
        StringOrRegex? IFilter.this[string key]
        {
            get
            {
                switch (key)
                {
                    case nameof(Id):
                        return Id;
                    case nameof(Topic):
                        return Topic;
                    default:
                        throw new MissingMemberException(GetType().FullName, key);
                }
            }
        }

        private readonly ImmutableList<string> _keys = new List<string>
        {
            nameof(Id),
            nameof(Topic)
        }.ToImmutableList();

        IReadOnlyList<string> IFilter.Keys => _keys;
        public string? Id { get; set; }
        public StringOrRegex? Topic { get; set; }
    }

    public delegate bool RoomPayloadFilterFunction(RoomPayload payload);

    public delegate RoomPayloadFilterFunction RoomPayloadFilterFactory(RoomQueryFilter query);

    public class RoomPayload
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public string? Avatar { get; set; }
        public List<string> MemberIdList { get; set; }
        public string? OwnerId { get; set; }
        public List<string> AdminIdList { get; set; }
    }

    public class RoomMemberPayload
    {
        public string Id { get; set; }
        public string? RoomAlias { get; set; }
        public string? InviterId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
    }
}
