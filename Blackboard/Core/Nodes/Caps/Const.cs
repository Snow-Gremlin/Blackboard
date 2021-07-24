using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a constant value.</summary>
    /// <typeparam name="T">The type of this constant.</typeparam>
    sealed public class Const<T>: ValueNode<T>, IConstant, INamed
        where T : IComparable<T>, new() {

        /// <summary>The name for this namespace.</summary>
        private string name;

        /// <summary>The parent scope or null.</summary>
        private INamespace scope;

        /// <summary>Creates a new constant value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public Const(string name = "Const", INamespace scope = null, T value = default) :
            base(value) {
            this.Name = name;
            this.Scope = scope;
            this.UpdateValue();
        }

        /// <summary>This sets the constant value.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>Always returns no parents since constants have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

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

        /// <summary>Updates thie value during evaluation.</summary>
        /// <remarks>Since the value is set outside this does nothing.</remarks>
        /// <returns>This always returns true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.Value.ToString();
    }
}
