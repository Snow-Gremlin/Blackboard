﻿using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Functions;

/// <summary>This is the factory for a node which has no parents as the source of the value.</summary>
/// <typeparam name="TReturn">The type of this function will return.</typeparam>
sealed internal class Function<TReturn> : FuncDef<TReturn>
    where TReturn : class, INode {

    /// <summary>The factory for creating the node.</summary>
    readonly private S.Func<TReturn> handle;

    /// <summary>Creates a new singular node factory.</summary>
    /// <param name="handle">The factory handle.</param>
    public Function(S.Func<TReturn> handle) :
        base(false, false) =>
        this.handle = handle;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Function<TReturn>(this.handle);

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Function<TReturn>);

    /// <summary>Builds and return the function node with the given arguments already casted.</summary>
    /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
    /// <returns>The resulting function node.</returns>
    protected override INode PostCastBuild(INode[] nodes) => this.handle();
}
