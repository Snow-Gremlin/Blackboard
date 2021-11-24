using Blackboard.Core.Data.Interfaces;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates when a node contains a data type.</summary>
    public interface IDataNode: INode {

        /// <summary>This gets the data being stored in this node.</summary>
        /// <returns>The data being stored.</returns>
        public IData Data { get; }
    }
}
