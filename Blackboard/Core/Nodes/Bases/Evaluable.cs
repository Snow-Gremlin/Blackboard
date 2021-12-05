using Blackboard.Core.Debug;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>
    /// This is the base node for all nodes within the main part of the blackboard graph
    /// which propagates updates when a change occurs.
    /// </summary>
    public abstract class Evaluable: IParent {

        /// <summary>The collection of children nodes to this node.</summary>
        private List<IChild> children;

        /// <summary>Creates a new node.</summary>
        protected Evaluable() {
            this.children = new List<IChild>();
            this.Depth = 0;
        }

        /// <summary>This is the type name of the node without any type parameters.</summary>
        public abstract string TypeName { get; }

        /// <summary>The depth in the graph from the furthest input of this node.</summary>
        public int Depth { get; private set; }

        /// <summary>Updates the node's value, provoked state, and any other state.</summary>
        /// <returns>
        /// True indicates that the value has changed or a trigger has been provoked, false otherwise.
        /// When the value has changed all the children are returned from the evaluation,
        /// otherwise no children are returned.
        /// </returns>
        public abstract bool Evaluate();

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<IChild> Children => this.children;

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>
        /// The parent will only keep a single copy of any child
        /// even if the child uses the same parent for multiple inputs.
        /// </remarks>
        /// <param name="children">The children to add.</param>
        public void AddChildren(IEnumerable<IChild> children) {
            if (this.CanReachAny(children.OfType<IParent>()))
                throw new Exception("May not add children to a parent which would cause a loop").
                    With("parent", this).
                    With("children", children);

            LinkedList<Evaluable> needsDepthUpdate = new();
            foreach (IChild child in children.NotNull()) {
                if (!this.children.Contains(child)) {
                    this.children.Add(child);
                    if (child is Evaluable eval)
                        needsDepthUpdate.SortInsertUnique(eval);
                }
            }
            updateDepths(needsDepthUpdate);
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<IChild> children) {
            LinkedList<Evaluable> needsDepthUpdate = new();
            foreach (IChild child in children.NotNull()) {
                if (this.children.Remove(child)) {
                    if (child is Evaluable eval)
                        needsDepthUpdate.SortInsertUnique(eval);
                }
            }
            updateDepths(needsDepthUpdate);
        }

        /// <summary>This updates the depth values of the given pending nodes.</summary>
        /// <remarks>
        /// The pending list will be emptied by this call. The pending nodes are expected to be
        /// presorted by depth which will usually provide the fastest update.
        /// </remarks>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        static private void updateDepths(LinkedList<Evaluable> pending) {
            while (pending.Count > 0) {
                Evaluable node = pending.TakeFirst();

                // Determine the depth that this node should be at based on its parents.
                int depth = node is not IChild child ? 0 :
                    child.Parents.OfType<Evaluable>().MaxDepth() + 1;

                // If the depth has changed then its children also need to be updated.
                if (node.Depth != depth) {
                    node.Depth = depth;
                    pending.SortInsertUnique(node.Children.OfType<Evaluable>());
                }
            }
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
