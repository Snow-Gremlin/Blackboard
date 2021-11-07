using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is the base node for all data in the blackboard graph.</summary>
    public abstract class EvalAdopter: INode, IEvaluatable, IAdopter {

        /// <summary>This updates the depth values of the given pending nodes.</summary>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        static public void UpdateDepths(LinkedList<IEvaluatable> pending) {
            while (pending.Count > 0) {
                IEvaluatable node = pending.TakeFirst();
                int depth = node.Parents.OfType<IEvaluatable>().MaxDepth() + 1;
                if ((node.Depth != depth) && (node is EvalAdopter)) {
                    (node as EvalAdopter).Depth = depth;
                    pending.SortInsertUniqueEvaluatable(node.Children.OfType<IEvaluatable>());
                }
            }
        }

        /// <summary>The collection of children nodes to this node.</summary>
        private List<INode> children;

        /// <summary>Creates a new node.</summary>
        protected EvalAdopter() {
            this.children = new List<INode>();
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

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public abstract IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        public IEnumerable<INode> Children => this.children;

        /// <summary>This is a helper method for setting a parent to the node.</summary>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="parent">The parent variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        protected void SetParent<T>(ref T parent, T newParent) where T : IAdopter {
            if (ReferenceEquals(parent, newParent)) return;
            parent?.RemoveChildren(this);
            parent = newParent;
            newParent?.AddChildren(this);
        }

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>This will always check for loops.</remarks>
        /// <param name="children">The children to add.</param>
        public void AddChildren(params INode[] children) =>
            this.AddChildren(children as IEnumerable<INode>);

        /// <summary>Adds children nodes onto this node.</summary>
        /// <param name="children">The children to add.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        public void AddChildren(IEnumerable<INode> children, bool checkedForLoops = true) {
            children = children.NotNull();
            if (checkedForLoops && this.CanReachAny(children))
                throw Exceptions.NodeLoopDetected();
            LinkedList<IEvaluatable> needsDepthUpdate = new();
            foreach (EvalAdopter child in children) {
                if (!this.children.Contains(child)) {
                    this.children.Add(child);
                    needsDepthUpdate.SortInsertUniqueEvaluatable(child);
                }
            }
            UpdateDepths(needsDepthUpdate);
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(params INode[] children) =>
            this.RemoveChildren(children as IEnumerable<INode>);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        public void RemoveChildren(IEnumerable<INode> children) {
            children = children.NotNull();
            LinkedList<IEvaluatable> needsDepthUpdate = new();
            foreach (EvalAdopter child in children) {
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
