using System;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty.Module.MemoryCard
{
    public struct NumberOrString : IEquatable<NumberOrString>, IComparable<NumberOrString>
    {
        public NumberOrString(int? intValue, string? stringValue, PayloadDataType type)
        {
            IntValue = intValue;
            StringValue = stringValue;
            Type = type;
        }

        public int? IntValue { get; }

        public string? StringValue { get; }

        public PayloadDataType Type { get; }

        public int CompareTo([DisallowNull] NumberOrString other) => Equals(other) ? 0 : GetHashCode().CompareTo(other.GetHashCode());

        public bool Equals([DisallowNull] NumberOrString other) => other.Type == Type && other.IntValue == IntValue && other.StringValue == StringValue;


        public static implicit operator NumberOrString(string value) => new NumberOrString(null, value, PayloadDataType.String);
        public static implicit operator NumberOrString(int value) => new NumberOrString(value, null, PayloadDataType.Number);

        public override bool Equals(object obj) => obj is NumberOrString v && Equals(v);

        public override int GetHashCode() => HashCode.Combine(Type, IntValue, StringValue);

        public static bool operator ==(NumberOrString left, NumberOrString right) => left.Equals(right);

        public static bool operator !=(NumberOrString left, NumberOrString right) => !(left == right);

        public static bool operator <(NumberOrString left, NumberOrString right) => left.CompareTo(right) < 0;

        public static bool operator <=(NumberOrString left, NumberOrString right) => left.CompareTo(right) <= 0;

        public static bool operator >(NumberOrString left, NumberOrString right) => left.CompareTo(right) > 0;

        public static bool operator >=(NumberOrString left, NumberOrString right) => left.CompareTo(right) >= 0;
    }
}
