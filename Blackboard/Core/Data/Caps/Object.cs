using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a base value type, object, such that it can be used in generics.</summary>
    public struct Object:
        IData,
        IEquatable<Object>, 
        IImplicit<Bool,   Object>,
        IImplicit<Double, Object>,
        IImplicit<Int,    Object>,
        IImplicit<String, Object>,
        INullable {

        #region Static...

        /// <summary>Gets an object with no value, this is the default value.</summary>
        static public readonly Object Null = new(null);

        #endregion

        /// <summary>The object value being stored.</summary>
        public readonly object Value;

        /// <summary>Creates a new object data value.</summary>
        /// <param name="value">The object value to store.</param>
        public Object(object value) => this.Value = value;

        #region Data...

        /// <summary>Gets the name for the type of data.</summary>
        public string TypeName => Type.Object.Name;

        /// <summary>Get the value of the data as a string.</summary>
        public string ValueAsString => this.Value?.ToString() ?? "null";

        /// <summary>Get the value of the data as an object.</summary>
        public object ValueAsObject => this.Value;

        #endregion
        #region Equatable...

        public static bool operator ==(Object left, Object right) =>  left.Equals(right);
        public static bool operator !=(Object left, Object right) => !left.Equals(right);

        /// <summary>Checks if the given double is equal to this data type.</summary>
        /// <param name="other">This is the double to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(Object other) =>
            this.Value is null ? other.Value is null : this.Value.Equals(other.Value);

        #endregion
        #region Casts...

        /// <summary>Casts a boolean into an object for an implicit cast.</summary>
        /// <param name="value">The boolean value to cast.</param>
        /// <returns>The resulting object value.</returns>
        public Object CastFrom(Bool value) => new(value.Value);

        /// <summary>Casts a double into an object for an implicit cast.</summary>
        /// <param name="value">The double value to cast.</param>
        /// <returns>The resulting object value.</returns>
        public Object CastFrom(Double value) => new(value.Value);

        /// <summary>Casts an integer into an object for an implicit cast.</summary>
        /// <param name="value">The integer value to cast.</param>
        /// <returns>The resulting object value.</returns>
        public Object CastFrom(Int value) => new(value.Value);

        /// <summary>Casts an string into an object for an implicit cast.</summary>
        /// <param name="value">The string value to cast.</param>
        /// <returns>The resulting object value.</returns>
        public Object CastFrom(String value) => new(value.Value);

        #endregion
        #region Nullable...

        /// <summary>Determines if the this value is null.</summary>
        /// <returns>True if null, false otherwise.</returns>
        public Bool IsNull() => new(this.Value is null);

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value?.GetHashCode() ?? 0;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object? obj) => obj is Object other && this.Equals(other);

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the double type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueAsString+")";
    }
}
