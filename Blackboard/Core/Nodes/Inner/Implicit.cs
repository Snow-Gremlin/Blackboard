using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Implicit casts a value node into another value node.</summary>
    sealed public class Implicit<T1, T2>: UnaryValue<T1, T2>
        where T1 : struct, IData
        where T2 : struct, IImplicit<T1, T2>, IEquatable<T2> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((value) => new Implicit<T1, T2>(value));

        /// <summary>Creates a node implicit cast.</summary>
        public Implicit() { }

        /// <summary>Creates a node implicit cast.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public Implicit(IValueParent<T1>? source = null) : base(source) { }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Implicit<T1, T2>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Implicit<T1, T2>);

        /// <summary>Gets the value to cast from the parent during evaluation.</summary>
        /// <param name="value">The parent value to cast.</param>
        /// <returns>The cast of the parent value.</returns>
        protected override T2 OnEval(T1 value) => this.Value.CastFrom(value);
    }
}
