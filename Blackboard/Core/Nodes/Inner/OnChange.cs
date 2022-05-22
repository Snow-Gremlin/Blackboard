using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
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
        private readonly List<IParent> sources;

        /// <summary>Creates an on change trigger node.</summary>
        public OnChange() : this(null) { }

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

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new OnChange();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(OnChange);

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
        public IParentCollection Parents => new VarParents<IParent>(this, this.sources);

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool ShouldProvoke() => true;
    }
}
