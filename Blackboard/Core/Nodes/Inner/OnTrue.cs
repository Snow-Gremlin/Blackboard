using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a trigger when the parent becomes true.</summary>
    sealed public class OnTrue: TriggerNode, IChild {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<Bool>, OnTrue>((input) => new OnTrue(input));

        /// <summary>This is the parent node to read from.</summary>
        private IValueParent<Bool> source;

        /// <summary>Creates a trigger node which triggers when the boolean goes from false to true.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public OnTrue(IValueParent<Bool> source = null) {
            this.Parent = source;
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "OnTrue";

        /// <summary>The parent node to get the source value from.</summary>
        public IValueParent<Bool> Parent {
            get => this.source;
            set => IChild.SetParent(this, ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.source);

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            IChild.ReplaceParent(this, ref this.source, oldParent, newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents) {
            IChild.CheckParentsBeingSet(newParents, false, typeof(IValueParent<Bool>));
            return IChild.SetParent(this, ref this.source, newParents[0] as IValueParent<Bool>);
        }

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool ShouldProvoke() => this.source.Value.Value;
    }
}
