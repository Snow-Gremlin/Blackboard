using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Parser.Performers;
using Blackboard.Parser.Preppers;
using PP = PetiteParser;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser {

    // TODO: CHECK ALL COMMENTS

    /// <summary>This is the complete set of performers being prepared.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes vertually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed internal class FormulaBuilder: PP.ParseTree.PromptArgs {
        private readonly LinkedList<IWrappedNode> scopes;
        private readonly LinkedList<Type> types;
        private readonly LinkedList<IPrepper> stack;
        private readonly LinkedList<IPerformer> pending;

        /// <summary>Creates a new stack to store the current namespace depths while processing statements.</summary>
        /// <param name="driver">The driver this stack is for.</param>
        public FormulaBuilder(Driver driver) {
            this.Driver  = driver;
            this.scopes  = new LinkedList<IWrappedNode>();
            this.types   = new LinkedList<Type>();
            this.stack   = new LinkedList<IPrepper>();
            this.pending = new LinkedList<IPerformer>();

            // Call reset to prepare the formula.
            this.Reset();
        }

        /// <summary>The driver for the Blackboard these stacks belongs too.</summary>
        public readonly Driver Driver;

        /// <summary>Resets the stack back to the initial state.</summary>
        public void Reset() {
            // By recreating the real node for the global, any virtual nodes will no longer exist. 
            this.Global = new RealNode(this.Driver.Global);
            this.scopes.Clear();
            this.scopes.AddFirst(this.Global);

            this.pending.Clear();
            this.types.Clear();
            this.stack.Clear();
        }

        /// <summary>Clears the prepper stack and type stack without changing pending nor scopes.</summary>
        public void ClearStacks() {
            this.types.Clear();
            this.stack.Clear();
        }

        /// <summary>Gets the formula containing all the pending actions.</summary>
        public Formula ToFormula() => new(this.Driver, this.pending);
        
        /// <summary>Adds a pending performer into this formula.</summary>
        /// <param name="performer">The performer to add.</param>
        public void Add(IPerformer performer) =>
            this.pending.AddLast(performer);

        /// <summary>The global node prewrapped so that virtual nodes can be added to it.</summary>
        public IWrappedNode Global { get; private set; }

        /// <summary>Gets the current top of the scope stack.</summary>
        public IWrappedNode CurrentScope => this.scopes.First.Value;

        /// <summary>Gets a copy of the current scopes.</summary>
        public IWrappedNode[] Scopes => this.scopes.ToArray();

        /// <summary>Pops a top node from the scope.</summary>
        public void PopScope() => this.scopes.RemoveFirst();

        /// <summary>Pushes a new node onto the scope.</summary>
        /// <param name="node">The node to pussh on the scope.</param>
        public void PushScope(IWrappedNode node) {
            if (!node.Type.IsAssignableTo(typeof(Namespace)))
                throw new Exception("May only push Namespaces onto the scope").
                    With("Node Type", node.Type);

            this.scopes.AddFirst(node);
        }

        /// <summary>Pushes a prepper onto the stack.</summary>
        /// <param name="prepper">The prepper to push.</param>
        public void PushPrepper(IPrepper prepper) => this.stack.AddLast(prepper);

        /// <summary>Pops off a prepper is on the top of the stack.</summary>
        /// <typeparam name="T">The type of the prepper to read as.</typeparam>
        /// <returns>The prepper which was on top of the stack.</returns>
        public T PopPrepper<T>() where T : class, IPrepper {
            IPrepper item = this.stack.Last.Value;
            this.stack.RemoveLast();
            return item as T;
        }

        /// <summary>Pops one or more prepper off the stack.</summary>
        /// <param name="count">The number of preppers to pop.</param>
        /// <returns>The popped preppers in the order oldest to newest.</returns>
        public IPrepper[] PopPreppers(int count) {
            IPrepper[] items = new IPrepper[count];
            for (int i = 0; i < count; i++)
                items[count-1-i] = this.PopPrepper<IPrepper>();
            return items;
        }

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

        /// <summary>Gets the formula debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => this.StackString();

        /// <summary>Gets the formula debug string.</summary>
        /// <param name="showGlobal">Indicates that the namespaces starting from the global should be returned.</param>
        /// <param name="showScope">Indicates that the scope stack should be returned.</param>
        /// <param name="showPending">Indicates that pending performers should be returned.</param>
        /// <returns>A human readable debug string.</returns>
        public string StackString(bool showGlobal = true, bool showScope = true, bool showPending = true) {
            const string indent = "  ";
            List<string> parts = new();
            if (showGlobal)  parts.Add("Global: "+this.Global.ToString());
            if (showScope)   parts.Add("Scope: [" + this.scopes.Select((scope) => scope.ToSimpleString()).Join(", ") + "]");
            if (showPending) parts.Add("Pending: [\n" + indent + this.pending.Strings().Indent(indent).Join(",\n" + indent) + "]");
            return parts.Join("\n");
        }
    }
}
