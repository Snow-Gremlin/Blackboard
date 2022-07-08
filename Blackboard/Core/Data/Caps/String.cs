using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a string value such that it can be used in generics.</summary>
    public struct String:
        IAdditive<String>,
        IComparable<String>,
        IData,
        IEquatable<String>,
        IImplicit<Bool,   String>,
        IImplicit<Double, String>,
        IImplicit<Int,    String>,
        IImplicit<Object, String> {

        #region Static...

        /// <summary>Gets an empty string. This is the same as default.</summary>
        static public readonly String Empty = new("");

        /// <summary>The identity of summation for this data type.</summary>
        static public readonly String SumIdentity = Empty;

        #endregion

        /// <summary>The string value being stored.</summary>
        public readonly string Value;

        /// <summary>Creates a new string data value.</summary>
        /// <param name="value">The string value to store.</param>
        public String(string value) => this.Value = value ?? "";

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

        /// <summary>
        /// Indicates that for this data type, summation is commutable,
        /// meaning that the order of the parents makes no difference to the result.
        /// </summary>
        /// <remarks>String summation is concatenation which is not commutable.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
        public bool SumCommutable => false;

        /// <summary>The identity of summation for this data type.</summary>
        /// <remarks>Typically this is zero or empty.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
        public String SumIdentityValue => SumIdentity;

        #endregion
        #region Comparable...

        public static bool operator < (String left, String right) => left.CompareTo(right) <  0;
        public static bool operator <=(String left, String right) => left.CompareTo(right) <= 0;
        public static bool operator > (String left, String right) => left.CompareTo(right) >  0;
        public static bool operator >=(String left, String right) => left.CompareTo(right) >= 0;

        /// <summary>Compares two strings together.</summary>
        /// <param name="other">The other string to compare.</param>
        /// <returns>The comparison result indicating which is greater than, less than, or equal.</returns>
        public int CompareTo(String other) => string.Compare(this.Value, other.Value);

        #endregion
        #region Data...

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.String.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueAsString => this.Value ?? "";

        /// <summary>Get the value of the data as an object.</summary>
        public object ValueAsObject => this.Value;

        #endregion
        #region Equatable...

        public static bool operator ==(String left, String right) => left.Equals(right);
        public static bool operator !=(String left, String right) => !left.Equals(right);

        /// <summary>Checks if the given string is equal to this data type.</summary>
        /// <param name="other">This is the string to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(String other) => this.Value == other.Value;

        #endregion
        #region Casts...

        /// <summary>Casts a boolean into a string for an implicit cast.</summary>
        /// <param name="value">The boolean value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Bool value) => new(value.ValueAsString);

        /// <summary>Casts a double into a string for an implicit cast.</summary>
        /// <param name="value">The double value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Double value) => new(value.ValueAsString);

        /// <summary>Casts an integer into a string for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Int value) => new(value.ValueAsString);

        /// <summary>Casts an object into a string for an implicit cast.</summary>
        /// <param name="value">The object value to cast.</param>
        /// <returns>The resulting string value.</returns>
        public String CastFrom(Object value) => new(value.ValueAsString);

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value?.GetHashCode() ?? 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is String other && this.Equals(other);

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the string type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueAsString+")";
    }
}
