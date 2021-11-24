using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is the base node for all data in the blackboard graph.</summary>
    public abstract class Evaluatable: IEvaluatable, IParent {

        /// <summary>This updates the depth values of the given pending nodes.</summary>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        static public void UpdateDepths(LinkedList<IEvaluatable> pending) {
            while (pending.Count > 0) {
                IEvaluatable node = pending.TakeFirst();
                int depth = node.Parents.OfType<IEvaluatable>().MaxDepth() + 1;
                if ((node.Depth != depth) && (node is Evaluatable)) {
                    (node as Evaluatable).Depth = depth;
                    pending.SortInsertUniqueEvaluatable(node.Children.OfType<IEvaluatable>());
                }
            }
        }

        /// <summary>This is a helper method for setting a parent to the node.</summary>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">Is the child node setting the parent.</param>
        /// <param name="parent">The parent variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        static protected void SetParent<T>(IChild child, ref T parent, T newParent) where T : IParent {
            if (ReferenceEquals(parent, newParent)) return;
            parent?.RemoveChildren(child);
            parent = newParent;
            // Do not add parent yet, so we can read from the parents when only evaluating.
        }

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
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        public void AddChildren(IEnumerable<IChild> children, bool checkedForLoops = true) {
            children = children.NotNull();
            if (checkedForLoops && this.CanReachAny(children))
                throw Exceptions.NodeLoopDetected();

            LinkedList<IEvaluatable> needsDepthUpdate = new();
            foreach (IChild child in children) {
                if (!this.children.Contains(child)) {
                    this.children.Add(child);
                    needsDepthUpdate.SortInsertUniqueEvaluatable(child);
                }
            }
            UpdateDepths(needsDepthUpdate);
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<IChild> children) {
            LinkedList<IEvaluatable> needsDepthUpdate = new();
            foreach (IChild child in children.NotNull()) {
                int index = this.children.IndexOf(child);
                if (index >= 0) {
                    this.children.RemoveAt(index);
                    needsDepthUpdate.SortInsertUniqueEvaluatable(child);
                }
            }
            UpdateDepths(needsDepthUpdate);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
