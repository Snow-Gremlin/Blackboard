using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Caps {

    /// <summary>Implicit casts a value node into another value node.</summary>
    sealed public class Implicit<T1, T2>: Unary<T1, T2>
        where T1 : IData
        where T2 : IImplicit<T1, T2>, IComparable<T2>, new() {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFunction Factory = new Func<IValue<T1>>((value) => new Implicit<T1, T2>(value));

        /// <summary>Creates a node implicit cast.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Implicit(IValue<T1> source = null, T2 value = default) :
            base(source, value) { }

        /// <summary>Gets the value to cast from the parent during evaluation.</summary>
        /// <param name="value">The parent value to cast.</param>
        /// <returns>The cast of the parent value.</returns>
        protected override T2 OnEval(T1 value) => this.Value.CastFrom(value);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Implicit"+base.ToString();
    }
}
