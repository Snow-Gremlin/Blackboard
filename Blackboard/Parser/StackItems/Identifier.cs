using PetiteParser.Scanner;

namespace Blackboard.Parser.StackItems {

    /// <summary>The stack item for a stored identifier.</summary>
    sealed internal class Identifier: StackItem {

        /// <summary>The identifier to apply to a receiver or scope.</summary>
        public readonly string Id;

        /// <summary>The value found at this identifier or null if nothing found.</summary>
        public readonly object Value;

        /// <summary>Creates a new identifier.</summary>
        /// <param name="loc">The location of the id.</param>
        /// <param name="id">The identifier to apply to the receiver.</param>
        /// <param name="value">The value found at this identifier or null.</param>
        public Identifier(Location loc, string id, object value): base(loc) {
            this.Id = id;
            this.Value = value;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Identifier " + this.Id + " at " + this.Location;
    }
}
