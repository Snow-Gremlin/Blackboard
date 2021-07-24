using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Data.Caps {

    /// <summary>This is the data storage for a boolean value such that it can be used in generics.</summary>
    public struct Bool: IComparable<Bool> {

        /// <summary>Assigns the predefined static values the first time they are used.</summary>
        static Bool() {
            True = new(true);
            False = new(false);
        }

        /// <summary>A predefined true boolean value.</summary>
        static public readonly Bool True;

        /// <summary>A predefined false boolean value.</summary>
        static public readonly Bool False;

        /// <summary>Gets the Bool for the given C# boolean value.</summary>
        /// <remarks>This should be used to instead of a new constructor to save memory were possible.</remarks>
        /// <param name="value">The value to wrap.</param>
        /// <returns>The predefined equivalent to the given value.</returns>
        static public Bool Wrap(bool value) => value ? True : False;

        /// <summary>The boolean value being stored.</summary>
        public readonly bool Value;

        /// <summary>Creates a new boolean value.</summary>
        /// <param name="value">The boolean value to store.</param>
        private Bool(bool value) {
            this.Value = value;
        }

        /// <summary>Compares two booleans together.</summary>
        /// <param name="other">The other boolean to compare.</param>
        /// <returns>The comparison result indicating which is greater than or equal.</returns>
        public int CompareTo(Bool other) => this.Value.CompareTo(other.Value);

        /// <summary>The name of this data type.</summary>
        /// <returns>The name of the bool type.</returns>
        public override string ToString() => "bool";
    }
}
