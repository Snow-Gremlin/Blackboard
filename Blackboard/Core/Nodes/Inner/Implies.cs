using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Performs a boolean implies of two boolean parents.</summary>
/// <see cref="https://mathworld.wolfram.com/Implies.html"/>
sealed internal class Implies : BinaryValue<Bool, Bool, Bool> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((left, right) => new Implies(left, right));

    /// <summary>Creates an implied value node.</summary>
    public Implies() { }

    /// <summary>Creates an implied value node.</summary>
    /// <param name="left">This is the first parent for the source value.</param>
    /// <param name="right">This is the second parent for the source value.</param>
    public Implies(IValueParent<Bool>? left = null, IValueParent<Bool>? right = null) :
        base(left, right) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Implies();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Implies);

    /// <summary>Determines the boolean implies value of the two parents.</summary>
    /// <param name="left">The first parent being implied</param>
    /// <param name="right">The second parent implied.</param>
    /// <returns>The boolean implies value of the two given parents.</returns>
    protected override Bool OnEval(Bool left, Bool right) => new(!left.Value || right.Value);
}
