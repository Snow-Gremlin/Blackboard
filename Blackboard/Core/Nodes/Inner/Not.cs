using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a boolean NOT of one boolean parent.</summary>
    /// <see cref="https://mathworld.wolfram.com/NOT.html"/>
    sealed public class Not: UnaryValue<Bool, Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory(input => new Not(input));

        /// <summary>Creates a boolean NOT value node.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Not(IValueParent<Bool> source = null) : base(source) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Not";

        /// <summary>Gets the boolean NOT of the given parent during evaluation.</summary>
        /// <param name="value">The parent value to get the NOT of.</param>
        /// <returns>The NOT of the given parent value.</returns>
        protected override Bool OnEval(Bool value) => new(!value.Value);
    }
}
