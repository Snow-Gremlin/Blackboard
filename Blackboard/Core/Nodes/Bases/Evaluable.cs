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
    public abstract class Evaluable: IEvaluable {

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
        public int Depth { get; set; }

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
        /// <returns>True if any children were added, false otherwise.</returns>
        public bool AddChildren(IEnumerable<IChild> children) {
            if (this.CanReachAny(children.OfType<IParent>()))
                throw new Exception("May not add children to a parent which would cause a loop").
                    With("parent", this).
                    With("children", children);

            bool anyAdded = false;
            foreach (IChild child in children.NotNull()) {
                if (!this.children.Contains(child)) {
                    this.children.Add(child);
                    anyAdded = true;
                }
            }
            return anyAdded;
        }

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        /// <returns>True if any children were removed, false otherwise.</returns>
        public bool RemoveChildren(IEnumerable<IChild> children) {
            bool anyRemoved = false;
            foreach (IChild child in children.NotNull()) {
                if (this.children.Remove(child)) anyRemoved = true;
            }
            return anyRemoved;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
