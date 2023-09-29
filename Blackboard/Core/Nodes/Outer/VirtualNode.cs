using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Outer;

/// <summary>This represents a field reader which has been added to or removed from.</summary>
/// <remarks>
/// This is used during parsing to temporarily edit namespaces and field writers
/// so that nodes can be written and read as if they were fully added to the field writer
/// without changing the field writer. This makes it easier to cancel a parse.
/// This node should only be held onto while parsing and never actually written into the graph.
/// </remarks>
sealed internal class VirtualNode : IFieldWriter {

    /// <summary>The overridden nodes for this receiver.</summary>
    /// <remarks>If the node value is null, then the node has been deleted.</remarks>
    private readonly Dictionary<string, INode?> overrides;

    /// <summary>Creates a new virtual node around the given receiver.</summary>
    /// <param name="name">The is the name for this virtual node.</param>
    /// <param name="receiver">The receiver to virtually add and remove nodes from.</param>
    public VirtualNode(string name, IFieldWriter receiver) {
        this.Name = name;
        this.Receiver = receiver;
        this.overrides = new Dictionary<string, INode?>();

        if (receiver is null or VirtualNode)
            throw new BlackboardException("May not construct a virtual node with a null or virtual node receiver.");
    }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public INode NewInstance() => new VirtualNode(this.Name, this.Receiver);

    /// <summary>The receiver having children virtually added or removed from it.</summary>
    public readonly IFieldWriter Receiver;

    /// <summary>This is the name for this virtual node in its field reader.</summary>
    public readonly string Name;

    /// <summary>This is the type name of the node without any type parameters.</summary>
    public string TypeName => nameof(VirtualNode);

    /// <summary>Gets or sets the field in this namespace.</summary>
    /// <param name="name">The name of the field.</param>
    /// <returns>The node to get or set to this field.</returns>
    public INode? this[string name] {
        get => this.ReadField(name);
        set => this.WriteField(name, value);
    }

    /// <summary>Determines if the given field by name exists.</summary>
    /// <param name="name">The name of the field to look for.</param>
    /// <returns>True if the name exists in this node.</returns>
    public bool ContainsField(string name) =>
        this.overrides.TryGetValue(name, out INode? over) ? over is not null :
        this.Receiver.ContainsField(name);

    /// <summary>Reads the node for the field by the given name.</summary>
    /// <param name="name">The name for the node to look for.</param>
    /// <returns>The node or null if not found.</returns>
    public INode? ReadField(string name) {
        if (this.overrides.TryGetValue(name, out INode? over)) return over;
        INode? node = this.Receiver.ReadField(name);
        if (node is not IFieldWriter field) return node;
        VirtualNode vNode = new(name, field);
        this.overrides[name] = vNode;
        return vNode;
    }

    /// <summary>Gets all the fields and their names in this reader.</summary>
    /// <remarks>This does not wrap all the nodes in virtual nodes so DO NOT modify the returned nodes.</remarks>
    public IEnumerable<KeyValuePair<string, INode>> Fields {
        get {
            HashSet<string> reached = new();
            foreach (KeyValuePair<string, INode?> pair in this.overrides) {
                reached.Add(pair.Key);
                INode? value = pair.Value;
                if (value is not null) yield return new KeyValuePair<string, INode>(pair.Key, value);
            }
            foreach (KeyValuePair<string, INode> pair in this.Receiver.Fields) {
                if (!reached.Contains(pair.Key)) yield return pair;
            }
        }
    }

    /// <summary>Writes or overwrites a new field to this node.</summary>
    /// <param name="name">The name of the field to write.</param>
    /// <param name="node">The node to write to the field.</param>
    public void WriteField(string name, INode? node) => this.overrides[name] = node;

    /// <summary>Removes fields from this node by name if they exist.</summary>
    /// <param name="names">The names of the fields to remove.</param>
    /// <returns>True if the fields were removed, false otherwise.</returns>
    public bool RemoveFields(IEnumerable<string> names) {
        foreach (string name in names) this.overrides[name] = null;
        return true;
    }

    /// <summary>Gets the name for this virtual node in its field reader.</summary>
    /// <returns>The name of this node.</returns>
    public override string ToString() => this.Name;
}
