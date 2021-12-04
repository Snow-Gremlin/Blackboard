using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;
using PP = PetiteParser;

namespace Blackboard.Parser {

    /// <summary>This is a tool for keeping the parse state while building a formula.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes virtually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed internal class Builder: PP.ParseTree.PromptArgs {
        private readonly LinkedList<VirtualNode> scopes;
        private readonly LinkedList<Type> types;
        private readonly LinkedList<INode> stack;
        private readonly HashSet<INode> newNodes;
        private readonly LinkedList<LinkedList<INode>> argStacks;
        private readonly LinkedList<IAction> actions;

        /// <summary>Creates a new formula builder for parsing states.</summary>
        /// <param name="driver">The driver this stack is for.</param>
        public Builder(Driver driver) {
            this.Driver    = driver;
            this.scopes    = new LinkedList<VirtualNode>();
            this.types     = new LinkedList<Type>();
            this.stack     = new LinkedList<INode>();
            this.newNodes  = new HashSet<INode>();
            this.argStacks = new LinkedList<LinkedList<INode>>();
            this.actions   = new LinkedList<IAction>();

            // Call reset to prepare the formula.
            this.Reset();
        }

        /// <summary>The driver for the Blackboard these stacks belongs too.</summary>
        public readonly Driver Driver;

        /// <summary>Resets the stack back to the initial state.</summary>
        public void Reset() {
            this.Global = new VirtualNode("Global", this.Driver.Global);
            this.scopes.Clear();
            this.scopes.AddFirst(this.Global);
            this.actions.Clear();
            this.Clear();
        }

        /// <summary>Clears the node stack and type stack without changing pending nor scopes.</summary>
        public void Clear() {
            this.types.Clear();
            this.stack.Clear();
            this.newNodes.Clear();
            this.argStacks.Clear();
        }

        #region Actions...

        /// <summary>Gets the formula containing all the pending actions.</summary>
        public IAction ToAction() => this.actions.Count <= 0 ? this.actions.First.Value : new Formula(this.actions);

        /// <summary>Adds a pending action into this formula.</summary>
        /// <param name="performer">The performer to add.</param>
        public void AddAction(IAction action) => this.actions.AddLast(action);

        #endregion
        #region Scope...

        /// <summary>The global node as a virtual node so it can be temporarily added to and removed from.</summary>
        public VirtualNode Global { get; private set; }

        /// <summary>Gets the current top of the scope stack.</summary>
        public VirtualNode CurrentScope => this.scopes.First.Value;

        /// <summary>Gets a copy of the current scopes.</summary>
        public VirtualNode[] Scopes => this.scopes.ToArray();

        /// <summary>Pops a top node from the scope.</summary>
        public void PopScope() => this.scopes.RemoveFirst();

        /// <summary>Pushes a new node onto the scope.</summary>
        /// <param name="node">The node to push on the scope.</param>
        public void PushScope(VirtualNode node) => this.scopes.AddFirst(node);

        #endregion
        #region Stack...

        /// <summary>Pushes a node onto the stack.</summary>
        /// <param name="node">The node to push.</param>
        public void Push(INode node) => this.stack.AddLast(node);

        /// <summary>Pops off a node is on the top of the stack.</summary>
        /// <typeparam name="T">The type of the node to read as.</typeparam>
        /// <returns>The node which was on top of the stack.</returns>
        public T Pop<T>() where T : class, INode {
            INode item = this.stack.Last.Value;
            this.stack.RemoveLast();
            return item as T;
        }

        /// <summary>Pops off a node is on the top of the stack.</summary>
        /// <returns>The node which was on top of the stack.</returns>
        public INode Pop() => this.Pop<INode>();

        /// <summary>Pops one or more node off the stack.</summary>
        /// <param name="count">The number of node to pop.</param>
        /// <returns>The popped node in the order oldest to newest.</returns>
        public INode[] Pop(int count) {
            INode[] items = new INode[count];
            for (int i = 0; i < count; i++) items[^i] = this.Pop<INode>();
            return items;
        }

        /// <summary>
        /// Pushes a node to the stack and add it as a node
        /// which has been created since the last clear.
        /// </summary>
        /// <param name="node">The node to push and add.</param>
        public void PushNew(INode node) {
            this.stack.AddLast(node);
            this.newNodes.Add(node);
        }

        /// <summary>Gets all the nodes which have been added since the last clear.</summary>
        public IEnumerable<INode> NewNodes => this.newNodes;

        /// <summary>Adds a node which has been created since the last clear.</summary>
        /// <param name="node">The node to add.</param>
        public void AddNewNode(INode node) => this.newNodes.Add(node);

        #endregion
        #region Arguments Stack...

        /// <summary>This starts a new argument list.</summary>
        public void StartArgs() => this.argStacks.AddFirst(new LinkedList<INode>());

        /// <summary>This adds the given node in to the newest argument list.</summary>
        /// <param name="node">The node to add to the argument list.</param>
        public void AddArg(INode node) => this.argStacks.First.Value.AddLast(node);

        /// <summary>This gets all the nodes which are in the current argument list, then removes the list.</summary>
        /// <returns>The nodes which were in the current argument list.</returns>
        public INode[] EndArgs() => this.argStacks.TakeFirst().ToArray();

        #endregion
        #region Type Stack...

        /// <summary>Pushes a type onto the stack of types.</summary>
        /// <param name="value">The type to push.</param>
        public void PushType(Type value) => this.types.AddLast(value);

        /// <summary>Pops off a type is on the top of the stack of types.</summary>
        /// <returns>The type which was on top of the stack.</returns>
        public Type PopType() {
            Type value = this.types.Last.Value;
            this.types.RemoveLast();
            return value;
        }

        #endregion

        /// <summary>Gets the formula debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => this.StackString();

        /// <summary>Gets the formula debug string.</summary>
        /// <param name="showGlobal">Indicates that the namespaces starting from the global should be shown.</param>
        /// <param name="showScope">Indicates that the scope stack should be shown.</param>
        /// <param name="showTypes">Indicates that the type stack should be shown.</param>
        /// <param name="showStack">Indicates that the node stack should be shown.</param>
        /// <param name="showActions">Indicates that pending actions should be shown.</param>
        /// <returns>A human readable debug string.</returns>
        public string StackString(bool showGlobal = true, bool showScope = true, bool showTypes = true, bool showStack = true, bool showActions = true) {
            const string indent = "  ";
            List<string> parts = new();
            if (showGlobal)  parts.Add("Global: "+this.Global.ToString());
            if (showScope)   parts.Add("Scope: [" + this.scopes.Strings().Join(", ") + "]");
            if (showTypes)   parts.Add("Types: [" + this.types.Strings().Join(", ") + "]");
            if (showStack)   parts.Add("Stack: [\n" + indent + this.stack.Strings().Indent(indent).Join(",\n" + indent) + "]");
            if (showActions) parts.Add("Actions: [\n" + indent + this.actions.Strings().Indent(indent).Join(",\n" + indent) + "]");
            return parts.Join("\n");
        }
    }
}
