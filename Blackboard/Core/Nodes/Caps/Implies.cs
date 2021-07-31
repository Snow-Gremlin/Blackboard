using Blackboard.Core.Data.Caps;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Performs a boolean implies of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/Implies.html"/>
    sealed public class Implies: Binary<Bool, Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory =
            new Function<IValue<Bool>, IValue<Bool>>((left, right) => new Implies(left, right));

        /// <summary>Creates an implied value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Implies(IValue<Bool> source1 = null, IValue<Bool> source2 = null, Bool value = default) :
            base(source1, source2, value) { }

        /// <summary>Determines the boolean implies value of the two parents.</summary>
        /// <param name="value1">The first parent being implied</param>
        /// <param name="value2">The second parent implied.</param>
        /// <returns>The boolean implies value of the two given parents.</returns>
        protected override Bool OnEval(Bool value1, Bool value2) => new(!value1.Value || value2.Value);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Implies"+base.ToString();
    }
}
