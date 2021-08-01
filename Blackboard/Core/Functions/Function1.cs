using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has a single parent as the source of the value.</summary>
    /// <typeparam name="T">The type of the parent's value for this node.</typeparam>
    public class Function<T>: IFunction
        where T : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T, INode> hndl;

        /// <summary>Creates a new singular node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Function(S.Func<T, INode> hndl) {
            this.hndl = hndl;

            if (Type.FromType<T>() is null) throw Exception.UnknownFunctionParamType(typeof(T), "T1");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) =>
            nodes.Length != 1 ? -1 :
            Type.Match<T>(nodes[0]);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been positive.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T node = Type.Implicit<T>(nodes[0]);
            return this.hndl(node);
        }
    }
}
