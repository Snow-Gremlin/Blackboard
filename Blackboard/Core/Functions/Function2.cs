using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has two parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    public class Function<T1, T2>: IFunction
        where T1 : class, INode
        where T2 : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T1, T2, INode> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Function(S.Func<T1, T2, INode> hndl) {
            this.hndl = hndl;

            if (Type.FromType<T1>() is null) throw Exception.UnknownFunctionParamType(typeof(T1), "T1");
            if (Type.FromType<T2>() is null) throw Exception.UnknownFunctionParamType(typeof(T2), "T2");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => IFunction.Join(
            nodes.Length == 2 ? 0 : -1,
            Type.Match<T1>(nodes[0]),
            Type.Match<T2>(nodes[1]));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T1 node1 = Type.Implicit<T1>(nodes[0]);
            T2 node2 = Type.Implicit<T2>(nodes[1]);
            return this.hndl(node1, node2);
        }
    }
}
