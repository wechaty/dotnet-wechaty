using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty
{

    //TODO:implement lru cache
    public class LRUCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _container;

        public LRUCache(LruOptions options) : this(options.MaxSize) { }

        public LRUCache(int capacity) => _container = new Dictionary<TKey, TValue>(capacity);

        public IReadOnlyList<TKey> Keys => _container.Keys.ToImmutableList();

        [return: MaybeNull]
        public TValue Get([DisallowNull] TKey key)
        {
            _ = _container.TryGetValue(key, out var value);
            return value;
        }

        public bool Set([DisallowNull] TKey key, [AllowNull] TValue value) => _container.TryAdd(key, value);

        public bool Delete(TKey key) => _container.Remove(key);
    }
}
