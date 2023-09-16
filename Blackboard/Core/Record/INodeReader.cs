using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Record;

/// <summary>An interface for getting nodes.</summary>
public interface INodeReader {

    /// <summary>Tries to get the node with the given node.</summary>
    /// <param name="names">The name of the node to get.</param>
    /// <param name="node">The returned node for the given name or null.</param>
    /// <returns>True if the node was found, false otherwise.</returns>
    public bool TryGetNode(IEnumerable<string> names, out INode? node);
}
