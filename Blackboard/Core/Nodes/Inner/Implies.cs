using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a boolean implies of two boolean parents.</summary>
    /// <see cref="https://mathworld.wolfram.com/Implies.html"/>
    sealed public class Implies: Binary<Bool, Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<Bool>, IValueAdopter<Bool>, Implies>((left, right) => new Implies(left, right));

        /// <summary>Creates an implied value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Implies(IValueAdopter<Bool> source1 = null, IValueAdopter<Bool> source2 = null, Bool value = default) :
            base(source1, source2, value) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Implies";

        /// <summary>Determines the boolean implies value of the two parents.</summary>
        /// <param name="value1">The first parent being implied</param>
        /// <param name="value2">The second parent implied.</param>
        /// <returns>The boolean implies value of the two given parents.</returns>
        protected override Bool OnEval(Bool value1, Bool value2) => new(!value1.Value || value2.Value);
    }
}
