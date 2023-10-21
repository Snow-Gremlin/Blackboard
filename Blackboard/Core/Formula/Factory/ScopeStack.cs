using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formula.Factory;

/// <summary>The stack of the namespaces to represent the scope being worked on.</summary>
sealed internal class ScopeStack {
    private readonly Slate slate;
    private readonly Logger? logger;
    private readonly LinkedList<VirtualNode> scopes;

    /// <summary>Creates a new scope stack.</summary>
    /// <param name="slate">The slate to create the formula for.</param>
    /// <param name="logger">The optional logger to write debugging information to.</param>
    internal ScopeStack(Slate slate, Logger? logger = null) {
        this.slate  = slate;
        this.logger = logger;
        this.Global = this.createGlobal();
        this.scopes = new();
        this.scopes.AddFirst(this.Global);
    }

    /// <summary>Resets the scope to a new virtual global scope.</summary>
    /// <remarks>By creating a new global virtual node anything virtually written will be dropped.</remarks>
    public void Reset() {
        this.Global = this.createGlobal();
        this.scopes.Clear();
        this.scopes.AddFirst(this.Global);
    }

    /// <summary>Creates a new virtual global namespace.</summary>
    /// <returns>The new virtual node for global.</returns>
    private VirtualNode createGlobal() => new("Global", this.slate.Global);

    /// <summary>The global node as a virtual node so it can be temporarily added to and removed from.</summary>
    public VirtualNode Global { get; private set; }

    /// <summary>Gets the current top of the scope stack.</summary>
    public VirtualNode Current => this.scopes.First?.Value ?? this.Global;

    /// <summary>Gets a copy of the current scopes.</summary>
    public VirtualNode[] Scopes => this.scopes.ToArray();

    /// <summary>Gets the names of the virtual nodes in read order and without global.</summary>
    public IEnumerable<string> Names => this.scopes.Reverse().Skip(1).Select(vn => vn.Name);

    /// <summary>Finds the given ID in the current scopes.</summary>
    /// <param name="name">The name of the node to find.</param>
    /// <returns>The found node by that name or null if not found.</returns>
    public INode? FindId(string name) {
        foreach (VirtualNode scope in this.scopes) {
            INode? node = scope.ReadField(name);
            if (node is not null) return node;
        }
        return null;
    }

    /// <summary>Pops a top node from the scope.</summary>
    public void Pop() {
        this.logger.Info("Pop Scope");
        this.scopes.RemoveFirst();
    }

    /// <summary>Pushes a new node onto the scope.</summary>
    /// <param name="node">The node to push on the scope.</param>
    public void Push(VirtualNode node) {
        this.logger.Info("Push Scope: {0}", node);
        this.scopes.AddFirst(node);
    }

    /// <summary>Gets the human readable string of the current scope.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => "[" + this.scopes.Join(", ") + "]";
}
