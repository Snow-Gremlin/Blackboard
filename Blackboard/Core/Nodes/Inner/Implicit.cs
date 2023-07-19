using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Implicit casts a value node into another value node.</summary>
sealed public class Implicit<Tin, Tout> : UnaryValue<Tin, Tout>
    where Tin : struct, IData
    where Tout : struct, IImplicit<Tin, Tout>, IEquatable<Tout> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((value) => new Implicit<Tin, Tout>(value));

    /// <summary>Creates a node implicit cast.</summary>
    public Implicit() { }

    /// <summary>Creates a node implicit cast.</summary>
    /// <param name="source">This is the single parent for the source value.</param>
    public Implicit(IValueParent<Tin>? source = null) : base(source) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Implicit<Tin, Tout>();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Implicit<Tin, Tout>);

    /// <summary>Gets the value to cast from the parent during evaluation.</summary>
    /// <param name="value">The parent value to cast.</param>
    /// <returns>The cast of the parent value.</returns>
    protected override Tout OnEval(Tin value) => this.Value.CastFrom(value);
}
