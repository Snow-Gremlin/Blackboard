using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This is a trigger which can be triggered from user input.</summary>
    sealed public class InputTrigger: TriggerNode, ITriggerInput {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>Creates a new input trigger.</summary>
        /// <param name="name">The initial name for this trigger.</param>
        /// <param name="scope">The initial scope for this trigger.</param>
        public InputTrigger(string name = "Input", INamespace scope = null) {
            this.Name = name;
            this.Scope = scope;
        }

        /// <summary>Gets or sets the name for the node.</summary>
        public string Name {
            get => this.name;
            set => this.name = Namespace.SetName(this, value);
        }

        /// <summary>Gets or sets the containing scope for this name or null.</summary>
        public INamespace Scope {
            get => this.scope;
            set {
                Namespace.CheckScopeChange(this, value);
                this.SetParent(ref this.scope, value);
            }
        }

        /// <summary>This event is emitted when the trigger has been triggered.</summary>
        public event EventHandler OnTriggered;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.scope is not null) yield return this.scope;
            }
        }

        /// <summary>This is set this trigger to emit during the next evaluation.</summary>
        /// <param name="value">True will trigger, false will reset the trigger.</param>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        public void Trigger(bool value = true) => this.Triggered = value;

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the triggered value as it currently is.</returns>
        protected override bool UpdateTrigger() {
            if (this.Triggered) {
                this.OnTriggered?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            (this.scope is null ? "" : this.Scope.ToString()+".")+this.Name;
    }
}
