using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Types;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a boolean value such that it can be used in generics.</summary>
    public struct Bool:
        IEquatable<Bool> {

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
        /// <remarks>
        /// We're defining this uniquely instead of using the bool.ToString since
        /// those return "True" and "False" which don't match the desired Blackboard language.
        /// </remarks>
        public string ValueString => this.Value ? "true" : "false";

        #region Equatable...

        public static bool operator ==(Bool left, Bool right) => left.Equals(right);
        public static bool operator !=(Bool left, Bool right) => !left.Equals(right);

        /// <summary>Checks if the given bool is equal to this data type.</summary>
        /// <param name="other">This is the bool to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public bool Equals(Bool other) => this.Value == other.Value;

        /// <summary>Checks if the given object is equal to this data type.</summary>
        /// <param name="obj">This is the object to test.</param>
        /// <returns>True if they are equal, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Bool other && this.Equals(other);

        #endregion

        /// <summary>Gets the hash code of the stored value.</summary>
        /// <returns>The stored value's hash code.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>Gets the name of this data type and value.</summary>
        /// <returns>The name of the boolean type and value.</returns>
        public override string ToString() => this.TypeName+"("+this.ValueString+")";
    }
}
