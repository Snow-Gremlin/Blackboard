using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>A node for user outputted values.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    public class OutputValue<T>: ValueNode<T>, IValueOutput<T> {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>The parent source to listen to.</summary>
        private IValue<T> source;

        /// <summary>Creates a new output value node.</summary>
        /// <param name="source">The initial source to get the value from.</param>
        /// <param name="name">The initial name for this value node.</param>
        /// <param name="scope">The initial scope for this value node.</param>
        /// <param name="value">The initial value for this node.</param>
        public OutputValue(IValue<T> source = null, string name = "Input",
            INamespace scope = null, T value = default) : base(value) {
            this.Parent = source;
            this.Name = name;
            this.Scope = scope;
            this.UpdateValue();
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
                Namespace.CheckScopeChange(this, value);
                this.scope = this.SetParent(this.scope, value);
            }
        }

        /// <summary>The parent node to get the value from.</summary>
        public IValue<T> Parent {
            get => this.source;
            set {
                this.source = this.SetParent(this.source, value);
                this.UpdateValue();
            }
        }

        /// <summary>This event is emitted when the value is changed.</summary>
        public event EventHandler OnChanged;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.scope is null)) yield return this.scope;
                if (!(this.source is null)) yield return this.source;
            }
        }

        /// <summary>This will update the value.</summary>
        /// <returns>This will always return true.</returns>
        protected override bool UpdateValue() {
            if (this.source is null) return false;
            if (this.SetNodeValue(this.source.Value)) {
                if (!(this.OnChanged is null))
                    this.OnChanged(this, EventArgs.Empty);
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
