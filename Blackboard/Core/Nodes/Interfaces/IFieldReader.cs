﻿using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>This indicates that the node has fields that can be read.</summary>
/// <remarks>
/// FieldReaders can have loops if a namespace is added into one of its children.
/// This loop is fine but means that algorithms must make sure algorithms
/// do not get stuck in that loop.
/// </remarks>
internal interface IFieldReader : INode {

    /// <summary>Determines if the given field by name exists.</summary>
    /// <param name="name">The name of the field to look for.</param>
    /// <returns>True if the name exists in this node.</returns>
    public bool ContainsField(string name);

    /// <summary>Reads the node for the field by the given name.</summary>
    /// <param name="name">The name for the node to look for.</param>
    /// <returns>The node or null if not found.</returns>
    public INode? ReadField(string name);

    /// <summary>Gets all the fields and their names in this reader.</summary>
    public IEnumerable<KeyValuePair<string, INode>> Fields { get; }
}
