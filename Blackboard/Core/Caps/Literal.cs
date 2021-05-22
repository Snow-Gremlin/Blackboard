using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Caps {

    /// <summary>This is a literal value.</summary>
    /// <typeparam name="T">The type of this literal.</typeparam>
    sealed public class Literal<T>: ValueNode<T> {

        /// <summary>Creates a new literal value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public Literal(T value = default) :
            base(value) { }

        /// <summary>This sets the literal value.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>Always returns no parents since literals have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>Updates thie value during evaluation.</summary>
        /// <remarks>Since the value is set outside this does nothing.</remarks>
        /// <returns>This always returns true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.Value.ToString();
    }
}
