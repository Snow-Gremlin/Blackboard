using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a string value such that it can be used in generics.</summary>
    public struct String: IAdditive<String>, IComparable<String>,
        IImplicit<IData, String> {

        /// <summary>Gets an empty string. This is the same as default.</summary>
        static public readonly String Empty = new("");

        /// <summary>The string value being stored.</summary>
        public readonly string Value;

        /// <summary>Creates a new string data value.</summary>
        /// <param name="value">The string value to store.</param>
        public String(string value) => this.Value = value ?? "";

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.String.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueString => this.Value;

        #region Additive Math...

        /// <summary>This will concatinate this string to the other string.</summary>
        /// <param name="other">The other string to concatinate to the end of this string.</param>
        /// <returns>The concatinated string.</returns>
        public String Sum(String other) => new(this.Value + other.Value);

        #endregion
        #region Casts...

        /// <summary>Casts a data into a string for an implicit cast.</summary>
        /// <param name="value">The boolean value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(IData value) => new(value.ValueString);

        #endregion
        #region Camparable...

        /// <summary>Compares two strings together.</summary>
        /// <param name="other">The other string to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(String other) => string.Compare(this.Value, other.Value);

        public static bool operator ==(String left, String right) => left.CompareTo(right) == 0;
        public static bool operator !=(String left, String right) => left.CompareTo(right) != 0;
        public static bool operator < (String left, String right) => left.CompareTo(right) <  0;
        public static bool operator <=(String left, String right) => left.CompareTo(right) <= 0;
        public static bool operator > (String left, String right) => left.CompareTo(right) >  0;
        public static bool operator >=(String left, String right) => left.CompareTo(right) >= 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is String other && this.Value == other.Value;

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
