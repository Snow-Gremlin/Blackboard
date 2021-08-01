using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has no parents as the source of the value.</summary>
    /// <typeparam name="T">The type of the parent's value for this node.</typeparam>
    public class Function: IFunction {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<INode> hndl;

        /// <summary>Creates a new singular node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Function(S.Func<INode> hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => nodes.Length != 0 ? -1 : 0;

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been positive.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) => this.hndl();
    }
}
