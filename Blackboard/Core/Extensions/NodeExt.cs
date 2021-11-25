using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of extensions for working with different types of Nodes.</summary>
    static class NodeExt {

        /// <summary>The values from the given input values.</summary>
        /// <typeparam name="T">The type of the values to get.</typeparam>
        /// <param name="nodes">The set of nodes to get all the values from.</param>
        /// <returns>The values from the given non-null nodes.</returns>
        static public IEnumerable<T> Values<T>(this IEnumerable<IValue<T>> nodes)
            where T : IData =>
            from node in nodes select node is not null ? node.Value : default;

        /// <summary>The triggers from the given input nodes.</summary>
        /// <param name="nodes">The set of nodes to get all the triggers from.</param>
        /// <returns>The triggers from the given non-null nodes.</returns>
        static public IEnumerable<bool> Triggers(this IEnumerable<ITrigger> nodes) =>
            from node in nodes select node?.Provoked ?? false;

        /// <summary>Determines if the node is constant or if all of it's parents are constant.</summary>
        /// <param name="node">The node to check if constant.</param>
        /// <returns>True if constant, false otherwise.</returns>
        static public bool IsConstant(this INode node) => node is IConstant ||
            (node is not IInput && node is IChild child && child.Parents.IsConstant());

        /// <summary>This determines if all the given nodes are constant.</summary>
        /// <param name="nodes">The nodes to check if constant.</param>
        /// <returns>True if all nodes are constant, false otherwise.</returns>
        static public bool IsConstant(this IEnumerable<INode> nodes) =>
            nodes.All(node => IsConstant(node));
    }
}
