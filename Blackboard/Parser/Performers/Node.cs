using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>An performer which contains an existing node.</summary>
    sealed internal class Node: IPerformer {

        /// <summary>Creates a new node performer.</summary>
        /// <param name="location">The nearest location in the code that requested this node.</param>
        /// <param name="node">The existing node to store.</param>
        public Node(Location location, INode node) {
            this.Location = location;
            this.Existing = node;
        }

        /// <summary>The existing node being stored.</summary>
        public INode Existing;

        /// <summary>The location this node was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public System.Type Returns() => this.Existing.GetType();

        /// <summary>This will build or evaluate the actor and apply it to Blackboard.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform(Formula formula) => this.Existing;
    }
}
