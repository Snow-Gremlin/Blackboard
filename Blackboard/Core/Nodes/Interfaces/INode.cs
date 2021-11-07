using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for all nodes in the blackboard.</summary>
    public interface INode {

        /// <summary>This is the type name of the node without any type parameters.</summary>
        string TypeName { get; }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        IEnumerable<INode> Parents { get; }

        /// <summary>The set of children nodes to this node in the graph.</summary>
        IEnumerable<INode> Children { get; }
    }
}
