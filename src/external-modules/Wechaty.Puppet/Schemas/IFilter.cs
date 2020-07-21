using System.Collections.Generic;

namespace Wechaty.Schemas
{
    internal interface IFilter
    {
        StringOrRegex? this[string key] { get; }
        IReadOnlyList<string> Keys { get; }
    }
}
