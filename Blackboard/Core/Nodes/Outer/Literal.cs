using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>These are factories for easily making literals.</summary>
    static public class Literal {

        /// <summary>Creates a new boolean literal.</summary>
        /// <param name="value">The value to use for the literal.</param>
        /// <returns>The new literal node.</returns>
        static public Literal<Bool> Bool(bool value) => new(new Bool(value));

        /// <summary>Creates a new integer literal.</summary>
        /// <param name="value">The value to use for the literal.</param>
        /// <returns>The new literal node.</returns>
        static public Literal<Int> Int(int value) => new(new Int(value));

        /// <summary>Creates a new double literal.</summary>
        /// <param name="value">The value to use for the literal.</param>
        /// <returns>The new literal node.</returns>
        static public Literal<Double> Double(double value) => new(new Double(value));

        /// <summary>Creates a new string literal.</summary>
        /// <param name="value">The value to use for the literal.</param>
        /// <returns>The new literal node.</returns>
        static public Literal<String> String(string value) => new(new String(value));
    }

    /// <summary>This is a literal value.</summary>
    /// <typeparam name="T">The type of this literal.</typeparam>
    sealed public class Literal<T>: ValueNode<T>, IConstant
        where T : IComparable<T>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = new Function<Literal<T>>(() => new Literal<T>());

        /// <summary>Creates a new literal value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public Literal(T value = default) :
            base(value) { }

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>This returns this literal itself.</returns>
        public override IConstant ToConstant() => this;

        /// <summary>This sets the literal value.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.SetNodeValue(value);

        /// <summary>Always returns no parents since literals have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>Updates thie value during evaluation.</summary>
        /// <returns>This always returns true.</returns>
        protected override bool UpdateValue() => true;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Literal";

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.TypeName + "<" + this.Value.TypeName + ">(" + this.Value.ValueString + ")";
    }
}
