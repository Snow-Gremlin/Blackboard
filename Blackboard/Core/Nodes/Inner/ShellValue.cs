using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>An shell node as a placeholder for value node.</summary>
/// <remarks>
/// A shell should be used in only specific cases, such as when an input or extern
/// is used in a define by itself but we don't want the new define to be input or extern
/// just a node. This prevents a define from making a named copy which cannot be
/// updated with an extern being defined or to be assigned as a define.
/// </remarks>
/// <typeparam name="T">The type of the value to hold.</typeparam>
sealed public class ShellValue<T> : Evaluable, IValueParent<T>, IChild
    where T : struct, IData, IEquatable<T> {
    
    /// <summary>This is the parent node to read from.</summary>
    private IValueParent<T>? source;

    /// <summary>Creates a new shell value node.</summary>
    public ShellValue() { }

    /// <summary>Creates a new shell value node.</summary>
    /// <param name="source">The parent node initialized for this shell.</param>
    public ShellValue(IValueParent<T> source) => this.Parent = source;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new ShellValue<T>();
    
    /// <summary>The parent node to get the source value from.</summary>
    public IValueParent<T>? Parent {
        get => this.source;
        set => IChild.SetParent(this, ref this.source, value);
    }

    /// <summary>The set of parent nodes to this node in the graph.</summary>
    public ParentCollection Parents => new ParentCollection(this, 1).
        With(() => this.source, parent => this.source = parent);

    /// <summary>This is the type name of the node.</summary>
    /// <remarks>Doesn't use nameof since this has both trigger and value nodes.</remarks>
    public override string TypeName => "Shell";
    
    /// <summary>The value being held by this node's parent.</summary>
    public T Value => this.source?.Value ?? default;
    
    /// <summary>This gets the data being stored in this node's parent.</summary>
    /// <returns>The data being stored by the parent.</returns>
    public IData Data => this.Value;

    /// <summary>Normally updates the node's value, but for the shell this does nothing.</summary>
    /// <returns>Always returns true assuming if evaluated, it's parent has been changed.</returns>
    public override bool Evaluate() => true;
}
