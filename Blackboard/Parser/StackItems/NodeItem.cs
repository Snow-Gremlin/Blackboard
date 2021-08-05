using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.StackItems {

    /// <summary>The stack item for storing a node.</summary>
    sealed public class NodeItem: StackItem {

        /// <summary>The node being stored.</summary>
        public readonly INode Node;

        /// <summary>Creates a stack item which contains a node.</summary>
        /// <param name="loc">The location near the right edge for the parts which created this node.</param>
        /// <param name="node">The node being stored.</param>
        public NodeItem(Location loc, INode node): base(loc) {
            this.Node = node;
        }

        /// <summary>The string for this stack item.</summary>
        /// <returns>The stack item's string.</returns>
        public override string ToString() =>
            "Node " + this.Node + " at " + this.Location;
    }
}
