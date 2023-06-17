using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Functions;

/// <summary>This is the factory for a node which has arbitrary number of parents as the source of the value.</summary>
/// <typeparam name="Tn">The type of the parents' values for this node.</typeparam>
/// <typeparam name="TReturn">The type of this function will return.</typeparam>
sealed public class FunctionN<Tn, TReturn> : FuncDef<TReturn>
    where Tn : class, INode
    where TReturn : class, INode {

    /// <summary>The factory for creating the node.</summary>
    private readonly S.Func<IEnumerable<Tn>, TReturn> handle;

    /// <summary>Creates a new N-ary node factory.</summary>
    /// <param name="handle">The factory handle.</param>
    /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
    /// <param name="passOne">
    /// Indicates if there is only one argument for a new node, return the argument.
    /// By default a Nary function will pass one unless otherwise indicated.
    /// </param>
    /// <param name="min">The minimum number of required nodes.</param>
    /// <param name="max">The maximum allowed number of nodes.</param>
    public FunctionN(S.Func<IEnumerable<Tn>, TReturn> handle,
        bool needsOneNoCast = false, bool passOne = true, int min = 1, int max = int.MaxValue) :
        base(min, max, needsOneNoCast, passOne, Type.FromType<Tn>()) =>
        this.handle = handle;

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new FunctionN<Tn, TReturn>(this.handle,
        this.NeedsOneNoCast, this.PassThroughOne, this.MinArgs, this.MaxArgs);

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(FunctionN<Tn, TReturn>);

    /// <summary>Builds and return the function node with the given arguments already casted.</summary>
    /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
    /// <returns>The resulting function node.</returns>
    protected override INode PostCastBuild(INode[] nodes) => this.handle(nodes.Cast<Tn>());
}
