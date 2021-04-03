using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>A node for user inputted values.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    public class InputValue<T>: ValueNode<T>, IValueInput<T> {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>Creates a new input value node.</summary>
        /// <param name="name">The initial name for this value node.</param>
        /// <param name="scope">The initial scope for this value node.</param>
        /// <param name="value">The initial value for this node.</param>
        public InputValue(string name = "Input", INamespace scope = null, T value = default): base(value) {
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

        /// <summary>This sets the value of this node.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>This will update the value.</summary>
        /// <remarks>Since the value is set by the user this will always return true.</remarks>
        /// <returns>This will always return true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            (this.scope is null ? "" : this.Scope.ToString()+".")+this.Name;
    }
}
