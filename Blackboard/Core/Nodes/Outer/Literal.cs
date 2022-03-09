using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

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

        /// <summary>Tries to create a new literal for the given data.</summary>
        /// <param name="value">The value to use for the literal.</param>
        /// <returns>The new literal node for the given data.</returns>
        static public IConstant Data(IData value) =>
            value switch {
                Bool   b => new Literal<Bool>  (b),
                Int    i => new Literal<Int>   (i),
                Double d => new Literal<Double>(d),
                String s => new Literal<String>(s),
                _        => throw new Message("Unexpected value type in literal creation").
                               With("Value", value)
            };

        /// <summary>This creates a new literal with the given typed data.</summary>
        /// <typeparam name="T">The type of data to get the literal from.</typeparam>
        /// <param name="value">The data value to set to the literal.</param>
        /// <returns>The new literal for the given data.</returns>
        static public Literal<T> Data<T>(T value) where T : IEquatable<T> => new(value);
    }

    /// <summary>This is a literal value.</summary>
    /// <typeparam name="T">The type of this literal.</typeparam>
    sealed public class Literal<T>: ValueNode<T>, IConstant
        where T : IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = new Function<Literal<T>>(() => new Literal<T>());

        /// <summary>Creates a new literal value node.</summary>
        public Literal() { }

        /// <summary>Creates a new literal value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public Literal(T value = default) : base(value) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Literal<T>();

        /// <summary>This sets the literal value.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.UpdateValue(value);

        /// <summary>This is called when the value is evaluated and updated.</summary>
        /// <returns>The new value that the node should be set to.</returns>
        protected override T CalcuateValue() => this.Value;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Literal<T>);
    }
}
