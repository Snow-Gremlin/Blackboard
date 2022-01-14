using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Attributes;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a string value such that it can be used in generics.</summary>
    [Commutable(false)]
    public struct String:
        IAdditive<String>,
        IComparable<String>,
        IImplicit<Bool,   String>,
        IImplicit<Double, String>,
        IImplicit<Int,    String> {

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

        #region Additive...

        /// <summary>This will concatenate of the given other data.</summary>
        /// <remarks>The current value is not used in the concatenation.</remarks>
        /// <param name="other">The other data to concatenate together.</param>
        /// <returns>The concatenation of the given other data values.</returns>
        public String Sum(IEnumerable<String> other) {
            StringBuilder buf = new();
            other.Select(t => t.Value).Foreach(buf.Append);
            return new(buf.ToString());
        }

        #endregion
        #region Comparable...

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

        /// <summary>Checks if the given string is equal to this data type.</summary>
        /// <param name="other">This is the string to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(String other) => this.Value == other.Value;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is String other && this.Equals(other);

        /// <summary>Gets the maximum value from the given other values.</summary>
        /// <remarks>The current value is not used in the maximum value.</remarks>
        /// <param name="other">The values to find the maximum from.</param>
        /// <returns>The maximum value from the given vales.</returns>
        public String Max(IEnumerable<String> other) => new(other.Max(t => t.Value));

        /// <summary>Gets the minimum value from given other values.</summary>
        /// <remarks>The current value is not used in the minimum value.</remarks>
        /// <param name="other">The values to find the minimum from.</param>
        /// <returns>The minimum value from the given vales.</returns>
        public String Min(IEnumerable<String> other) => new(other.Min(t => t.Value));

        /// <summary>Gets this value clamped to the inclusive range of the given min and max.</summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The value clamped between the given values.</returns>
        public String Clamp(String min, String max) => (this < min) ? min : (this > max) ? max : this;

        #endregion
        #region Casts...

        /// <summary>Casts a boolean into a string for an implicit cast.</summary>
        /// <param name="value">The boolean value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Bool value) => new(value.ValueString);

        /// <summary>Casts a double into a string for an implicit cast.</summary>
        /// <param name="value">The double value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Double value) => new(value.ValueString);

        /// <summary>Casts an integer into a string for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Int value) => new(value.ValueString);

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the string type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
