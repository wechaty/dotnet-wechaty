using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Wechaty.Module.MemoryCard
{
    public interface IAsyncMap<TKey, TValue>
    {
        Task<int> Size { get; }

        IAsyncEnumerator<KeyValuePair<TKey, TValue>> Entries { get; }
        IAsyncEnumerator<TKey> Keys { get; }
        IAsyncEnumerator<TValue> Values { get; }

        Task<Optional<TValue>> Get(TKey key);
        Task Set(TKey key, TValue value);
        Task<bool> Has(TKey key);
        Task Delete(TKey key);
        Task Clear();
    }
}
