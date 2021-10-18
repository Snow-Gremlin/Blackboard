using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has no parents as the source of the value.</summary>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    sealed public class Function<TReturn>: FuncDef<TReturn>
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<TReturn> hndl;

        /// <summary>Creates a new singular node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Function(S.Func<TReturn> hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public override FuncMatch Match(Type[] types) =>
            types.Length != 0 ? FuncMatch.NoMatch : FuncMatch.Create(false);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public override INode Build(INode[] nodes) => this.hndl();
    }
}
