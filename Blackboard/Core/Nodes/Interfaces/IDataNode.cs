using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates when a node contains a data type.</summary>
    public interface IDataNode : INode {

        /// <summary>This gets the data being stored in this node.</summary>
        /// <returns>The data being stored.</returns>
        IData Data { get; }

        /// <summary>Determines if the node is constant or if all of it's parents are constant.</summary>
        /// <returns>True if constant, false otherwise.</returns>
        bool IsConstant { get; }

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>A literal of this node, itself if already literal, otherwise null.</returns>
        IConstant ToConstant();
    }
}
