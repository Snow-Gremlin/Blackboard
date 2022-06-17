using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a boolean value such that it can be used in generics.</summary>
    public struct Bool:
        IComparable<Bool>,
        IData,
        IEquatable<Bool> {

        #region Static...

        /// <summary>Gets a string for the false boolean value.</summary>
        /// <remarks>This is defined to match the Blackboard language instead of C#, "False".</remarks>
        public const string FalseString = "false";

        /// <summary>Gets a string for the true boolean value.</summary>
        /// <remarks>This is defined to match the Blackboard language instead of C#, "True".</remarks>
        public const string TrueString = "true";

        /// <summary>Gets a boolean value for false. This is the same as default.</summary>
        static public readonly Bool False = new(false);

        /// <summary>Gets a boolean value for true.</summary>
        static public readonly Bool True = new(true);

        #endregion

        /// <summary>The boolean value being stored.</summary>
        public readonly bool Value;

        /// <summary>Creates a new boolean value.</summary>
        /// <param name="value">The boolean value to store.</param>
        public Bool(bool value) => this.Value = value;

        #region Comparable...

        public static bool operator < (Bool left, Bool right) => left.CompareTo(right) <  0;
        public static bool operator <=(Bool left, Bool right) => left.CompareTo(right) <= 0;
        public static bool operator > (Bool left, Bool right) => left.CompareTo(right) >  0;
        public static bool operator >=(Bool left, Bool right) => left.CompareTo(right) >= 0;

        /// <summary>Compares two integers together.</summary>
        /// <param name="other">The other integer to compare.</param>
        /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
        public int CompareTo(Bool other) => this.Value.CompareTo(other.Value);

        #endregion
        #region Data...

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.Bool.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueString => this.Value ? TrueString : FalseString;

        /// <summary>Get the value of the data as an object.</summary>
        public object ValueObject => this.Value;

        #endregion
        #region Equatable...

        public static bool operator ==(Bool left, Bool right) => left.Equals(right);
        public static bool operator !=(Bool left, Bool right) => !left.Equals(right);

        /// <summary>Checks if the given bool is equal to this data type.</summary>
        /// <param name="other">This is the bool to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(Bool other) => this.Value == other.Value;

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Bool other && this.Equals(other);

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the boolean type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
