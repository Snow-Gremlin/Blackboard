using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a boolean value such that it can be used in generics.</summary>
    public struct Bool: IComparable<Bool> {

        /// <summary>Gets a boolean value for true.</summary>
        static public readonly Bool True = new(true);

        /// <summary>Gets a boolean value for false. This is the same as default.</summary>
        static public readonly Bool False = new(false);

        /// <summary>The boolean value being stored.</summary>
        public readonly bool Value;

        /// <summary>Creates a new boolean value.</summary>
        /// <param name="value">The boolean value to store.</param>
        public Bool(bool value) => this.Value = value;

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.Bool.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueString => this.Value ? "true" : "false";

        #region Comparable...

        /// <summary>Compares two booleans together.</summary>
        /// <param name="other">The other boolean to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(Bool other) => this.Value.CompareTo(other.Value);

        public static bool operator ==(Bool left, Bool right) => left.CompareTo(right) == 0;
        public static bool operator !=(Bool left, Bool right) => left.CompareTo(right) != 0;
        public static bool operator < (Bool left, Bool right) => left.CompareTo(right) <  0;
        public static bool operator <=(Bool left, Bool right) => left.CompareTo(right) <= 0;
        public static bool operator > (Bool left, Bool right) => left.CompareTo(right) >  0;
        public static bool operator >=(Bool left, Bool right) => left.CompareTo(right) >= 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Bool other && this.Value == other.Value;

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
