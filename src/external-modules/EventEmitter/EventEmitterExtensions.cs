using System;
using System.Diagnostics.CodeAnalysis;

namespace EventEmitter
{
    /// <summary>
    /// 事件发射器扩展类
    /// </summary>
    public static class EventEmitterExtensions
    {
        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter}(TEventEmitter, string, Delegate)"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter}(TEventEmitter, string, Delegate)"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Delegate listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter}(TEventEmitter, string, Action)"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter}(TEventEmitter, string, Action)"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1}(TEventEmitter, string, Action{T1})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1}(TEventEmitter, string, Action{T1})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2}(TEventEmitter, string, Action{T1, T2})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2}(TEventEmitter, string, Action{T1, T2})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3}(TEventEmitter, string, Action{T1, T2, T3})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3}(TEventEmitter, string, Action{T1, T2, T3})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4}(TEventEmitter, string, Action{T1, T2, T3, T4})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4}(TEventEmitter, string, Action{T1, T2, T3, T4})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3, T4>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5}(TEventEmitter, string, Action{T1, T2, T3, T4, T5})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5}(TEventEmitter, string, Action{T1, T2, T3, T4, T5})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3, T4, T5>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3, T4, T5, T6>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6, T7}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6, T7})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6, T7}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6, T7})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3, T4, T5, T6, T7>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6, T7, T8}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6, T7, T8})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener{TEventEmitter, T1, T2, T3, T4, T5, T6, T7, T8}(TEventEmitter, string, Action{T1, T2, T3, T4, T5, T6, T7, T8})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter RemoveListener<TEventEmitter, T1, T2, T3, T4, T5, T6, T7, T8>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
             where TEventEmitter : IEventEmitter<TEventEmitter>
        {
            var wrapper = ListenerWrapper.GetListenerWrapper(listener);
            wrapper.Decrement();
            return eventEmitter.RemoveListener(eventName, wrapper.Process);
        }

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Delegate listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3, T4>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3, T4, T5>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3, T4, T5, T6>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3, T4, T5, T6, T7>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.On(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter On<TEventEmitter, T1, T2, T3, T4, T5, T6, T7, T8>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.On(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Delegate listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3, T4>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3, T4, T5>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3, T4, T5, T6>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3, T4, T5, T6, T7>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);

        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// Helper method for <see cref="IEventEmitter{TEventEmitter}.Once(string, Action{object[]})"/>.
        /// </summary>
        /// <typeparam name="TEventEmitter"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="eventEmitter"></param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        [return: NotNullIfNotNull("eventEmitter")]
        public static TEventEmitter Once<TEventEmitter, T1, T2, T3, T4, T5, T6, T7, T8>(
            [DisallowNull] this TEventEmitter eventEmitter,
            [DisallowNull] string eventName,
            [DisallowNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> listener)
            where TEventEmitter : IEventEmitter<TEventEmitter> => eventEmitter.Once(eventName, ListenerWrapper.GetListenerWrapper(listener).Increment().Process);
    }
}
