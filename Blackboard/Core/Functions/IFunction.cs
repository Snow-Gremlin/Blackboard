using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Functions {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFunction {

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        FuncMatch Match(Type[] types);

        /// <summary>Returns the type that would be return if built.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="types">The types passing into the function as parameters.</param>
        /// <returns>The type which would be returned.</returns>
        Type Returns(Type[] types);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
