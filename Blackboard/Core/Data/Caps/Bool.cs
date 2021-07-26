using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a boolean value such that it can be used in generics.</summary>
    public struct Bool: IComparable<Bool> {

        /// <summary>The boolean value being stored.</summary>
        public readonly bool Value;

        /// <summary>Creates a new boolean value.</summary>
        /// <param name="value">The boolean value to store.</param>
        public Bool(bool value = false) {
            this.Value = value;
        }

        /// <summary>Compares two booleans together.</summary>
        /// <param name="other">The other boolean to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(Bool other) => this.Value.CompareTo(other.Value);

        public override bool Equals(object obj) => obj is Bool other && this.Value == other.Value;
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>The name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => "bool";

        public static bool operator == (Bool left, Bool right) => left.Equals(right);
        public static bool operator != (Bool left, Bool right) => !(left == right);
        public static bool operator < (Bool left, Bool right) => left.CompareTo(right) < 0;
        public static bool operator <= (Bool left, Bool right) => left.CompareTo(right) <= 0;
        public static bool operator > (Bool left, Bool right) => left.CompareTo(right) > 0;
        public static bool operator >= (Bool left, Bool right) => left.CompareTo(right) >= 0;
    }
}
