using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>
    /// This is the factory for a node which has one specific parent
    /// followed by an arbitrary number of parents as the source of the value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parent value for this node.</typeparam>
    /// <typeparam name="Tn">The type of the remaining parents' values for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    sealed public class Function1N<T1, Tn, TReturn> : FuncDef<TReturn>
        where T1 : class, INode
        where Tn : class, INode
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        private readonly S.Func<T1, IEnumerable<Tn>, TReturn> handle;

        /// <summary>Creates a new one type followed by N-ary node factory.</summary>
        /// <param name="handle">The factory handle.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        /// <param name="passOne">
        /// Indicates if there is only one argument for a new node, return the argument.
        /// By default a Nary function will pass one unless otherwise indicated.
        /// </param>
        /// <param name="min">The minimum number of required nodes. May not be less than 1.</param>
        /// <param name="max">The maximum allowed number of nodes.</param>
        public Function1N(S.Func<T1, IEnumerable<Tn>, TReturn> handle,
            bool needsOneNoCast = false, bool passOne = false, int min = 1, int max = int.MaxValue) :
            base(S.Math.Max(1, min), max, needsOneNoCast, passOne, Type.FromType<T1>(), Type.FromType<Tn>()) =>
            this.handle = handle;

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Function1N<T1, Tn, TReturn>(this.handle,
            this.NeedsOneNoCast, this.PassThroughOne, this.MinArgs, this.MaxArgs);

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Function1N<T1, Tn, TReturn>);

        /// <summary>Builds and return the function node with the given arguments already casted.</summary>
        /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
        /// <returns>The resulting function node.</returns>
        protected override INode PostCastBuild(INode[] nodes) => this.handle((T1)nodes[0], nodes[1..].Cast<Tn>());
    }
}
