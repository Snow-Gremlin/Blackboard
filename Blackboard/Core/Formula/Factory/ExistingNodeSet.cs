using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Factory;

/// <summary>A collection of existing nodes.</summary>
sealed internal class ExistingNodeSet {
    private readonly Logger? logger;
    private readonly HashSet<INode> nodes;

    /// <summary>Creates a new existing node set.</summary>
    /// <param name="logger">The optional logger to write debugging information to.</param>
    internal ExistingNodeSet(Logger? logger = null) {
        this.logger = logger;
        this.nodes  = new HashSet<INode>();
    }

    /// <summary>Clears the set of existing nodes.</summary>
    public void Clear() => this.nodes.Clear();

    /// <summary>Adds an existing node which has been referenced since the last clear.</summary>
    /// <param name="node">The existing node to add.</param>
    public void Add(INode node) {
        this.logger.Info("Add Existing: {0} ", node);
        this.nodes.Add(node is VirtualNode virt ? virt.Receiver : node);
    }

    /// <summary>Determines if the given node is in the existing nodes set.</summary>
    /// <param name="node">The node to check for.</param>
    /// <returns>True if the node is an existing node, false otherwise.</returns>
    public bool Has(INode node) => this.nodes.Contains(node);

    /// <summary>Gets the human readable string of the current existing nodes.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => this.ToString("");

    /// <summary>Gets the human readable string of the current existing nodes.</summary>
    /// <param name="indent">The indent to apply to all but the first line being returned.</param>
    /// <returns>The human readable string.</returns>
    public string ToString(string indent) =>
        this.nodes.Count <= 0 ? "[]" :
        "[\n" + indent + this.nodes.Strings().Indent(indent).Join(",\n" + indent) + "]";
}
