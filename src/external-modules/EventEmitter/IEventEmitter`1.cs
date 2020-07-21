using System;
using System.Collections.Generic;

namespace Wechaty
{
    /// <summary>
    /// 事件发射器
    /// </summary>
    /// <typeparam name="TEventEmitter"></typeparam>
    public interface IEventEmitter<TEventEmitter>
        where TEventEmitter : IEventEmitter<TEventEmitter>
    {
        /// <summary>
        /// Alias for <see cref="On(string, Action{object[]})"/>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter AddListener(string eventName, Action<object[]> listener);
        /// <summary>
        /// Synchronously calls each of the listeners registered for the event named eventName,
        /// in the order they were registered, passing the supplied arguments to each.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        /// <returns>Returns true if the event had listeners, false otherwise.</returns>
        bool Emit(string eventName, params object[] args);
        /// <summary>
        /// Returns an array listing the events for which the emitter has registered listeners. The values in the array will be strings
        /// </summary>
        IReadOnlyList<string> EventNames { get; }
        /// <summary>
        /// The current max listener value for the EventEmitter or defaults to <see cref="EventEmitter.DefaultMaxListeners"/>.
        /// </summary>
        int MaxListeners { get; set; }
        /// <summary>
        /// Returns the number of listeners listening to the event named eventName.
        /// </summary>
        /// <param name="eventName">The name of the event being listened for</param>
        /// <returns>Returns the number of listeners listening to the event named eventName.</returns>
        int ListenerCount(string eventName);
        /// <summary>
        /// Returns a copy of the array of listeners for the event named eventName.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IReadOnlyList<Action<object[]>> Listeners(string eventName);
        /// <summary>
        /// Alias for <see cref="RemoveListener(string, Action{object[]})"/>.
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter Off(string eventName, Action<object[]> listener);
        /// <summary>
        /// Adds the listener function to the end of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter On(string eventName, Action<object[]> listener);
        /// <summary>
        /// Adds a one-time listener function for the event named eventName.
        /// The next time eventName is triggered, this listener is removed and then invoked.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter Once(string eventName, Action<object[]> listener);
        /// <summary>
        /// Adds the listener function to the beginning of the listeners array for the event named eventName.
        /// No checks are made to see if the listener has already been added.
        /// Multiple calls passing the same combination of eventName and listener will result in the listener being added, and called, multiple times.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter PrependListener(string eventName, Action<object[]> listener);
        /// <summary>
        /// Adds a one-time listener function for the event named eventName to the beginning of the listeners array.
        /// The next time eventName is triggered, this listener is removed, and then invoked.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter PrependOnceListener(string eventName, Action<object[]> listener);
        /// <summary>
        /// Removes all listeners, or those of the specified eventName.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter RemoveAllListener(string eventName);
        /// <summary>
        /// Removes the specified listener from the listener array for the event named eventName.
        /// <see cref="RemoveListener(string, Action{object[]})"/> will remove, at most, one instance of a listener from the listener array.
        /// If any single listener has been added multiple times to the listener array for the specified eventName,
        /// then <see cref="RemoveListener(string, Action{object[]})"/> must be called multiple times to remove each instance.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The callback function</param>
        /// <returns>Returns a reference to the EventEmitter, so that calls can be chained.</returns>
        TEventEmitter RemoveListener(string eventName, Action<object[]> listener);
        /// <summary>
        /// Returns a copy of the array of listeners for the event named eventName, including any wrappers (such as those created by <see cref="Once"/>).
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IReadOnlyList<Action<object[]>> RawListeners(string eventName);
    }
}
