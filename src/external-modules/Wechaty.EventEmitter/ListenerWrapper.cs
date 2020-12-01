using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EventEmitter
{
    internal struct ListenerWrapper
    {
        public Action<object?[]> Process { get; }

        private readonly Counter _counter;

        private HashKey Key { get; }

        public ListenerWrapper Increment()
        {
            _ = _counter.Increment();
            return this;
        }

        public void Decrement()
        {
            var decremented = _counter.Decrement();
            if (decremented <= 0)
            {
                Destory();
            }
        }

        public void Destory() => _ = ListenerWrappers.TryRemove(Key, out _);

        private ListenerWrapper([DisallowNull] Action<object?[]> process, HashKey key)
        {
            Process = process;
            Key = key;
            _counter = new Counter();
        }

        private readonly struct HashKey : IEquatable<HashKey>, IComparable<HashKey>
        {
            public HashKey(object? target, MethodInfo method)
            {
                Target = target;
                Method = method;
            }

            public object? Target { get; }
            public MethodInfo Method { get; }

            public int CompareTo([DisallowNull] HashKey other) => Equals(other) ? 0 : GetHashCode() - other.GetHashCode();

            public bool Equals([DisallowNull] HashKey other) => other.Target == Target && other.Method == Method;

            public override int GetHashCode() => HashCode.Combine(Target, Method);
        }

        private static readonly ConcurrentDictionary<HashKey, ListenerWrapper> ListenerWrappers
            = new ConcurrentDictionary<HashKey, ListenerWrapper>();

        public static ListenerWrapper GetListenerWrapper([DisallowNull] Delegate listener)
        {
            if (listener.Method.ReturnType != typeof(void))
            {
                throw new ArgumentException("listener must be return void.");
            }
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers.GetOrAdd(hashKey, k => new ListenerWrapper(args => listener.DynamicInvoke(args), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper([DisallowNull] Action listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers.GetOrAdd(hashKey, k => new ListenerWrapper(args => listener(), hashKey));
        }

#pragma warning disable CS8604 // 可能的 null 引用参数。
        public static ListenerWrapper GetListenerWrapper<T1>([DisallowNull] Action<T1> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2>([DisallowNull] Action<T1, T2> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3>([DisallowNull] Action<T1, T2, T3> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3, T4>([DisallowNull] Action<T1, T2, T3, T4> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3, T4, T5>([DisallowNull] Action<T1, T2, T3, T4, T5> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3, T4, T5, T6>([DisallowNull] Action<T1, T2, T3, T4, T5, T6> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3, T4, T5, T6, T7>([DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]), hashKey));
        }

        public static ListenerWrapper GetListenerWrapper<T1, T2, T3, T4, T5, T6, T7, T8>([DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
        {
            var hashKey = new HashKey(listener.Target, listener.Method);
            return ListenerWrappers
                           .GetOrAdd(hashKey,
                               k => new ListenerWrapper(args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7]), hashKey));
        }
#pragma warning restore CS8604 // 可能的 null 引用参数。
    }
}
