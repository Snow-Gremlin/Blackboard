using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>This indicates that the node has fields that can be read and written.</summary>
public interface IFieldWriter : IFieldReader {

    /// <summary>Writes or overwrites a new field to this node.</summary>
    /// <param name="name">The name of the field to write.</param>
    /// <param name="node">The node to write to the field.</param>
    public void WriteField(string name, INode? node);

    /// <summary>Removes fields from this node by name if they exist.</summary>
    /// <param name="names">The names of the fields to remove.</param>
    /// <returns>True if the fields were removed, false otherwise.</returns>
    public bool RemoveFields(IEnumerable<string> names);
}
