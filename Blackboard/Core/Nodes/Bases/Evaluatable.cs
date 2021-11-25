using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is the base node for all data in the blackboard graph.</summary>
    public abstract class Evaluatable: IEvaluatable, IParent {

        /// <summary>The collection of children nodes to this node.</summary>
        private List<IChild> children;

        /// <summary>Creates a new node.</summary>
        protected Evaluatable() {
            this.children = new List<IChild>();
            this.Depth = 0;
        }

        /// <summary>This is the type name of the node without any type parameters.</summary>
        public abstract string TypeName { get; }

        /// <summary>The depth in the graph from the furthest input of this node.</summary>
        public int Depth { get; private set; }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of children that should be updated based on the results of this update.
        /// If this evaluation made no change then typically no children will be returned.
        /// Usually the entire set of children are returned on change, but it is not required.
        /// </returns>
        abstract public IEnumerable<IEvaluatable> Eval();

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<IChild> Children => this.children;

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        public void AddChildren(IEnumerable<IChild> children) {
            LinkedList<Evaluatable> needsDepthUpdate = new();
            foreach (IChild child in children.NotNull()) {
                if (!this.children.Contains(child)) {
                    this.children.Add(child);
                    if (child is Evaluatable eval)
                        needsDepthUpdate.SortInsertUnique(eval);
                }
            }
            updateDepths(needsDepthUpdate);
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<IChild> children) {
            LinkedList<Evaluatable> needsDepthUpdate = new();
            foreach (IChild child in children.NotNull()) {
                int index = this.children.IndexOf(child);
                if (index >= 0) {
                    this.children.RemoveAt(index);
                    if (child is Evaluatable eval)
                        needsDepthUpdate.SortInsertUnique(eval);
                }
            }
            updateDepths(needsDepthUpdate);
        }

        /// <summary>This updates the depth values of the given pending nodes.</summary>
        /// <remarks>
        /// The pending list will be emptied by this call. The pending nodes are expected to be
        /// pre-sorted by depth which will usually provide the fastest update.
        /// </remarks>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        static private void updateDepths(LinkedList<Evaluatable> pending) {
            while (pending.Count > 0) {
                Evaluatable node = pending.TakeFirst();

                // Determine the depth that this node should be at based on its parents.
                int depth = node is not IChild child ? 0 :
                    child.Parents.OfType<IEvaluatable>().MaxDepth() + 1;

                // If the depth has changed then its children also need to be updated.
                if (node.Depth != depth) {
                    node.Depth = depth;
                    pending.SortInsertUnique(node.Children.OfType<Evaluatable>());
                }
            }
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
