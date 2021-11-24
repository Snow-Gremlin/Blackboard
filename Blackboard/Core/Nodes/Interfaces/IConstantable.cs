namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates nodes which can be or is const or literal.</summary>
    public interface IConstantable: INode {

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>A literal of this node, itself if already literal, otherwise null.</returns>
        public IConstant ToConstant();
    }
}
