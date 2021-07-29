using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>This is a literal value.</summary>
    /// <typeparam name="T">The type of this literal.</typeparam>
    sealed public class Literal<T>: ValueNode<T>, IConstant
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new Func(() => new Literal<T>());

        /// <summary>Creates a new literal value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public Literal(T value = default) :
            base(value) { }

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>This returns this literal itself.</returns>
        public override INode ToLiteral() => this;

        /// <summary>This sets the literal value.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>Always returns no parents since literals have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>Updates thie value during evaluation.</summary>
        /// <returns>This always returns true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.Value.ToString();
    }
}
