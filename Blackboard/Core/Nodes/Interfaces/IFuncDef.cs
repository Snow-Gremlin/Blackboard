using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for function factory.</summary>
    public interface IFuncDef: INode {

        /// <summary>The collection of argument types.</summary>
        /// <remarks>
        /// If the maximum number of arguments is more than the number of
        /// argument types then the last argument is a repeatable variable length argument.
        /// </remarks>
        public IReadOnlyList<Type> ArgumentTypes { get; }

        /// <summary>
        /// Indicates that the last argument type maybe used zero
        /// or more times based on the maximum and minimum number of arguments.
        /// If false the last argument may only be used zero or one time.
        /// </summary>
        public bool LastArgVariable { get; }

        /// <summary>Determines the type that will be returned for the given types.</summary>
        /// <param name="types">The types of the parameters being passed into the function.</param>
        /// <returns>Type that the function would return once it is built, or null if not a match.</returns>
        public S.Type ReturnType { get; }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The closest match is lower but not negative.</returns>
        public FuncMatch Match(Type[] types);

        /// <summary>Builds and returns the function node.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes);
    }
}
