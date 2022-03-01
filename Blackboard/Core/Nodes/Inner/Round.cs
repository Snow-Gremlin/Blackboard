using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This gets the rounded value from the parent at the given decimal point.</summary>
    sealed public class Round<T>: BinaryValue<T, Int, T>
        where T : IFloatingPoint<T>, IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((value, decimals) => new Round<T>(value, decimals));

        /// <summary>Creates a rounded value node.</summary>
        public Round() { }

        /// <summary>Creates a rounded value node.</summary>
        /// <param name="value">This is the value parent for the source value.</param>
        /// <param name="decimals">This is the decimals parent for the source value.</param>
        public Round(IValueParent<T> value = null, IValueParent<Int> decimals = null) : base(value, decimals) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Round<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Round<T>);

        /// <summary>Rounds the parent's value during evaluation.</summary>
        /// <param name="value">The value to round.</param>
        /// <param name="decimals">The number of decimals to round to.</param>
        /// <returns>The rounded value.</returns>
        protected override T OnEval(T value, Int decimals) => value.Round(decimals);
    }
}
