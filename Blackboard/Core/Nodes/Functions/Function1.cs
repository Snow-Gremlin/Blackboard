using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using S = System;

namespace Blackboard.Core.Nodes.Functions;

/// <summary>This is the factory for a node which has a single parent as the source of the value.</summary>
/// <typeparam name="T">The type of the parent's value for this node.</typeparam>
/// <typeparam name="TReturn">The type of this function will return.</typeparam>
sealed public class Function<T, TReturn> : FuncDef<TReturn>
    where T : class, INode
    where TReturn : class, INode {

    /// <summary>The factory for creating the node.</summary>
    readonly private S.Func<T, TReturn> handle;

    /// <summary>Creates a new singular node factory.</summary>
    /// <param name="handle">The factory handle.</param>
    /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
    public Function(S.Func<T, TReturn> handle, bool needsOneNoCast = false) :
        base(needsOneNoCast, false, Type.FromType<T>()) =>
        this.handle = handle;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Function<T, TReturn>(this.handle, this.NeedsOneNoCast);

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Function<T, TReturn>);

    /// <summary>Builds and return the function node with the given arguments already casted.</summary>
    /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
    /// <returns>The resulting function node.</returns>
    protected override INode PostCastBuild(INode[] nodes) => this.handle((T)nodes[0]);
}
