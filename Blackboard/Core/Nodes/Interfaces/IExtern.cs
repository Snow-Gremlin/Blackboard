namespace Blackboard.Core.Nodes.Interfaces;

/// <summary>The interface for an external node.</summary>
public interface IExtern : IParent {

    /// <summary>This is the child shell node for the extern.</summary>
    /// <remarks>
    /// When working with external nodes, any references in defines will use
    /// this shell instead. That allows the external node to be replaced and
    /// the children from the extern, which includes this shell, are moved from
    /// the external node to the replacement.
    /// </remarks>
    public INode Shell { get; }
}
