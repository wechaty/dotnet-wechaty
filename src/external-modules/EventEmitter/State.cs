using System;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty
{
    public readonly struct State : IEquatable<State>, IComparable<State>
    {
        private readonly bool? _state;
        private readonly bool? _pending;

        public const string PendingSymbol = "pending";
        public const string OnSymbol = "on";
        public const string OffSymbol = "off";

        public static readonly State Pending = PendingSymbol;

        public static readonly State On = true;

        public static readonly State Off = false;

        private State(bool? state, bool? pending)
        {
            _state = state;
            _pending = pending;
        }

        public static implicit operator State(bool state) => new State(state, null);

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

        public static implicit operator bool(State state) => state != Off;

        public override string ToString()
        {
            if (_pending.HasValue)
            {
                return PendingSymbol;
            }
            return _state.GetValueOrDefault() ? "on" : "off";
        }

        public override bool Equals(object obj) => obj is State state ? state == this : false;

        public override int GetHashCode() => HashCode.Combine(_state, _pending);

        public bool Equals([DisallowNull] State other) => other._pending == _pending && other._state == _state;

        public int CompareTo([AllowNull] State other) => other == this ? 0 : GetHashCode().CompareTo(other.GetHashCode());

        public static bool operator ==(State left, State right) => left.Equals(right);

        public static bool operator !=(State left, State right) => !(left == right);

        public static bool operator <(State left, State right) => left.CompareTo(right) < 0;

        public static bool operator <=(State left, State right) => left.CompareTo(right) <= 0;

        public static bool operator >(State left, State right) => left.CompareTo(right) > 0;

        public static bool operator >=(State left, State right) => left.CompareTo(right) >= 0;
    }
}
