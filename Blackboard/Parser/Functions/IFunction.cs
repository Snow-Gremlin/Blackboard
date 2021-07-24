using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Functions {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFunction {

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        int Match(INode[] nodes);

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
