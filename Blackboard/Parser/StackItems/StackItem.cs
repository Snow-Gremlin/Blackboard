using PetiteParser.Scanner;

namespace Blackboard.Parser.StackItems {

    /// <summary>The base class for all stack items.</summary>
    public abstract class StackItem {

        /// <summary>The location for this stack item.</summary>
        public readonly Location Location;

        /// <summary>Creates a new stack item.</summary>
        /// <param name="loc">The location of this stack item.</param>
        protected StackItem(Location loc) {
            this.Location = loc;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Unspecified stack item at "+Location;
    }
}
