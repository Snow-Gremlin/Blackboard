using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a string value such that it can be used in generics.</summary>
    public struct String: IAdditive<String>, IComparable<String>,
        IImplicit<Bool, String>, IImplicit<Int, String>, IImplicit<Double, String> {

        /// <summary>The string value being stored.</summary>
        public readonly string Value;

        /// <summary>Creates a new string data value.</summary>
        /// <param name="value">The string value to store.</param>
        public String(string value = "") {
            this.Value = value;
        }

        /// <summary>This will concatinate this string to the other string.</summary>
        /// <param name="other">The other string to concatinate to the end of this string.</param>
        /// <returns>The concatinated string.</returns>
        public String Sum(String other) => new(this.Value + other.Value);

        /// <summary>Casts a boolean into a string for an implicit cast.</summary>
        /// <param name="value">The boolean value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Bool value) => new(value.Value.ToString());

        /// <summary>Casts an integer into a string for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Int value) => new(value.Value.ToString());

        /// <summary>Casts a double into a string for an implicit cast.</summary>
        /// <param name="value">The double value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Double value) => new(value.Value.ToString());

        /// <summary>Compares two strings together.</summary>
        /// <param name="other">The other string to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(String other) => string.Compare(this.Value, other.Value);
        public static bool operator ==(String left, String right) => left.CompareTo(right) == 0;
        public static bool operator !=(String left, String right) => left.CompareTo(right) != 0;
        public static bool operator <(String left, String right) => left.CompareTo(right) < 0;
        public static bool operator <=(String left, String right) => left.CompareTo(right) <= 0;
        public static bool operator >(String left, String right) => left.CompareTo(right) > 0;
        public static bool operator >=(String left, String right) => left.CompareTo(right) >= 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is String other && this.Value == other.Value;

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => "string";
    }
}
