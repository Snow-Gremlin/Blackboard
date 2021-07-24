namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for a named object.</summary>
    public interface INamed: INode {

        /// <summary>Gets or sets the name for the node.</summary>
        string Name { get; set; }

        /// <summary>Gets or sets the containing namespace scope for this name or null.</summary>
        INamespace Scope { get; set; }
    }
}
