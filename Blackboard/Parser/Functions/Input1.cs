using Blackboard.Core.Interfaces;
using Blackboard.Core;

namespace Blackboard.Parser.Functions {

    /// <summary>This is the factory for a node which has a single parent as the source of the value.</summary>
    /// <typeparam name="T">The type of the parent's value for this node.</typeparam>
    public class Input1<T>: IFunction {

        /// <summary>The node factory handle.</summary>
        /// <param name="input">The parent node to pass into the factory.</param>
        /// <returns>The new created node from this factory.</returns>
        public delegate INode Handle(T input);

        /// <summary>The factory for creating the node.</summary>
        readonly private Handle hndl;

        /// <summary>Creates a new singular node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public Input1(Handle hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) => Cast.JoinMatches(
            nodes.Length == 1 ? 0 : -1,
            Cast.Match<T>(nodes[0]));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            T node = Cast.As<T>(nodes[0]);
            return this.hndl(node);
        }
    }
}
