using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Builder;

/// <summary>The stack of argument lists used for building up function calls.</summary>
sealed internal class ArgumentStack {
    private readonly LinkedList<LinkedList<INode>> argStacks;

    /// <summary>Create a new argument stack.</summary>
    internal ArgumentStack() => this.argStacks = new LinkedList<LinkedList<INode>>();

    /// <summary>Clears all the argument lists from the argument stack.</summary>
    public void Clear() => this.argStacks.Clear();

    /// <summary>This starts a new argument list.</summary>
    public void Start() => this.argStacks.AddFirst(new LinkedList<INode>());

    /// <summary>This adds the given node in to the newest argument list.</summary>
    /// <param name="node">The node to add to the argument list.</param>
    public void Add(INode node) {
        LinkedListNode<LinkedList<INode>> first = this.argStacks.First ??
            throw new Message("May not add an argument without first starting an argument list.");
        first.Value.AddLast(node);
    }

    /// <summary>This gets all the nodes which are in the current argument list, then removes the list.</summary>
    /// <returns>The nodes which were in the current argument list.</returns>
    public INode[] End() =>
        this.argStacks.First?.Value?.ToArray() ??
            throw new Message("May not end an argument without first starting an argument list.");

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => this.ToString("");

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <param name="indent">The indent to apply to all but the first line being returned.</param>
    /// <returns>The human readable string.</returns>
    public string ToString(string indent) =>
        this.argStacks.Count <= 0 ? "{}" :
        "{\n" + indent + this.argStacks.
            Select(list => "[" + list.Join(", ") + "]").
            Indent(indent).Join(",\n" + indent) + "}";
}
