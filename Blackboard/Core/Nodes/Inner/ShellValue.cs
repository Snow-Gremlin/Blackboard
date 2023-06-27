using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>An external node as a placeholder for value node.</summary>
/// <typeparam name="T">The type of the value to hold.</typeparam>
sealed public class ShellValue<T> : UnaryValue<T, T>, IValueExtern<T>
    where T : struct, IData, IEquatable<T> {

    /// <summary>Creates a new extern value node.</summary>
    public ShellValue() { }

    /// <summary>Creates a new extern value node.</summary>
    /// <param name="source">The parent node initialized for this external.</param>
    public ShellValue(IValueParent<T> source) : base(source) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ShellValue<T>();

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Shell";

    /// <summary>If the parent is set, then this will be called, so just return the parent value.</summary>
    /// <param name="value">The value from the parent to pass through.</param>
    /// <returns>The value from the parent unchanged.</returns>
    protected override T OnEval(T value) => value;

    /// <summary>This sets the value of this node.</summary>
    /// <param name="value">The value to set.</param>
    /// <returns>True if the value has changed, false otherwise.</returns>
    public bool SetValue(T value) => this.UpdateValue(value);
}
