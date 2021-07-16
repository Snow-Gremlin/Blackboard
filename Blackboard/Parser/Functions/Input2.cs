﻿using Blackboard.Core.Interfaces;
using Blackboard.Core;

namespace Blackboard.Parser.Functions {

    /// <summary>This is the factory for a node which has two parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    public class Input2<T1, T2>: IFunction {

        /// <summary>The node factory handle.</summary>
        /// <param name="input1">The first parent node to pass into the factory.</param>
        /// <param name="input2">The second parent node to pass into the factory.</param>
        /// <returns>The new created node from this factory.</returns>
        public delegate INode Handle(T1 input1, T2 input2);

        /// <summary>The factory for creating the node.</summary>
        readonly private Handle hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Input2(Handle hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => Cast.JoinMatches(
            nodes.Length == 2 ? 0 : -1,
            Cast.Match<T1>(nodes[0]),
            Cast.Match<T2>(nodes[1]));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T1 node1 = Cast.As<T1>(nodes[0]);
            T2 node2 = Cast.As<T2>(nodes[1]);
            return this.hndl(node1, node2);
        }
    }
}
