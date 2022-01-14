using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Parser.Optimization {

    /// <summary>
    /// This rule will remove a node if that node is among the added node types and
    /// it has only one non-nil parent. The node will be replaced by the parent itself.
    /// </summary>
    public class NoEffectDegenerate: IRule {
        private List<S.Type> types;

        /// <summary>Creates a new rule to remove nodes with single parents which have no effect.</summary>
        public NoEffectDegenerate() => this.types = new List<S.Type>();

        /// <summary>Adds a new type to check for degenerates.</summary>
        /// <typeparam name="T">The type of node to add.</typeparam>
        /// <returns>The same rule so these calls can be chained.</returns>
        public NoEffectDegenerate Add<T>()
            where T : INode {
            this.types.Add(typeof(T));
            return this;
        }

        /// <summary>Recursively finds constant branches and replaces the branch with literals.</summary>
        /// <param name="slate">The slate the formula is for.</param>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The formula nodes to optimize.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
        public INode Perform(Slate slate, INode root, HashSet<INode> nodes, ILogger logger = null) {
            foreach (INode node in nodes) {

                // Check if node has one and only one parent.
                if (node is IChild child && child.Parents.Count() == 1) {

                    // Check if the node is one of the types which have been added.
                    S.Type nodeType = node.GetType();
                    if (this.types.Any(t => nodeType.IsAssignableTo(t))) {
                        IParent parent = child.Parents.First();
                        if (parent is null) continue;

                        // Remove child from parent, then replace child with parent in the child' children.
                        parent.RemoveChildren(child);
                        if (child is IParent par)
                            par.Children.Foreach(grandchild => grandchild.ReplaceParent(par, parent));

                        // If the child was the root, update the root.
                        if (ReferenceEquals(child, root)) root = parent;
                    }
                }
            }
            return root;
        }
    }
}
