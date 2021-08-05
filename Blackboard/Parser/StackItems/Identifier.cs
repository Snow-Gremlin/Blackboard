using PetiteParser.Scanner;

namespace Blackboard.Parser.StackItems {

    /// <summary>The stack item for an identifier.</summary>
    sealed public class Identifier: StackItem {

        /// <summary>The receiver object, usually a namespace.</summary>
        public readonly object Receiver;

        /// <summary>The identifier to apply to the receiver.</summary>
        public readonly string Id;

        /// <summary>Creates a new identifier.</summary>
        /// <param name="loc">The location of the id.</param>
        /// <param name="receiver">The receiver object for the id.</param>
        /// <param name="id">The identifier to apply to the receiver.</param>
        public Identifier(Location loc, object receiver, string id): base(loc) {
            this.Receiver = receiver;
            this.Id = id;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Identifier " + this.Id + " at " + this.Location;
    }
}
