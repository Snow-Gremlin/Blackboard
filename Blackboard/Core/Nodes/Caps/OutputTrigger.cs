using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a trigger which can be provoked from user output.</summary>
    sealed public class OutputTrigger: TriggerNode, ITriggerOutput {

        /// <summary>The parent source to listen to.</summary>
        private ITrigger source;

        /// <summary>Creates a new output trigger.</summary>
        /// <param name="source">The initial source trigger to listen to.</param>
        public OutputTrigger(ITrigger source = null) {
            this.Parent = source;
        }

        /// <summary>The parent trigger node to listen to.</summary>
        public ITrigger Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        public event S.EventHandler OnProvoked;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool UpdateTrigger() {
            if (this.source.Provoked) {
                this.OnProvoked?.Invoke(this, S.EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Output<trigger>";
    }
}
