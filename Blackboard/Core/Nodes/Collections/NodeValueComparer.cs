using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Collections;

/// <summary>A comparer which compares the values of the given nodes.</summary>
/// <typeparam name="T">The node type to compare.</typeparam>
sealed public class NodeValueComparer<T> : IEqualityComparer<T>
    where T : INode {

    /// <summary>Checks if the nodes have the same value.</summary>
    /// <param name="x">The first node to check equality with.</param>
    /// <param name="y">The second node to check equality against.</param>
    /// <returns>True if the values are the same, false otherwise.</returns>
    public bool Equals(T? x, T? y) => x?.SameValue(y) ?? y is null;

    /// <summary>Gets the hash code for the given node.</summary>
    /// <param name="node">The node to get the hash code for.</param>
    /// <returns>The hash code for the given node.</returns>
    public int GetHashCode(T node) => node.GetNodeHashCode();
}
