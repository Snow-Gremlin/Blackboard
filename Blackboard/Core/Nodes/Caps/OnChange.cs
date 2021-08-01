using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a trigger when any parent is changed.</summary>
    sealed public class OnChange: TriggerNode {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new FunctionN<INode>((inputs) => new OnChange(inputs), false);

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private List<INode> sources;

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(params INode[] parents) :
            this(parents as IEnumerable<INode>) { }

        /// <summary>Creates an on change trigger node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public OnChange(IEnumerable<INode> parents = null) {
            this.sources = new List<INode>();
            this.AddParents(parents);
        }
 
        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(params INode[] parents) =>
            this.AddParents(parents as IEnumerable<INode>);

        /// <summary>This adds parents to this node.</summary>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<INode> parents) {
            this.sources.AddRange(parents);
            foreach (INode parent in parents)
                parent.AddChildren(this);
        }

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(params INode[] parents) =>
            this.RemoveParents(parents as IEnumerable<INode>);

        /// <summary>This removes the given parents from this node.</summary>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<INode> parents) {
            bool anyRemoved = false;
            foreach (INode parent in parents) {
                if (this.sources.Remove(parent)) {
                    parent.RemoveChildren(this);
                    anyRemoved = true;
                }
            }
            return anyRemoved;
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => this.sources;

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool UpdateTrigger() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "OnChange("+NodeString(this.sources)+")";
    }
}
