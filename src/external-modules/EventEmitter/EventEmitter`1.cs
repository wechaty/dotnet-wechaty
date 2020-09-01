using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace EventEmitter
{
    /// <summary>
    /// event emitter
    /// </summary>
    /// <typeparam name="TEventEmitter"></typeparam>
    public abstract class EventEmitter<TEventEmitter> : IEventEmitter<TEventEmitter>, IInheritance<TEventEmitter, EventEmitter<TEventEmitter>>
        where TEventEmitter : EventEmitter<TEventEmitter>
    {
        internal class Handler : IComparable<Handler>, IEquatable<Handler>
        {
            private int _processCount;

            public Handler(Action<object?[]> listener, bool once = false)
            {
                Listener = listener;
                Once = once;
            }

            public bool Once { get; private set; }

            public int ProcessCount => _processCount;

            public Action<object?[]> Listener { get; }

            public int CompareTo([AllowNull] Handler other) => Equals(other) ? 0 : (other == null ? Listener.GetHashCode() : Listener.GetHashCode() - other.Listener.GetHashCode());

            public bool Equals([AllowNull] Handler other) => other != null && other.Listener == Listener;

            public void Process(object?[] args)
            {
                var incremented = Interlocked.Increment(ref _processCount);
                if (Once && incremented != 1)
                {
                    return;
                }
                Listener(args);
            }
        }

        /// <summary>
        /// The EventEmitter instance will emit its own <see cref="NewListenerSymbol"/> event before a listener is added to its internal array of listeners.
        /// </summary>
        public static string NewListenerSymbol { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The <see cref="RemoveListenerSymbol"/> event is emitted after the listener is removed.
        /// </summary>
        public static string RemoveListenerSymbol { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// This symbol shall be used to install a listener for only monitoring 'error' events.
        /// Listeners installed using this symbol are called before the regular 'error' listeners are called.
        /// Installing a listener using this symbol does not change the behavior once an 'error' event is emitted,
        /// therefore the process will still crash if no regular 'error' listener is installed.
        /// </summary>
        public static string ErrorMonitorSymbol { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// By default, a maximum of 10 listeners can be registered for any single event.
        /// This limit can be changed for individual EventEmitter instances using the emitter.setMaxListeners(n) method.
        /// To change the default for all EventEmitter instances, the EventEmitter.defaultMaxListeners property can be used.
        /// If this value is not a positive number, a TypeError will be thrown.
        /// </summary>
        public static int DefaultMaxListeners { get; set; } = 10;

        private readonly ConcurrentDictionary<string, List<Handler>> _listeners = new ConcurrentDictionary<string, List<Handler>>();

        ///<inheritdoc/>
        public virtual IReadOnlyList<string> EventNames
        {
            get
            {
                var names = new List<string>();
                names.AddRange(_listeners.Keys);
                return names;
            }
        }

        ///<inheritdoc/>
        public virtual int MaxListeners { get; set; }

        ///<inheritdoc/>
        public virtual TEventEmitter AddListener(string eventName, Action<object?[]> listener) => On(eventName, listener);

        ///<inheritdoc/>
        public virtual bool Emit(string eventName, [AllowNull] params object?[] args)
        {
            var registered = false;
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                registered = true;
                for (var i = funcs.Count - 1; i > -1; i--)
                {
                    var item = funcs[i];
                    if (item.Once)
                    {
                        funcs.RemoveAt(i);
                        if (item.Listener.Target is ListenerWrapper wrapper)
                        {
                            wrapper.Decrement();
                        }
                    }
                    item.Process(args ?? Array.Empty<object>());
                }
            }
            return registered;
        }

        ///<inheritdoc/>
        public virtual int ListenerCount(string eventName) =>
            _listeners.TryGetValue(eventName, out var funcs1) ? funcs1.Count : 0;

        ///<inheritdoc/>
        public virtual IReadOnlyList<Action<object?[]>> Listeners(string eventName)
        {
            _ = _listeners.TryGetValue(eventName, out var funcs);
            return funcs?.Where(h => !h.Once).Select(h => h.Listener).ToList() ?? new List<Action<object?[]>>();
        }

        ///<inheritdoc/>
        public virtual TEventEmitter Off(string eventName, Action<object?[]> listener) => RemoveListener(eventName, listener);

        ///<inheritdoc/>
        public virtual TEventEmitter On(string eventName, Action<object?[]> listener)
        {
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                funcs.Add(new Handler(listener));
            }
            else
            {
                funcs = new List<Handler>
                {
                    new Handler(listener)
                };
                _ = _listeners.TryAdd(eventName, funcs);
            }
            return ToImplement;
        }

        ///<inheritdoc/>
        public virtual TEventEmitter Once(string eventName, Action<object?[]> listener)
        {
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                funcs.Add(new Handler(listener, true));
            }
            else
            {
                funcs = new List<Handler>
                {
                    new Handler(listener, true)
                };
                _ = _listeners.TryAdd(eventName, funcs);
            }
            return ToImplement;
        }

        ///<inheritdoc/>
        public virtual TEventEmitter PrependListener(string eventName, Action<object?[]> listener)
        {
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                funcs.Insert(0, new Handler(listener));
            }
            else
            {
                funcs = new List<Handler>
                {
                    new Handler(listener)
                };
                _ = _listeners.TryAdd(eventName, funcs);
            }
            return ToImplement;
        }

        ///<inheritdoc/>
        public virtual TEventEmitter PrependOnceListener(string eventName, Action<object?[]> listener)
        {
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                funcs.Insert(0, new Handler(listener, true));
            }
            else
            {
                funcs = new List<Handler>
                {
                    new Handler(listener, true)
                };
                _ = _listeners.TryAdd(eventName, funcs);
            }
            return ToImplement;
        }

        ///<inheritdoc/>
        public virtual IReadOnlyList<Action<object?[]>> RawListeners(string eventName)
        {
            _ = _listeners.TryGetValue(eventName, out var funcs);
            return funcs?.Select(h => h.Listener).ToList() ?? new List<Action<object?[]>>();
        }

        ///<inheritdoc/>
        public virtual TEventEmitter RemoveAllListener(string? eventName)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                _ = _listeners.TryRemove(eventName!, out _);
            }
            else
            {
                _listeners.Clear();
            }
            return ToImplement;
        }

        ///<inheritdoc/>
        public virtual TEventEmitter RemoveListener(string eventName, Action<object?[]> listener)
        {
            if (_listeners.TryGetValue(eventName, out var funcs))
            {
                for (var i = 0; i < funcs.Count; i++)
                {
                    if (funcs[i].Listener == listener)
                    {
                        funcs.RemoveAt(i);
                        if (funcs.Count == 0)
                        {
                            _ = _listeners.TryRemove(eventName, out _);
                        }
                        break;
                    }
                }
            }
            return ToImplement;
        }

        /// <summary>
        /// return this.
        /// </summary>
        /// <returns></returns>
        public abstract TEventEmitter ToImplement { get; }
    }
}
