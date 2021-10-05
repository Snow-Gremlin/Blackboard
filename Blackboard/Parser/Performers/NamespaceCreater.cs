using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>Creates a performer to create a namespace.</summary>
    sealed internal class NamespaceCreater: IPerformer {

        /// <summary>The virtual node for this namespace.</summary>
        public VirtualNode VirtualNode;

        /// <summary>Creates a new namespace performer.</summary>
        /// <param name="loc">The location the namespace is created in code.</param>
        /// <param name="node">The virtual node.</param>
        public NamespaceCreater(Location loc, VirtualNode node) {
            this.Location = loc;
            this.VirtualNode = node;
        }

        /// <summary>The location the thing being performed was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <remarks>This may not be set until after Prepare is called.</remarks>
        /// <returns>The returned value type.</returns>
        public System.Type Returns() => this.VirtualNode.Type;

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform(Formula formula) {
            string name = this.VirtualNode.Name;

            INode receiver = this.VirtualNode.Receiver.Node;
            if (receiver is null)
                throw new Exception("Receiver for new namespace was not created first.").
                    With("Name", name).
                    With("Location", this.Location).
                    With("Receiver", this.VirtualNode.Receiver);

            if (receiver is not IFieldWriter writer)
                throw new Exception("Receiver for new namespace was not an IFieldWriter.").
                    With("Name", name).
                    With("Location", this.Location).
                    With("Receiver", this.VirtualNode.Receiver);

            Namespace space = new();
            this.VirtualNode.Node = space;
            writer.WriteField(name, space);

            // TODO: Rework based on changes to Virtual and real node.
            return space;
        }
    }
}
