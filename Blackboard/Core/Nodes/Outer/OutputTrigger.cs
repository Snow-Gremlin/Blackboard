using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a trigger which can be provoked from user output.</summary>
    sealed public class OutputTrigger: TriggerNode, ITriggerOutput, IChild {

        /// <summary>The parent source to listen to.</summary>
        private ITriggerParent source;

        /// <summary>Creates a new output trigger.</summary>
        /// <param name="source">The initial source trigger to listen to.</param>
        public OutputTrigger(ITriggerParent source = null) => this.Parent = source;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Output";

        /// <summary>The parent trigger node to listen to.</summary>
        public ITriggerParent Parent {
            get => this.source;
            set => IChild.SetParent(this, ref this.source, value);
        }

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        public event S.EventHandler OnProvoked;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.source);

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            IChild.ReplaceParent(this, ref this.source, oldParent, newParent);

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool ShouldProvoke() =>
            this.source is not null && this.source.Provoked;

        /// <summary>Updates the node's provoked state.</summary>
        /// <remarks>Here we want to return if provoked and NOT if the provoke state has changed.</remarks>
        /// <returns>True indicates that the value has been provoked, false otherwise.</returns>
        public override bool Evaluate() {
            if (!base.Evaluate()) return false;
            this.OnProvoked?.Invoke(this, S.EventArgs.Empty);
            return true;
        }
    }
}
