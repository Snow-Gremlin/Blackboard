using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a trigger when any parent is changed.</summary>
    sealed public class OnChange: TriggerNode, IChild {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new FunctionN<IParent, OnChange>((inputs) => new OnChange(inputs), passOne: false);

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private List<IParent> sources;

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(params IParent[] parents) :
            this(parents as IEnumerable<IParent>) { }

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(IEnumerable<IParent> parents = null) {
            this.sources = new List<IParent>();
            this.AddParents(parents);
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "OnChange";

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<IParent> parents) =>
            this.sources.AddParents(parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<IParent> parents) =>
            this.sources.RemoveParents(this, parents);

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => this.sources;

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            this.sources.ReplaceParents(this, oldParent, newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents) =>
            this.sources.SetAllParents(this, newParents);

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool ShouldProvoke() => true;
    }
}
