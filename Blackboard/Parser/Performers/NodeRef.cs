using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>
    /// This is a reference to a node which wasn't created by the time
    /// this performer was created. Before this is performed, the creation
    /// of the referenced node must have been performed first.
    /// </summary>
    sealed internal class NodeRef: IPerformer {

        /// <summary>The reference to the real or virtual node.</summary>
        public VirtualNode Node;

        /// <summary>Indicates the result should be turned into a constant.</summary>
        public bool Evaluate;

        /// <summary>Create a new virtual node reference.</summary>
        /// <param name="location">The location the node was referenced.</param>
        /// <param name="node">The reference to the real or virtual node.</param>
        /// <param name="evaluate">Indicates if the node should be convereted to constant.</param>
        public NodeRef(Location location, VirtualNode node, bool evaluate = false) {
            this.Location = location;
            this.Node = node;
            this.Evaluate = evaluate;
        }

        /// <summary>The location this node is referenced.</summary>
        public Location Location { get; private set; }

        /// <summary>The type of the node.</summary>
        /// <returns>The real or virtual node type.</returns>
        public System.Type Returns() => this.Node.Type;

        /// <summary>This will resolve to the real node.</summary>
        /// <param name="formula">Not used.</param>
        /// <returns>This is the virtual node or a constant of it.</returns>
        public INode Perform(Formula formula) {
            INode node = this.Node.Node;
            if (this.Evaluate) node = (node as IDataNode)?.ToConstant();
            return node is not null ? node :
                // This exception should never be hit if the preper is working as expected.
                throw new Exception("The virtual node was unable to be resolved.").
                    With("Name", this.Node.Name).
                    With("Evaluate", this.Evaluate).
                    With("Type", this.Node.Type).
                    With("Location", this.Location);
        }
    }
}
