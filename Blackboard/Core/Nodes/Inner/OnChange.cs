using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a trigger when any parent is changed.</summary>
    sealed public class OnChange: TriggerNode {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IAdopter, OnChange>((inputs) => new OnChange(inputs), false);

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private List<IAdopter> sources;

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(params IAdopter[] parents) :
            this(parents as IEnumerable<IAdopter>) { }

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(IEnumerable<IAdopter> parents = null) {
            this.sources = new List<IAdopter>();
            this.AddParents(parents);
        }
 
        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(params IAdopter[] parents) =>
            this.AddParents(parents as IEnumerable<IAdopter>);

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<IAdopter> parents) {
            parents = parents.NotNull();
            this.sources.AddRange(parents);
            foreach (IAdopter parent in parents)
                parent.AddChildren(this);
        }

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(params IAdopter[] parents) =>
            this.RemoveParents(parents as IEnumerable<IAdopter>);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<IAdopter> parents) {
            bool anyRemoved = false;
            foreach (IAdopter parent in parents) {
                if (this.sources.Remove(parent)) {
                    parent.RemoveChildren(this);
                    anyRemoved = true;
                }
            }
            return anyRemoved;
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<IAdopter> Parents => this.sources;

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool UpdateTrigger() => true;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "OnChange";
    }
}
