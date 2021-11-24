using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a trigger node which has several parents.</summary>
    public abstract class Multitrigger: TriggerNode {

        /// <summary>This is the list of all the parent nodes to listen to.</summary>
        protected List<ITriggerAdopter> Sources;

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Multitrigger(params ITriggerAdopter[] parents) :
            this(parents as IEnumerable<ITriggerAdopter>) { }

        /// <summary>Creates a multi-trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Multitrigger(IEnumerable<ITriggerAdopter> parents = null) {
            this.Sources = new List<ITriggerAdopter>();
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
            this.Sources.AddRange(parents);
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
                if (this.Sources.Remove(parent)) {
                    parent.RemoveChildren(this);
                    anyRemoved = true;
                }
            }
            return anyRemoved;
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<IAdopter> Parents => this.Sources;

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
            this.Provoked = this.OnEval(this.Sources.NotNull().Triggers());
    }
}
