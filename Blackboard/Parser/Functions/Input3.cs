using Blackboard.Core.Interfaces;

namespace Blackboard.Parser.Functions {

    /// <summary>This is the factory for a node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the thrid parent's value for this node.</typeparam>
    public class Input3<T1, T2, T3>: IFunction {

        /// <summary>The node factory handle.</summary>
        /// <param name="input1">The first parent node to pass into the factory.</param>
        /// <param name="input2">The second parent node to pass into the factory.</param>
        /// <param name="input3">The third parent node to pass into the factory.</param>
        /// <returns>The new created node from this factory.</returns>
        public delegate INode Handle(T1 input1, T2 input2, T3 input3);

        /// <summary>The factory for creating the node.</summary>
        readonly private Handle hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Input3(Handle hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => IFunction.JoinMatches(
            nodes.Length == 3 ? 0 : -1,
            IFunction.Match<T1>(nodes[0]),
            IFunction.Match<T2>(nodes[1]),
            IFunction.Match<T3>(nodes[2]));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T1 node1 = IFunction.As<T1>(nodes[0]);
            T2 node2 = IFunction.As<T2>(nodes[1]);
            T3 node3 = IFunction.As<T3>(nodes[2]);
            return this.hndl(node1, node2, node3);
        }
    }
}
