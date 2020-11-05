using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Wechaty.Schemas
{
    public enum ContactGender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }

    public enum ContactType
    {
        Unknown = 0,
        Individual = 1,
        Official = 2,
        [Obsolete("use Individual instead")]
        Personal = Individual,
    }

    public class ContactQueryFilter : IFilter
    {
        StringOrRegex? IFilter.this[string key]
        {
            get
            {
                switch (key)
                {
                    case nameof(Alias):
                        return Alias;
                    case nameof(Id):
                        return Id;
                    case nameof(Name):
                        return Name;
                    case nameof(Weixin):
                        return Weixin;
                    default:
                        throw new MissingMemberException(GetType().FullName, key);
                }
            }
        }

        private readonly ImmutableList<string> _keys = new List<string>
        {
            nameof(Alias),
            nameof(Id),
            nameof(Name),
            nameof(Weixin)
        }.ToImmutableList();

        IReadOnlyList<string> IFilter.Keys => _keys;

        public StringOrRegex? Alias { get; set; }
        public string? Id { get; set; }
        public StringOrRegex? Name { get; set; }
        public string? Weixin { get; set; }
    }

    public class ContactPayload
    {
        public string Id { get; set; }
        public ContactGender Gender { get; set; }
        public ContactType Type { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string? Address { get; set; }
        public string? Alias { get; set; }
        public string? City { get; set; }
        public bool? Friend { get; set; }
        public string? Province { get; set; }
        public string? Signature { get; set; }
        public bool? Star { get; set; }
        public string? Weixin { get; set; }
    }

    public delegate bool ContactPayloadFilterFunction(ContactPayload payload);

    public delegate ContactPayloadFilterFunction ContactPayloadFilterFactory(ContactQueryFilter query);

    public static class ContactPayloadFilterFunctionExtensions
    {
        public static ContactPayloadFilterFunction Every(this IEnumerable<ContactPayloadFilterFunction> functions) => payload => !functions.Any(f => !f(payload));
    }
}
