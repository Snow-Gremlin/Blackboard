using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Builder;

sealed internal partial class Builder {

    /// <summary>The stack of the namespaces to represent the scope being worked on.</summary>
    public class ScopeStack {
        private readonly Builder builder;
        private readonly LinkedList<VirtualNode> scopes;

        /// <summary>Creates a new scope stack.</summary>
        /// <param name="builder">The builder this stack belongs too.</param>
        internal ScopeStack(Builder builder) {
            this.builder = builder;
            this.Global  = new VirtualNode("Global", this.builder.Slate.Global);
            this.scopes  = new LinkedList<VirtualNode>();
            this.scopes.AddFirst(this.Global);
        }

        /// <summary>Resets the scope to a new virtual global scope.</summary>
        /// <remarks>By creating a new global virtual node anything virtually written will be dropped.</remarks>
        public void Reset() {
            this.Global = new VirtualNode("Global", this.builder.Slate.Global);
            this.scopes.Clear();
            this.scopes.AddFirst(this.Global);
        }

        /// <summary>The global node as a virtual node so it can be temporarily added to and removed from.</summary>
        public VirtualNode Global { get; private set; }

        /// <summary>Gets the current top of the scope stack.</summary>
        public VirtualNode Current => this.scopes.First?.Value ?? this.Global;

        /// <summary>Gets a copy of the current scopes.</summary>
        public VirtualNode[] Scopes => this.scopes.ToArray();

        /// <summary>Finds the given ID in the current scopes.</summary>
        /// <param name="name">The name of the node to find.</param>
        /// <returns>The found node by that name or null if not found.</returns>
        public INode? FindID(string name) {
            foreach (VirtualNode scope in this.Scopes) {
                INode? node = scope.ReadField(name);
                if (node is not null) return node;
            }
            return null;
        }

        /// <summary>Pops a top node from the scope.</summary>
        public void Pop() {
            this.builder.Logger.Info("Pop Scope");
            this.scopes.RemoveFirst();
        }

        /// <summary>Pushes a new node onto the scope.</summary>
        /// <param name="node">The node to push on the scope.</param>
        public void Push(VirtualNode node) {
            this.builder.Logger.Info("Push Scope: {0}", node);
            this.scopes.AddFirst(node);
        }

        /// <summary>Gets the human readable string of the current scope.</summary>
        /// <returns>The human readable string.</returns>
        public override string ToString() => "[" + this.scopes.Join(", ") + "]";
    }
}
