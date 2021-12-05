using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has arbitrary number of parents as the source of the value.</summary>
    /// <typeparam name="Tn">The type of the parents' values for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    sealed public class FunctionN<Tn, TReturn>: FuncDef<TReturn>
        where Tn : class, INode
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        private readonly S.Func<IEnumerable<Tn>, TReturn> hndl;

        /// <summary>Creates a new N-ary node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        /// <param name="passOne">
        /// Indicates if there is only one argument for a new node, return the argument.
        /// By default a Nary function will pass one unless otherwise indicated.
        /// For 
        /// </param>
        /// <param name="min">The minimum number of required nodes.</param>
        /// <param name="max">The maximum allowed number of nodes.</param>
        public FunctionN(S.Func<IEnumerable<Tn>, TReturn> hndl,
            bool needsOneNoCast = false, bool passOne = true, int min = 1, int max = int.MaxValue) :
            base(min, max, needsOneNoCast, passOne, Type.FromType<Tn>()) {
            this.hndl = hndl;
        }

        /// <summary>Builds and return the function node with the given arguments already casted.</summary>
        /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
        /// <returns>The resulting function node.</returns>
        protected override INode PostCastBuild(INode[] nodes) => this.hndl(nodes.Cast<Tn>());
    }
}
