using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a trigger which can be provoked from user output.</summary>
    sealed public class OutputTrigger: TriggerNode, ITriggerOutput {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>The parent source to listen to.</summary>
        private ITrigger source;

        /// <summary>Creates a new output trigger.</summary>
        /// <param name="source">The initial source trigger to listen to.</param>
        /// <param name="name">The initial name for this trigger.</param>
        /// <param name="scope">The initial scope for this trigger.</param>
        public OutputTrigger(ITrigger source = null, string name = "Output", INamespace scope = null) {
            this.Parent = source;
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

        /// <summary>The parent trigger node to listen to.</summary>
        public ITrigger Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        public event EventHandler OnProvoked;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.scope is not null) yield return this.scope;
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool UpdateTrigger() {
            if (this.source.Provoked) {
                this.OnProvoked?.Invoke(this, EventArgs.Empty);
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
