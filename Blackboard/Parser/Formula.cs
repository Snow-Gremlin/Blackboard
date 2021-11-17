using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Parser.Performers;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser {

    /// <summary>This is the complete set of performers being prepared.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes vertually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed internal class Formula {

        /// <summary>The driver for the Blackboard to apply this formula to.</summary>
        public readonly Driver Driver;

        private readonly LinkedList<IWrappedNode> scopes;
        private readonly LinkedList<IPerformer> pending;

        // TODO: Add a collection of nodes which can NOT be gotten because they are pending removal.

        /// <summary>Creates a new formula to store pending changes to the given driver.</summary>
        /// <param name="driver">The driver this formula will change.</param>
        public Formula(Driver driver) {
            this.Driver  = driver;
            this.scopes  = new LinkedList<IWrappedNode>();
            this.pending = new LinkedList<IPerformer>();

            // Call reset to prepare the formula.
            this.Reset();
        }

        /// <summary>Adds a pending performer into this formula.</summary>
        /// <param name="performer">The performer to add.</param>
        public void Add(IPerformer performer) =>
            this.pending.AddLast(performer);

        /// <summary>Performs all pending actions then resets the formula.</summary>
        public void Perform() {
            this.Driver.Touch(this.pending.Select(perf => perf.Perform()).OfType<IEvaluatable>());
            this.Reset();
        }

        /// <summary>Resets the formula back to the initial state.</summary>
        public void Reset() {
            this.pending.Clear();
            this.scopes.Clear();
            this.Global = new RealNode(this.Driver.Global);
            this.scopes.AddFirst(this.Global);
        }

        /// <summary>The global node prewrapped so that virtual nodes can be added to it.</summary>
        public IWrappedNode Global { get; private set; }

        /// <summary>Gets the current top of the scope stack.</summary>
        public IWrappedNode CurrentScope => this.scopes.First.Value;

        /// <summary>Gets a copy of the current scopes.</summary>
        public IWrappedNode[] Scopes => this.scopes.ToArray();

        /// <summary>Pushes a new node onto the scope.</summary>
        /// <param name="node">The node to pussh on the scope.</param>
        public void PushScope(IWrappedNode node) {
            if (!node.Type.IsAssignableTo(typeof(Namespace)))
                throw new Exception("May only push Namespaces onto the scope").
                    With("Node Type", node.Type);

            this.scopes.AddFirst(node);
        }

        /// <summary>Pops a top node from the scope.</summary>
        public void PopScope() => this.scopes.RemoveFirst();

        /// <summary>Gets the formula debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => this.FormulaString();

        /// <summary>Gets the formula debug string.</summary>
        /// <param name="showGlobal">Indicates that the namespaces starting from the global should be returned.</param>
        /// <param name="showScope">Indicates that the scope stack should be returned.</param>
        /// <param name="showPending">Indicates that pending performers should be returned.</param>
        /// <returns>A human readable debug string.</returns>
        public string FormulaString(bool showGlobal = true, bool showScope = true, bool showPending = true) {
            const string indent = "  ";
            List<string> parts = new();
            if (showGlobal)  parts.Add("Global: "+this.Global.ToString());
            if (showScope)   parts.Add("Scope: [" + this.scopes.Select((scope) => scope.ToSimpleString()).Join(", ") + "]");
            if (showPending) parts.Add("Pending: [\n" + indent + this.pending.Strings().Indent(indent).Join(",\n" + indent) + "]");
            return parts.Join("\n");
        }
    }
}
