using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Functions {

    /// <summary>This is the factory for a node which has abritrary parents as the source of the value.</summary>
    /// <typeparam name="T">The type of the parents' values for this node.</typeparam>
    public class InputN<T>: IFunction {

        /// <summary>The node factory handle.</summary>
        /// <param name="inputs">The parent nodes to pass into the factory.</param>
        /// <returns>The new created node from this factory.</returns>
        public delegate INode Handle(IEnumerable<T> inputs);

        /// <summary>The factory for creating the node.</summary>
        readonly private Handle hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public InputN(Handle hndl) {
            this.hndl = hndl;
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) =>
            Cast.JoinMatches(nodes.Select((node) => Cast.Match<T>(node)));

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) =>
            this.hndl(nodes.Select((node) => Cast.As<T>(node)));
    }
}
