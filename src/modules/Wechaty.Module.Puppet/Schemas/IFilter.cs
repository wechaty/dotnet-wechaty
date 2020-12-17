using System.Collections.Generic;

namespace Wechaty.Module.Puppet.Schemas
{
    internal interface IFilter
    {
        StringOrRegex? this[string key] { get; }
        IReadOnlyList<string> Keys { get; }
    }
}
