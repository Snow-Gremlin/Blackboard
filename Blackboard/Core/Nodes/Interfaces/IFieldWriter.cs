using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>This indicates that the node has fields that can be read and written.</summary>
    public interface IFieldWriter: IFieldReader {

        /// <summary>Writes or overwrites a new field to this node.</summary>
        /// <param name="name">The name of the field to write.</param>
        /// <param name="node">The node to write to the field.</param>
        /// <param name="checkedForLoops">Indicates if loops in the graph should be checked for.</param>
        void WriteField(string name, INode node, bool checkedForLoops = true);

        /// <summary>Remove a field from this node by name if it exists.</summary>
        /// <param name="name">The name of the fields to remove.</param>
        /// <returns>True if the field wwas removed, false otherwise.</returns>
        bool RemoveField(string name);
    }
}
