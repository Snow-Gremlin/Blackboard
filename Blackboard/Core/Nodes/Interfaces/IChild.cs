using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>A child node is a node which may specify one or more parents.</summary>
    /// <remarks>
    /// It is up to the node to define how the parents are set since the parents are
    /// the input values into a node so there may be a specific number of them and
    /// they may be required to be a specific type of nodes.
    /// </remarks>
    public interface IChild: INode {

        /// <summary>This will enumerate the given nodes which are not null.</summary>
        /// <remarks>This is designed to be used to help return parents as an enumerator.</remarks>
        /// <param name="values">The parent nodes to enumerate.</param>
        /// <returns>The enumerator for the passed in nodes which are not null.</returns>
        static protected IEnumerable<IParent> EnumerateParents(params INode[] values) =>
            values.NotNull().OfType<IParent>();

        /// <summary>The set of parent nodes to this node.</summary>
        public IEnumerable<IParent> Parents { get; }
    }
}
