using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>
    /// This is an action to define a named node in a field writer.
    /// Typically this is for defining a new node into the namespaces reachable from global.
    /// </summary>
    public class Define: IAction {

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
        public Define(IFieldWriter receiver, string name, INode node, IEnumerable<INode> allNodes) {
            this.Receiver = receiver;
            this.Name = name;
            this.Node = node;
            this.needParents = allNodes.NotNull().OfType<IChild>().Where(child => child.NeedsToAddParents()).ToArray();
        }

        /// <summary>This is the receiver that will be written to.</summary>
        public readonly IFieldWriter Receiver;

        /// <summary>The name to write the node with.</summary>
        public readonly string Name;

        /// <summary>The node being set to the receiver with the given name.</summary>
        public readonly INode Node;

        /// <summary>All the nodes which are new children of the node to write.</summary>
        public IReadOnlyList<IChild> NeedParents => this.needParents;

        /// <summary>This will perform the action.</summary>
        /// <param name="driver">The driver for this action.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Driver driver, Logger logger = null) {
            this.Receiver.WriteField(this.Name, this.Node);
            driver.Pend(this.needParents.Where(child => child.AddToParents()));
        }

        /// <summary>Gets a human readable string for this define.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
