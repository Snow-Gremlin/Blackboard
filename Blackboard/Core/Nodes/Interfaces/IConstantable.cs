namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates nodes which can be or is const or literal.</summary>
    public interface IConstantable {

        /// <summary>Determines if the node is constant or if all of it's parents are constant.</summary>
        /// <returns>True if constant, false otherwise.</returns>
        bool IsConstant { get; }

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>A literal of this node, itself if already literal, otherwise null.</returns>
        IConstant ToConstant();
    }
}
