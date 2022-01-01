using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
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

        /// <summary>Creates a new formula builder for parsing states.</summary>
        /// <param name="slate">The slate this stack is for.</param>
        /// <param name="logger">The optional logger to output the build steps.</param>
        public Builder(Slate slate, ILogger logger = null) {
            this.Slate  = slate;
            this.Logger = logger;

            this.Actions     = new ActionCollection(this);
            this.Scope       = new ScopeStack(this);
            this.Nodes       = new BuilderStack<INode>("Node", this);
            this.Types       = new BuilderStack<Type>("Type", this);
            this.Identifiers = new BuilderStack<string>("Id", this);
            this.Existing    = new ExistingNodeSet(this);
            this.Arguments   = new ArgumentStack(this);
        }

        /// <summary>The slate for the Blackboard these stacks belongs too.</summary>
        public readonly Slate Slate;

        /// <summary>
        /// The logger to help debug the parser and builder.
        /// This may be null if no logger is being used.
        /// </summary>
        public readonly ILogger Logger;

        /// <summary>Resets the stack back to the initial state.</summary>
        public void Reset() {
            this.Logger?.Log("Reset");

            this.Actions.Clear();
            this.Scope.Reset();

            this.Nodes.Clear();
            this.Types.Clear();
            this.Identifiers.Clear();
            this.Arguments.Clear();
            this.Existing.Clear();
        }

        /// <summary>Clears the node stack and type stack without changing pending actions nor scopes.</summary>
        public void Clear() {
            this.Logger?.Log("Clear");

            this.Nodes.Clear();
            this.Types.Clear();
            this.Identifiers.Clear();
            this.Arguments.Clear();
            this.Existing.Clear();
        }

        #region Actions...

        /// <summary>The collection of actions which have been parsed.</summary>
        public class ActionCollection {
            private readonly Builder builder;
            private readonly LinkedList<IAction> actions;

            /// <summary>Creates new a new action collection.</summary>
            /// <param name="builder">The builder this collection belongs to.</param>
            internal ActionCollection(Builder builder) {
                this.builder = builder;
                this.actions = new LinkedList<IAction>();
            }

            /// <summary>Clears the collection of actions.</summary>
            public void Clear() => this.actions.Clear();

            /// <summary>
            /// Gets the formula containing all the actions or null if there were no actions.
            /// </summary>
            public Formula Formula =>
                this.actions.Count <= 0 ? null :
                new Formula(this.builder.Slate, this.actions.Append(new Finish()));

            /// <summary>Adds a pending action into this formula.</summary>
            /// <param name="performer">The performer to add.</param>
            public void Add(IAction action) {
                this.builder.Logger?.Log("Add Action: {0}", action);
                this.actions.AddLast(action);
            }

            /// <summary>Gets the human readable string of the current actions.</summary>
            /// <returns>The human readable string.</returns>
            public override string ToString() => this.ToString("");

            /// <summary>Gets the human readable string of the current actions.</summary>
            /// <param name="indent">The indent to apply to all but the first line being returned.</param>
            /// <returns>The human readable string.</returns>
            public string ToString(string indent) =>
                this.actions.Count <= 0 ? "[]" :
                "[\n" + indent + this.actions.Strings().Indent(indent).Join(",\n" + indent) + "]";
        }

        /// <summary>The collection of actions which will perform the actions which have been parsed.</summary>
        public readonly ActionCollection Actions;

        #endregion
        #region Scope...

        /// <summary>The stack of the namespaces to represent the scope being worked on.</summary>
        public class ScopeStack {
            private readonly Builder builder;
            private readonly LinkedList<VirtualNode> scopes;

            /// <summary>Creates a new scope stack.</summary>
            /// <param name="builder">The builder this stack belongs too.</param>
            internal ScopeStack(Builder builder) {
                this.builder = builder;
                this.scopes  = new LinkedList<VirtualNode>();

                // Reset to prepare the global virtual node.
                this.Reset();
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
            public VirtualNode Current => this.scopes.First.Value;

            /// <summary>Gets a copy of the current scopes.</summary>
            public VirtualNode[] Scopes => this.scopes.ToArray();

            /// <summary>Pops a top node from the scope.</summary>
            public void Pop() {
                this.builder.Logger?.Log("Pop Scope");
                this.scopes.RemoveFirst();
            }

            /// <summary>Pushes a new node onto the scope.</summary>
            /// <param name="node">The node to push on the scope.</param>
            public void Push(VirtualNode node) {
                this.builder.Logger?.Log("Push Scope: {0}", node);
                this.scopes.AddFirst(node);
            }

            /// <summary>Gets the human readable string of the current scope.</summary>
            /// <returns>The human readable string.</returns>
            public override string ToString() => "[" + this.scopes.Join(", ") + "]";
        }

        /// <summary>The stack of the namespaces to represent the scope being worked on.</summary>
        public readonly ScopeStack Scope;

        #endregion
        #region Stacks...

        /// <summary>The stack of values which are currently being worked on during a parse.</summary>
        public class BuilderStack<T> {
            private readonly string usage;
            private readonly Builder builder;
            private readonly LinkedList<T> stack;

            /// <summary>Creates a new stack.</summary>
            /// <param name="usage">The short usage of this stack used for logging.</param>
            /// <param name="builder">The builder this stack belongs to.</param>
            internal BuilderStack(string usage, Builder builder) {
                this.usage   = usage;
                this.builder = builder;
                this.stack   = new LinkedList<T>();
            }

            /// <summary>Removes all the values from this stack.</summary>
            public void Clear() => this.stack.Clear();

            /// <summary>Pushes a value onto the stack.</summary>
            /// <param name="value">The value to push.</param>
            public void Push(T value) {
                this.builder.Logger?.Log("Push {0}: {1}", this.usage, value);
                this.stack.AddLast(value);
            }

            /// <summary>Peeks the value off the top of the stack without removing it.</summary>
            /// <returns>The value that is on the top of the stack.</returns>
            public T Peek() => this.stack.Last.Value;

            /// <summary>Pops off a value is on the top of the stack.</summary>
            /// <returns>The value which was on top of the stack.</returns>
            public T Pop() {
                this.builder.Logger?.Log("Pop {0}", this.usage);
                T node = this.stack.Last.Value;
                this.stack.RemoveLast();
                return node;
            }

            /// <summary>Pops one or more values off the stack.</summary>
            /// <param name="count">The number of values to pop.</param>
            /// <returns>The popped values in the order oldest to newest.</returns>
            public T[] Pop(int count) {
                this.builder.Logger?.Log("Pop {1} {0}(s)", this.usage, count);
                T[] items = new T[count];
                for (int i = count-1; i >= 0; i--) {
                    items[i] = this.stack.Last.Value;
                    this.stack.RemoveLast();
                }
                return items;
            }

            /// <summary>Gets the human readable string of the current stack.</summary>
            /// <returns>The human readable string.</returns>
            public override string ToString() => this.ToString("", false);

            /// <summary>Gets the human readable string of the current stack.</summary>
            /// <param name="indent">The indent to apply to all but the first line being returned.</param>
            /// <param name="inline">Indicates if the output should be one line or multiple lines.</param>
            /// <returns>The human readable string.</returns>
            public string ToString(string indent, bool inline) =>
                this.stack.Count <= 0 ? "[]" :
                inline ? "[" + this.stack.Join(", ") + "]" :
                "[\n" + indent + this.stack.Strings().Indent(indent).Join(",\n" + indent) + "]";
        }

        /// <summary>The stack of nodes which are currently being parsed but haven't been consumed yet.</summary>
        public readonly BuilderStack<INode> Nodes;

        /// <summary>A stack of types which have been read during the parse.</summary>
        public readonly BuilderStack<Type> Types;

        /// <summary>A stack of identifiers which have been read but not used yet during the parse.</summary>
        public readonly BuilderStack<string> Identifiers;

        #endregion
        #region Arguments...

        /// <summary>The stack of argument lists used for building up function calls.</summary>
        public class ArgumentStack {
            private readonly Builder builder;
            private readonly LinkedList<LinkedList<INode>> argStacks;

            /// <summary>Create a new argument stack.</summary>
            /// <param name="builder">The builder this stack belong to.</param>
            internal ArgumentStack(Builder builder) {
                this.builder   = builder;
                this.argStacks = new LinkedList<LinkedList<INode>>();
            }

            /// <summary>Clears all the argument lists from the argument stack.</summary>
            public void Clear() => this.argStacks.Clear();

            /// <summary>This starts a new argument list.</summary>
            public void Start() {
                this.builder.Logger?.Log("Start Arguments");
                this.argStacks.AddFirst(new LinkedList<INode>());
            }

            /// <summary>This adds the given node in to the newest argument list.</summary>
            /// <param name="node">The node to add to the argument list.</param>
            public void Add(INode node) {
                this.builder.Logger?.Log("Add Argument: {0}", node);
                this.argStacks.First.Value.AddLast(node);
            }

            /// <summary>This gets all the nodes which are in the current argument list, then removes the list.</summary>
            /// <returns>The nodes which were in the current argument list.</returns>
            public INode[] End() {
                this.builder.Logger?.Log("End Arguments");
                return this.argStacks.TakeFirst().ToArray();
            }

            /// <summary>Gets the human readable string of the current actions.</summary>
            /// <returns>The human readable string.</returns>
            public override string ToString() => this.ToString("");

            /// <summary>Gets the human readable string of the current actions.</summary>
            /// <param name="indent">The indent to apply to all but the first line being returned.</param>
            /// <returns>The human readable string.</returns>
            public string ToString(string indent) =>
                this.argStacks.Count <= 0 ? "{}" :
                "{\n" + indent + this.argStacks.Select(list => "[" + list.Join(", ") + "]").Indent(indent).Join(",\n" + indent) + "}";
        }

        /// <summary>The stack of argument lists used for building up function calls.</summary>
        public readonly ArgumentStack Arguments;

        #endregion
        #region Existing...

        /// <summary>A collection of existing nodes.</summary>
        public class ExistingNodeSet {
            private readonly Builder builder;
            private readonly HashSet<INode> nodes;

            /// <summary>Creates a new existing node set.</summary>
            /// <param name="builder">The builder this et belongs to.</param>
            internal ExistingNodeSet(Builder builder) {
                this.builder  = builder;
                this.nodes = new HashSet<INode>();
            }

            /// <summary>Clears the set of existing nodes.</summary>
            public void Clear() => this.nodes.Clear();

            /// <summary>Adds an existing node which has been referenced since the last clear.</summary>
            /// <param name="node">The existing node to add.</param>
            public void Add(INode node) {
                this.builder.Logger?.Log("Add Existing: {0} ", node);
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

        /// <summary>The set of existing nodes which have been references by new nodes.</summary>
        public readonly ExistingNodeSet Existing;

        #endregion
        #region Helper Methods...

        /// <summary>Performs a cast if needed from one value to another by creating a new node.</summary>
        /// <remarks>If a cast is added then the cast will be added to the builder as a new node.</remarks>
        /// <param name="type">The type to cast the value to.</param>
        /// <param name="value">The value to cast to the given type.</param>
        /// <param name="explicitCasts">
        /// Indicates if explicit casts are allowed to be used when casting.
        /// If false then only inheritance or implicit casts will be used.
        /// </param>
        /// <returns>The cast value or the given value in the given type.</returns>
        public INode PerformCast(Type type, INode value, bool explicitCasts = false) {
            Type valueType = Type.TypeOf(value);
            TypeMatch match = type.Match(valueType, explicitCasts);
            if (!match.IsAnyCast)
                return match.IsMatch ? value :
                    throw new Exception("The value type can not be cast to the given type.").
                        With("Location", this.LastLocation).
                        With("Target", type).
                        With("Type", valueType).
                        With("Value", value);

            Namespace ops = this.Slate.Global.Find(Slate.OperatorNamespace) as Namespace;
            INode castGroup =
                type == Type.Bool    ? ops.Find("castBool") :
                type == Type.Int     ? ops.Find("castInt") :
                type == Type.Double  ? ops.Find("castDouble") :
                type == Type.String  ? ops.Find("castString") :
                type == Type.Trigger ? ops.Find("castTrigger") :
                throw new Exception("Unsupported type for new definition cast").
                    With("Location", this.LastLocation).
                    With("Type", type);

            INode castValue = (castGroup as IFuncGroup).Build(value);;
            return castValue;
        }

        /// <summary>Prepares the tree from the given node up to the old nodes from the builder.</summary>
        /// <param name="root">The root node of the tree to prepare.</param>
        /// <returns>All the nodes which are new node in the tree.</returns>
        public IEnumerable<INode> PrepareTree(INode root) {
            HashSet<INode> newNodes = new();

            // Find all new nodes
            Stack<INode> stack = new();
            stack.Push(root);
            while (stack.Count > 0) {
                INode cur = stack.Pop();
                if (newNodes.Contains(cur) || this.Existing.Has(cur)) continue;
                newNodes.Add(cur);
                if (cur is IChild child) {
                    foreach (IParent par in child.Parents.NotNull()) {
                        par.AddChildren(child); // TODO: Rework to sort using first found parent and depth first.
                        stack.Push(par);
                    }
                }
            }
            this.Existing.Clear();

            // Update Depths
            LinkedList<IEvaluable> needsUpdate = new();
            needsUpdate.SortInsertUnique(root as IEvaluable);
            needsUpdate.SortInsertUnique(newNodes.OfType<IEvaluable>());
            needsUpdate.UpdateDepths();


            // TODO: Add Optimization

            return newNodes;
        }

        /// <summary>Creates an assignment action and adds it to the builder if possible.</summary>
        /// <param name="builder">The formula builder being used.</param>
        /// <param name="target">The node to assign the value to.</param>
        /// <param name="value">The value to assign to the given target node.</param>
        /// <returns>The cast value or given value which was used in the assignment.</returns>
        public INode AddAssignment(INode target, INode value) {
            // Check if the base types match. Don't need to check that the type is
            // a data type or trigger since only those can be reduced to constants.
            PP.Scanner.Location loc = this.LastLocation;
            Type targetType = Type.TypeOf(target);
            INode castValue = this.PerformCast(targetType, value);
            IEnumerable<INode> allNewNodes = this.PrepareTree(castValue);
            
            IAction assign =
                targetType == Type.Bool    ? Assign<Bool>.  Create(loc, target, castValue, allNewNodes) :
                targetType == Type.Int     ? Assign<Int>.   Create(loc, target, castValue, allNewNodes) :
                targetType == Type.Double  ? Assign<Double>.Create(loc, target, castValue, allNewNodes) :
                targetType == Type.String  ? Assign<String>.Create(loc, target, castValue, allNewNodes) :
                targetType == Type.Trigger ? Provoke.       Create(loc, target, castValue, allNewNodes) :
                throw new Exception("Unsupported type for an assignment").
                    With("Location", loc).
                    With("Type", targetType).
                    With("Input", target).
                    With("Value", value);

            this.Actions.Add(assign);
            return castValue;
        }

        /// <summary>Creates a new input node with the given name in the local scope.</summary>
        /// <param name="name">The name to create the input for.</param>
        /// <param name="type">The type of input to create.</param>
        /// <returns>The newly created input.</returns>
        public INode CreateInput(string name, Type type) {
            VirtualNode scope = this.Scope.Current;
            if (scope.ContainsField(name))
                throw new Exception("A node already exists with the given name.").
                    With("Name", name).
                    With("Type", type);

            INode node =
                type == Type.Bool    ? new InputValue<Bool>() :
                type == Type.Int     ? new InputValue<Int>() :
                type == Type.Double  ? new InputValue<Double>() :
                type == Type.String  ? new InputValue<String>() :
                type == Type.Trigger ? new InputTrigger() :
                throw new Exception("Unsupported type for new typed input").
                    With("Location", this.LastLocation).
                    With("Type", type);

            this.Actions.Add(new Define(scope.Receiver, name, node, Enumerable.Empty<INode>()));
            scope.WriteField(name, node);
            return node;
        }

        #endregion

        /// <summary>Gets the formula debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => this.StackString();

        /// <summary>Gets the formula debug string.</summary>
        /// <param name="showActions">Indicates that pending actions should be shown.</param>
        /// <param name="showGlobal">Indicates that the namespaces starting from the global should be shown.</param>
        /// <param name="showScope">Indicates that the scope stack should be shown.</param>
        /// <param name="showNodes">Indicates that the new node stack should be shown.</param>
        /// <param name="showTypes">Indicates that the type stack should be shown.</param>
        /// <param name="showIds">Indicates that the identifier stack should be shown.</param>
        /// <param name="showArguments">Indicates that the arguments stack should be shown.</param>
        /// <param name="showExisting">Indicates that the new nodes should be shown.</param>
        /// <returns>A human readable debug string.</returns>
        public string StackString(bool showActions = true, bool showGlobal = true, bool showScope = true,
            bool showNodes = true, bool showTypes = true, bool showIds = true, bool showArguments = true,
            bool showExisting = true) {
            const string indent = "  ";
            List<string> parts = new();
            if (showActions)   parts.Add("Actions: " + this.Actions.ToString(indent));
            if (showGlobal)    parts.Add("Global: " + this.Scope.Global.ToString());
            if (showScope)     parts.Add("Scope: " + this.Scope);
            if (showNodes)     parts.Add("Stack: " + this.Nodes.ToString(indent, false));
            if (showTypes)     parts.Add("Types: " + this.Types.ToString(indent, true));
            if (showIds)       parts.Add("Ids: " + this.Identifiers.ToString(indent, true));
            if (showArguments) parts.Add("Arguments: " + this.Arguments.ToString(indent));
            if (showExisting)  parts.Add("Existing: " + this.Existing.ToString(indent));
            return parts.Join("\n");
        }
    }
}
