using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a trigger node which has several parents.</summary>
    public abstract class Multitrigger: TriggerNode {

        /// <summary>This is the list of all the parent nodes to listen to.</summary>
        protected List<ITriggerAdopter> sources;

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Multitrigger(params ITriggerAdopter[] parents) :
            this(parents as IEnumerable<ITriggerAdopter>) { }

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Multitrigger(IEnumerable<ITriggerAdopter> parents = null) {
            this.sources = new List<ITriggerAdopter>();
            this.AddParents(parents);
        }

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(params ITriggerAdopter[] parents) =>
            this.AddParents(parents as IEnumerable<ITriggerAdopter>);

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<ITriggerAdopter> parents) {
            parents = parents.NotNull();
            this.sources.AddRange(parents);
            foreach (ITriggerAdopter parent in parents)
                parent.AddChildren(this);
        }

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(params ITriggerAdopter[] parents) =>
            this.RemoveParents(parents as IEnumerable<ITriggerAdopter>);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<ITriggerAdopter> parents) {
            bool anyRemoved = false;
            foreach (ITriggerAdopter parent in parents) {
                if (this.sources.Remove(parent)) {
                    parent.RemoveChildren(this);
                    anyRemoved = true;
                }
            }
            return anyRemoved;
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => this.sources;

        /// <summary>
        /// This handles updating this node's value given the
        /// parents' provoked state during evaluation.
        /// </summary>
        /// <remarks>Any null parents are ignored.</remarks>
        /// <param name="provoked">The value from the all the non-null parents.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract bool OnEval(IEnumerable<bool> provoked);

        /// <summary>This updates the trigger during evaluation.</summary>
        /// <returns>True if the value was provoked, false otherwise.</returns>
        protected override bool UpdateTrigger() =>
            this.Provoked = this.OnEval(this.sources.NotNull().Triggers());

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="scopeName">The name of this node from a parent namespace or empty for no name.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(string scopeName = "", int nodeDepth = int.MaxValue) {
            string name = string.IsNullOrEmpty(scopeName) ? this.TypeName : scopeName;
            string tail = nodeDepth > 0 ?
                INode.NodePrettyString(this.sources, scopeName, nodeDepth-1) :
                this.Provoked.ToString();
            return name + "(" + tail + ")";
        }
    }
}
