using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>
    /// This is an action to define a named node in a field writer.
    /// Typically this is for defining a new node into the namespaces reachable from global.
    /// </summary>
    public class Define: IAction {

        /// <summary>This is the receiver that will be written to.</summary>
        private readonly IFieldWriter receiver;

        /// <summary>The name to write the node with.</summary>
        private readonly string name;

        /// <summary>The node being set to the receiver with the given name.</summary>
        private readonly INode node;

        /// <summary>
        /// This is a subset of all the node for this node to write which need to be
        /// added to parents their parents to make this node reactive to changes.
        /// </summary>
        private readonly IChild[] needParents;

        /// <summary>Creates a new define action.</summary>
        /// <param name="receiver">This is the receiver that will be written to.</param>
        /// <param name="name">The name to write the node with.</param>
        /// <param name="node">The node being set to the receiver with the given name.</param>
        /// <param name="allNodes">All the nodes which are new children of the node to write.</param>
        public Define(IFieldWriter receiver, string name, INode node, params INode[] allNodes) :
            this(receiver, name, node, allNodes as IEnumerable<INode>) { }

        /// <summary>Creates a new define action.</summary>
        /// <param name="receiver">This is the receiver that will be written to.</param>
        /// <param name="name">The name to write the node with.</param>
        /// <param name="node">The node being set to the receiver with the given name.</param>
        /// <param name="allNodes">All the nodes which are new children of the node to write.</param>
        public Define(IFieldWriter receiver, string name, INode node, IEnumerable<INode> allNodes) {
            this.receiver = receiver;
            this.name = name;
            this.node = node;
            this.needParents = allNodes.NotNull().OfType<IChild>().Where(child => child.NeedsToAddParents()).ToArray();
        }

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        public void Perform(Driver driver) {
            this.receiver.WriteField(this.name, this.node);
            this.needParents.Foreach(child => child.AddToParents());
        }
    }
}
