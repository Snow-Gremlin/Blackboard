using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Functions {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFunction {

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        FuncMatch Match(INode[] nodes);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been positive.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
