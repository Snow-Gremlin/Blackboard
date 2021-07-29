using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the thrid parent's value for this node.</typeparam>
    public class Func<T1, T2, T3>: IFunction
        where T1 : class, INode
        where T2 : class, INode
        where T3 : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T1, T2, T3, INode> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Func(S.Func<T1, T2, T3, INode> hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => IFunction.Join(
            nodes.Length == 2 ? 0 : -1,
            Type.FromType<T1>().Match(Type.TypeOf(nodes[0])),
            Type.FromType<T2>().Match(Type.TypeOf(nodes[1])),
            Type.FromType<T3>().Match(Type.TypeOf(nodes[2])));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T1 node1 = Type.FromType<T1>().Implicit(nodes[0]) as T1;
            T2 node2 = Type.FromType<T2>().Implicit(nodes[1]) as T2;
            T3 node3 = Type.FromType<T3>().Implicit(nodes[2]) as T3;
            return this.hndl(node1, node2, node3);
        }
    }
}
