using Blackboard.Core;
using PetiteParser.Scanner;

namespace Blackboard.Parser.StackItems {

    /// <summary>The stack item for storing a type.</summary>
    sealed internal class TypeItem: StackItem {

        /// <summary>The type being stored.</summary>
        public readonly Type Type;

        /// <summary>Creates a stack item which contains a type.</summary>
        /// <param name="loc">The location of this type.</param>
        /// <param name="t">The type being stored.</param>
        public TypeItem(Location loc, Type t): base(loc) {
            this.Type = t;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Type " + this.Type + " at " + this.Location;
    }
}
