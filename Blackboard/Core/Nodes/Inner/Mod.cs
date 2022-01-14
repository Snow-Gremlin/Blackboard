using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will get the modulo the first parent value by the second parent value.</summary>
    sealed public class Mod<T>: BinaryValue<T, T, T>
        where T : IDivisible<T>, IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((left, right) => new Mod<T>(left, right));

        /// <summary>Creates a modulo value node.</summary>
        /// <param name="left">This is the first parent for the source value.</param>
        /// <param name="right">This is the second parent for the source value.</param>
        public Mod(IValueParent<T> left = null, IValueParent<T> right = null) : base(left, right) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Mod";

        /// <summary>Gets the modulo of the two given values.</summary>
        /// <param name="left">The first value to modulo.</param>
        /// <param name="right">The second value to modulo.</param>
        /// <returns>The modulo of the two values.</returns>
        protected override T OnEval(T left, T right) => left.Mod(right);
    }
}
