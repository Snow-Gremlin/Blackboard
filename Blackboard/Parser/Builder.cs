using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using Blackboard.Parser.Optimization;
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
        public Builder(Slate slate, Logger? logger = null) {
            this.Slate  = slate;
            this.Logger = logger.SubGroup(nameof(Builder));

            this.Actions     = new ActionCollection(this);
            this.Scope       = new ScopeStack(this);
            this.Nodes       = new BuilderStack<INode>("Node", this);
            this.Types       = new BuilderStack<Type>("Type", this);
            this.Identifiers = new BuilderStack<string>("Id", this);
            this.Existing    = new ExistingNodeSet(this);
            this.Arguments   = new ArgumentStack(this);
            this.optimizer   = new Optimizer();
        }

        /// <summary>The slate for the Blackboard these stacks belongs too.</summary>
        public readonly Slate Slate;

        /// <summary>
        /// The logger to help debug the parser and builder.
        /// This may be null if no logger is being used.
        /// </summary>
        public readonly Logger? Logger;

        /// <summary>The optimizer being used to optimize nodes for new actions.</summary>
        private readonly Optimizer optimizer;

        /// <summary>Resets the stack back to the initial state.</summary>
        public void Reset() {
            this.Logger.Info("Reset");

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
            this.Logger.Info("Clear");

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
                this.builder.Logger.Info("Add Action: {0}", action);
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

            /// <summary>Finds the given ID in the current scopes.</summary>
            /// <param name="name">The name of the node to find.</param>
            /// <returns>The found node by that name or null if not found.</returns>
            public INode FindID(string name) {
                foreach (VirtualNode scope in this.Scopes) {
                    INode node = scope.ReadField(name);
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
                this.builder.Logger.Info("Push {0}: {1}", this.usage, value);
                this.stack.AddLast(value);
            }

            /// <summary>Peeks the value off the top of the stack without removing it.</summary>
            /// <returns>The value that is on the top of the stack.</returns>
            public T Peek() => this.stack.Last.Value;

            /// <summary>Pops off a value is on the top of the stack.</summary>
            /// <returns>The value which was on top of the stack.</returns>
            public T Pop() {
                this.builder.Logger.Info("Pop {0}", this.usage);
                T node = this.stack.Last.Value;
                this.stack.RemoveLast();
                return node;
            }

            /// <summary>Pops one or more values off the stack.</summary>
            /// <param name="count">The number of values to pop.</param>
            /// <returns>The popped values in the order oldest to newest.</returns>
            public T[] Pop(int count) {
                this.builder.Logger.Info("Pop {1} {0}(s)", this.usage, count);
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
                this.builder.Logger.Info("Start Arguments");
                this.argStacks.AddFirst(new LinkedList<INode>());
            }

            /// <summary>This adds the given node in to the newest argument list.</summary>
            /// <param name="node">The node to add to the argument list.</param>
            public void Add(INode node) {
                this.builder.Logger.Info("Add Argument: {0}", node);
                this.argStacks.First.Value.AddLast(node);
            }

            /// <summary>This gets all the nodes which are in the current argument list, then removes the list.</summary>
            /// <returns>The nodes which were in the current argument list.</returns>
            public INode[] End() {
                this.builder.Logger.Info("End Arguments");
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
                this.builder.Logger.Info("Add Existing: {0} ", node);
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
                    throw new Message("The value type can not be cast to the given type.").
                        With("Location", this.LastLocation).
                        With("Target",   type).
                        With("Type",     valueType).
                        With("Value",    value);

            IFuncGroup castGroup = Maker.GetCastMethod(this.Slate, type);
            if (castGroup is null)
                throw new Message("Unsupported type for new definition cast").
                    With("Location", this.LastLocation).
                    With("Type",     type);

            INode castValue = (castGroup as IFuncGroup).Build(value);
            return castValue;
        }

        /// <summary>Collects all the new nodes and apply depths.</summary>
        /// <param name="root">The root node of the branch to check.</param>
        /// <returns>The collection of new nodes.</returns>
        private HashSet<INode> collectAndOrder(INode root) {
            HashSet<INode> newNodes = new();
            this.collectAndOrder(root, newNodes);
            this.Existing.Clear();
            return newNodes;
        }

        /// <summary>Recessively collect the new nodes and apply depths.</summary>
        /// <param name="node">The current node to check.</param>
        /// <param name="newNodes">The set of new nodes being added.</param>
        /// <returns>True if a new node, false if not.</returns>
        private bool collectAndOrder(INode node, HashSet<INode> newNodes) {
            if (node is null || this.Existing.Has(node)) return false;
            if (newNodes.Contains(node)) return true;
            newNodes.Add(node);
            
            // Continue up to all the parents.
            if (node is IChild child) {
                foreach (IParent par in child.Parents) {
                    if (this.collectAndOrder(par, newNodes))
                        par.AddChildren(child);
                }
            }

            // Now that all parents are prepared, update the depth.
            // During optimization the depths may change from this but this initial depth
            // will help make all future depth updates perform efficiently.
            if (node is IEvaluable eval)
                eval.Depth = eval.MinimumAllowedDepth();
            return true;
        }

        /// <summary>Creates a define action and adds it to the builder.</summary>
        /// <param name="value">The value to define the node with.</param>
        /// <param name="type">The type of the node to define or null to use the value type.</param>
        /// <param name="name">The name to write the node with to the current scope.</param>
        /// <returns>The root of the value branch which was used in the assignment.</returns>
        public INode AddDefine(INode value, Type type, string name) {
            this.Logger.Info("Add Define:");
            INode root = type is null ? value : this.PerformCast(type, value);

            HashSet<INode> newNodes = this.collectAndOrder(root);
            root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);

            VirtualNode curScope = this.Scope.Current;
            this.Actions.Add(new Define(curScope.Receiver, name, root, newNodes));
            curScope.WriteField(name, root);
            return root;
        }

        /// <summary>Creates a trigger provoke action and adds it t the builder.</summary>
        /// <param name="target">The target trigger to effect.</param>
        /// <param name="value">The conditional value to trigger with or null if unconditional.</param>
        /// <returns>The root of the value branch which was used in the assignment.</returns>
        public INode AddProvokeTrigger(INode target, INode value) {
            this.Logger.Info("Add Provoke Trigger:");
            if (target is not ITriggerInput input)
                throw new Message("Target node is not an input trigger.").
                    With("Target",   target).
                    With("Value",    value).
                    With("Location", this.LastLocation);

            // If there is no condition, add an unconditional provoke.
            if (value is null) {
                IAction assign = Provoke.Create(target);
                if (assign is null)
                    throw new Message("Unexpected node types for a unconditional provoke.").
                        With("Location", this.LastLocation).
                        With("Target", target);

                this.Actions.Add(assign);
                return null;
            }

            INode root = this.PerformCast(Type.Trigger, value);
            HashSet<INode> newNodes = this.collectAndOrder(root);
            root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
            IAction condAssign = Provoke.Create(input, root, newNodes);
            if (condAssign is null)
                throw new Message("Unexpected node types for a conditional provoke.").
                   With("Location", this.LastLocation).
                   With("Target", target).
                   With("Value", value);

            this.Actions.Add(condAssign);
            return root;
        }

        /// <summary>Creates an assignment action and adds it to the builder if possible.</summary>
        /// <param name="target">The node to assign the value to.</param>
        /// <param name="value">The value to assign to the given target node.</param>
        /// <returns>The root of the value branch which was used in the assignment.</returns>
        public INode AddAssignment(INode target, INode value) {
            this.Logger.Info("Add Assignment:");
            if (target is not IInput)
                throw new Message("May not assign to a node which is not an input.").
                    With("Location", this.LastLocation).
                    With("Input", target).
                    With("Value", value);

            // Check if the base types match. Don't need to check that the type is
            // a data type or trigger since only those can be reduced to constants.
            Type targetType = Type.TypeOf(target);
            INode root = this.PerformCast(targetType, value);
            HashSet<INode> newNodes = this.collectAndOrder(root);
            root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
            IAction assign = Maker.CreateAssignAction(targetType, target, root, newNodes);
            if (assign is null)
                throw new Message("Unsupported types for an assignment action.").
                    With("Location", this.LastLocation).
                    With("Type",     targetType).
                    With("Input",    target).
                    With("Value",    value);

            this.Actions.Add(assign);
            return root;
        }

        /// <summary>Creates a getter action and adds it to the builder if possible.</summary>
        /// <param name="targetType">The target type of the value to get.</param>
        /// <param name="name">The name to output the value to.</param>
        /// <param name="value">The value to get and write to the given name.</param>
        /// <returns>The root of the value branch which was used in the assignment.</returns>
        public INode AddGetter(Type targetType, string name, INode value) {
            this.Logger.Info("Add Getter:");

            // Check if the base types match. Don't need to check that the type is
            // a data type or trigger since only those can be reduced to constants.
            INode root = this.PerformCast(targetType, value);
            HashSet<INode> newNodes = this.collectAndOrder(root);
            root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
            IAction getter = Maker.CreateGetterAction(targetType, name, root, newNodes);
            if (getter is null)
                throw new Message("Unsupported type for a getter action.").
                    With("Location", this.LastLocation).
                    With("Type",     targetType).
                    With("Name",     name).
                    With("Value",    value);

            this.Actions.Add(getter);
            return root;
        }

        /// <summary>Creates a new input node with the given name in the local scope.</summary>
        /// <param name="name">The name to create the input for.</param>
        /// <param name="type">The type of input to create.</param>
        /// <returns>The newly created input.</returns>
        public INode CreateInput(string name, Type type) {
            this.Logger.Info("Create Input:");

            VirtualNode scope = this.Scope.Current;
            if (scope.ContainsField(name))
                throw new Message("A node already exists with the given name.").
                    With("Name", name).
                    With("Type", type);

            IInput node = Maker.CreateInputNode(type);
            if (node is null)
                throw new Message("Unsupported type for new typed input").
                    With("Location", this.LastLocation).
                    With("Name",     name).
                    With("Type",     type);

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
            if (showActions)   parts.Add("Actions: "   + this.Actions.ToString(indent));
            if (showGlobal)    parts.Add("Global: "    + this.Scope.Global.ToString());
            if (showScope)     parts.Add("Scope: "     + this.Scope);
            if (showNodes)     parts.Add("Stack: "     + this.Nodes.ToString(indent, false));
            if (showTypes)     parts.Add("Types: "     + this.Types.ToString(indent, true));
            if (showIds)       parts.Add("Ids: "       + this.Identifiers.ToString(indent, true));
            if (showArguments) parts.Add("Arguments: " + this.Arguments.ToString(indent));
            if (showExisting)  parts.Add("Existing: "  + this.Existing.ToString(indent));
            return parts.Join("\n");
        }
    }
}
