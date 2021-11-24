using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>A child node is a node which may specify one or more parents.</summary>
    /// <remarks>
    /// It is upto the node to define how the parents are set since the parents are
    /// the input values into a node so there may be a specific number of them and
    /// they may be required to be a specific type of nodes.
    /// </remarks>
    public interface IChild: INode {

        /// <summary>This will enumerate the given nodes which are not null.</summary>
        /// <remarks>This is designed to be used to help return parents as an enumerator.</remarks>
        /// <param name="values">The parent nodes to enumerate.</param>
        /// <returns>The enumerator for the passed in nodes which are not null.</returns>
        static protected IEnumerable<IParent> EnumerateParents(params IParent[] values) => values.NotNull();

        /// <summary>The set of parent nodes to this node.</summary>
        public IEnumerable<IParent> Parents { get; }

        /// <summary>
        /// Installing this node into the parent will make this node be automatially called when
        /// the parent is updated. This should be done if the node is part of a definition.
        /// Do not do install if the node is only suppose to evaluate so that we aren't updating what
        /// we don't need to update and aren't constantly adding and removing children from parents.
        /// </summary>
        public void InstallIntoParents() {
            foreach (IParent parent in this.Parents)
                parent?.AddChildren(this);
        }
    }
}
