using System;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty.EventEmitter
{
    /// <summary>
    /// state of: on/off/pending
    /// </summary>
    public readonly struct State : IEquatable<State>, IComparable<State>
    {
        /// <summary>
        /// symbol of pending state
        /// </summary>
        public const string PendingSymbol = "pending";

        /// <summary>
        /// symbol of on state
        /// </summary>
        public const string OnSymbol = "on";

        /// <summary>
        /// symbol of off state
        /// </summary>
        public const string OffSymbol = "off";

        /// <summary>
        /// state pending
        /// </summary>
        public static readonly State Pending = PendingSymbol;

        /// <summary>
        /// state on
        /// </summary>
        public static readonly State On = true;

        /// <summary>
        /// state off
        /// </summary>
        public static readonly State Off = false;

        private readonly bool? _state;
        private readonly bool? _pending;

        private State(bool? state, bool? pending)
        {
            _state = state;
            _pending = pending;
        }

        /// <summary>
        /// convert bool to <see cref="State"/>
        /// </summary>
        /// <param name="state"></param>
        public static implicit operator State(bool state) => new State(state, null);


        /// <summary>
        /// convert string to <see cref="State"/>
        /// </summary>
        /// <param name="pending"></param>
        public static implicit operator State(string pending)
        {
            switch (pending)
            {
                case PendingSymbol:
                    return new State(null, true);
                case OnSymbol:
                    return new State(true, null);
                case OffSymbol:
                    return new State(false, null);
                default:
                    throw new InvalidCastException($"can't cast {pending} to `Pending`");
            }
        }

        /// <summary>
        /// convert state to bool
        /// </summary>
        /// <param name="state"></param>
        public static implicit operator bool(State state) => state != Off;

        ///<inheritdoc/>
        public override string ToString()
        {
            if (_pending.HasValue)
            {
                return PendingSymbol;
            }
            return _state.GetValueOrDefault() ? "on" : "off";
        }

        ///<inheritdoc/>
        public override bool Equals(object obj) => obj is State state ? state == this : false;

        ///<inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(_state, _pending);

        ///<inheritdoc/>
        public bool Equals([DisallowNull] State other) => other._pending == _pending && other._state == _state;

        ///<inheritdoc/>
        public int CompareTo([AllowNull] State other) => other == this ? 0 : GetHashCode().CompareTo(other.GetHashCode());

        ///<inheritdoc/>
        public static bool operator ==(State left, State right) => left.Equals(right);

        ///<inheritdoc/>
        public static bool operator !=(State left, State right) => !(left == right);

        ///<inheritdoc/>
        public static bool operator <(State left, State right) => left.CompareTo(right) < 0;

        ///<inheritdoc/>
        public static bool operator <=(State left, State right) => left.CompareTo(right) <= 0;

        ///<inheritdoc/>
        public static bool operator >(State left, State right) => left.CompareTo(right) > 0;

        ///<inheritdoc/>
        public static bool operator >=(State left, State right) => left.CompareTo(right) >= 0;
    }
}
