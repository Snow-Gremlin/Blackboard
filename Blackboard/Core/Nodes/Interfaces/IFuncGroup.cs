﻿namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFuncGroup: INode {

        /// <summary>Finds and returns the best matching function in this collection.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The best matching function or null if none match.</returns>
        IFuncDef Find(params Type[] types);

        /// <summary>Determines the type that will be returned for the given types.</summary>
        /// <param name="types">The types of the parameters being passed into the function.</param>
        /// <returns>Type that the function would return once it is built, or null if not a match.</returns>
        System.Type Returns(params Type[] types);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}