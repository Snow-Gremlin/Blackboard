namespace Blackboard.Core.Interfaces {

    /// <summary>The interface for a node which has a name.</summary>
    public interface INamed: INode {

        /// <summary>Gets the name for the node.</summary>
        string Name { get; }
    }
}
