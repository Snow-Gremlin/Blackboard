namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFuncDef: INode {

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        FuncMatch Match(Type[] types);

        /// <summary>Determines the type that will be returned for the given types.</summary>
        /// <param name="types">The types of the parameters being passed into the function.</param>
        /// <returns>Type that the function would return once it is built, or null if not a match.</returns>
        System.Type ReturnType { get; }

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
