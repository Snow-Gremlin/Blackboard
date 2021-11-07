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
        public Function(S.Func<TReturn> hndl) :
            base(false, false) {
            this.hndl = hndl;
        }

        /// <summary>Builds and return the function node with the given arguments already casted.</summary>
        /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
        /// <returns>The resulting function node.</returns>
        protected override INode PostCastBuild(INode[] nodes) => this.hndl();
    }
}
