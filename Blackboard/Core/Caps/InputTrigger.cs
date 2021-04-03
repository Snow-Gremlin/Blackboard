using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This is a trigger which can be triggered from user input.</summary>
    public class InputTrigger: TriggerNode, ITriggerInput {

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
            set => Namespace.SetName(this, value);
        }

        /// <summary>Gets or sets the containing scope for this name or null.</summary>
        public INamespace Scope {
            get => this.scope;
            set {
                if (value?.Exists(this.name) ?? false)
                    throw Exception.RenameDuplicateInScope(this.name, value);
                this.scope?.RemoveChildren(this);
                this.scope = value;
                this.scope?.AddChildren(this);
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.scope is null)) yield return this.scope;
            }
        }

        /// <summary>This is set this trigger to emit during the next evaluation.</summary>
        public void Trigger() => this.Triggered = true;

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the triggered value as it currently is.</returns>
        protected override bool UpdateTrigger() => this.Triggered;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            (this.scope is null ? "" : this.Scope.ToString()+".")+this.Name;
    }
}
