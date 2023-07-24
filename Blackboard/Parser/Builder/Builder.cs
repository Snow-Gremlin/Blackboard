using Blackboard.Core;
using Blackboard.Core.Actions;
using Blackboard.Core.Extensions;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using Blackboard.Parser.Optimization;
using System.Collections.Generic;
using System.Linq;
using PP = PetiteParser;

namespace Blackboard.Parser.Builder;

/// <summary>This is a tool for keeping the parse state while building a formula.</summary>
/// <remarks>
/// It contains the set of actions pending to be performed on the Blackboard.
/// This holds onto virtual nodes being added and nodes virtually removed
/// prior to the actions being performed. 
/// </remarks>
sealed internal partial class Builder : PP.ParseTree.PromptArgs {

    /// <summary>Creates a new formula builder for parsing states.</summary>
    /// <param name="slate">The slate this stack is for.</param>
    /// <param name="logger">The optional logger to output the build steps.</param>
    public Builder(Slate slate, Logger? logger = null) {
        this.Slate  = slate;
        this.Logger = logger.SubGroup(nameof(Builder));

        this.Actions = new ActionCollection(this.Slate, this.Logger);
        this.Scope   = new ScopeStack(this.Slate, this.Logger);

        this.Nodes       = new BuilderStack<INode>("Node", this.Logger);
        this.Types       = new BuilderStack<Type>("Type", this.Logger);
        this.Identifiers = new BuilderStack<string>("Id", this.Logger);
        this.Existing    = new ExistingNodeSet(this.Logger);
        this.Arguments   = new ArgumentStack(this.Logger);
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

    /// <summary>The collection of actions which will perform the actions which have been parsed.</summary>
    public readonly ActionCollection Actions;

    /// <summary>The stack of the namespaces to represent the scope being worked on.</summary>
    public readonly ScopeStack Scope;

    /// <summary>The stack of nodes which are currently being parsed but haven't been consumed yet.</summary>
    public readonly BuilderStack<INode> Nodes;

    /// <summary>A stack of types which have been read during the parse.</summary>
    public readonly BuilderStack<Type> Types;

    /// <summary>A stack of identifiers which have been read but not used yet during the parse.</summary>
    public readonly BuilderStack<string> Identifiers;

    /// <summary>The stack of argument lists used for building up function calls.</summary>
    public readonly ArgumentStack Arguments;

    /// <summary>The set of existing nodes which have been references by new nodes.</summary>
    public readonly ExistingNodeSet Existing;

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
        Type? valueType = Type.TypeOf(value);
        TypeMatch match = type.Match(valueType, explicitCasts);
        if (!match.IsAnyCast)
            return match.IsMatch ? value :
                throw new Message("The value type can not be cast to the given type.").
                    With("Location", this.LastLocation).
                    With("Target",   type).
                    With("Type",     valueType).
                    With("Value",    value);

        IFuncGroup? castGroup = Maker.GetCastMethod(this.Slate, type);
        return castGroup?.Build(value) ??
            throw new Message("Unsupported type for new definition cast").
                With("Location", this.LastLocation).
                With("Type",     type);
    }

    /// <summary>Collects all the new nodes and apply depths.</summary>
    /// <param name="root">The root node of the branch to check.</param>
    /// <returns>The collection of new nodes.</returns>
    private HashSet<INode> collectAndOrder(INode? root) {
        HashSet<INode> newNodes = new();
        this.collectAndOrder(root, newNodes);
        this.Existing.Clear();
        return newNodes;
    }

    /// <summary>Recessively collect the new nodes and apply depths.</summary>
    /// <param name="node">The current node to check.</param>
    /// <param name="newNodes">The set of new nodes being added.</param>
    /// <returns>True if a new node, false if not.</returns>
    private bool collectAndOrder(INode? node, HashSet<INode> newNodes) {
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
    public INode AddDefine(INode value, Type? type, string name) {
        this.Logger.Info("Add Define:");
        INode root = type is null ? value : this.PerformCast(type, value);

        HashSet<INode> newNodes = this.collectAndOrder(root);
        root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
        
        VirtualNode scope = this.Scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {
            if (existing is not IExtern)
                throw new Message("A node already exists with the given name.").
                    With("Location", this.LastLocation).
                    With("Name",     name).
                    With("Type",     type);
            scope.RemoveFields(name);
        }

        if (root is IInput)
            root = Maker.CreateShell(root) ??
                throw new Message("The root for a define could not be shelled.").
                    With("Location", this.LastLocation).
                    With("Name",     name).
                    With("Root",     root);

        this.Actions.Add(new Define(scope.Receiver, name, root, newNodes));
        scope.WriteField(name, root);
        return root;
    }

    /// <summary>Creates a trigger provoke action and adds it to the builder.</summary>
    /// <param name="target">The target trigger to effect.</param>
    /// <param name="value">The conditional value to trigger with or null if unconditional.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode? AddProvokeTrigger(INode target, INode? value) {
        this.Logger.Info("Add Provoke Trigger:");
        if (target is not ITriggerInput input)
            throw new Message("Target node is not an input trigger.").
                With("Location", this.LastLocation).
                With("Target",   target).
                With("Value",    value);

        // If there is no condition, add an unconditional provoke.
        if (value is null) {
            IAction assign = Provoke.Create(target) ??
                throw new Message("Unexpected node types for a unconditional provoke.").
                    With("Location", this.LastLocation).
                    With("Target",   target);

            this.Actions.Add(assign);
            return null;
        }

        INode root = this.PerformCast(Type.Trigger, value);
        HashSet<INode> newNodes = this.collectAndOrder(root);
        root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
        IAction condAssign = Provoke.Create(input, root, newNodes) ??
            throw new Message("Unexpected node types for a conditional provoke.").
                With("Location", this.LastLocation).
                With("Target",   target).
                With("Value",    value);

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
                With("Input",    target).
                With("Value",    value);

        // Check if the base types match. Don't need to check that the type is
        // a data type or trigger since only those can be reduced to constants.
        Type targetType = Type.TypeOf(target) ??
            throw new Message("Unable to find target type.").
                With("Location", this.LastLocation).
                With("Input",    target).
                With("Value",    value);

        INode root = this.PerformCast(targetType, value);
        HashSet<INode> newNodes = this.collectAndOrder(root);
        root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
        IAction assign = Maker.CreateAssignAction(targetType, target, root, newNodes) ??
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
        INode? root = this.PerformCast(targetType, value);
        HashSet<INode> newNodes = this.collectAndOrder(root);
        root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);
        string[] names = this.Scope.Names.Append(name).ToArray();

        // TODO: Add a way to check if the getter by the names is already set or part of a path.

        IAction getter = Maker.CreateGetterAction(targetType, names, root, newNodes) ??
            throw new Message("Unsupported type for a getter action.").
                With("Location", this.LastLocation).
                With("Type",     targetType).
                With("Name",     names.Join(".")).
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

        IInput node = Maker.CreateInputNode(type) ??
            throw new Message("Unsupported type for new typed input").
                With("Location", this.LastLocation).
                With("Name",     name).
                With("Type",     type);

        VirtualNode scope = this.Scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {
            if (existing is not IExtern)
                throw new Message("A node already exists with the given name.").
                    With("Location", this.LastLocation).
                    With("Name",     name).
                    With("Type",     type);
            scope.RemoveFields(name);
        }

        this.Actions.Add(new Define(scope.Receiver, name, node));
        scope.WriteField(name, node);
        return node;
    }

    /// <summary>Creates a temporary node and adds it to the builder if possible.</summary>
    /// <param name="targetType">The target type of the value for the temporary value.</param>
    /// <param name="name">The name to output the value to.</param>
    /// <param name="value">The value to get and write to the given name.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode AddTemp(Type targetType, string name, INode value) {
        this.Logger.Info("Add Temp:");

        VirtualNode scope = this.Scope.Current;
        if (scope.ContainsField(name))
            throw new Message("A node already exists with the given name.").
                With("Location", this.LastLocation).
                With("Name",     name).
                With("Type",     targetType);

        INode root = targetType is null ? value : this.PerformCast(targetType, value);
        HashSet<INode> newNodes = this.collectAndOrder(root);
        root = this.optimizer.Optimize(this.Slate, root, newNodes, this.Logger);

        this.Actions.Add(new Temp(name, root, newNodes));
        this.Scope.Current.WriteField(name, root);
        return root;
    }

    /// <summary>
    /// Checks for an existing node and, if none exists, creates an extern node
    /// as a placeholder with the given name in the local scope.
    /// </summary>
    /// <param name="name">The name to create the extern for.</param>
    /// <param name="type">The type of extern to create.</param>
    /// <param name="value">The optional initial default value the extern node.</param>
    public void RequestExtern(string name, Type type, INode? value = null) {
        this.Logger.Info("Request Extern:");

        if (value is not null && type == Type.Trigger)
            throw new Message("May not initialize an extern trigger.").
                With("Location", this.LastLocation).
                With("Name",     name).
                With("Type",     type);

        VirtualNode scope = this.Scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {

            Type existType = Type.TypeOf(existing) ??
                throw new Message("Unable to find existing type.").
                    With("Location", this.LastLocation).
                    With("Name",     name).
                    With("Existing", existing).
                    With("New Type", type).
                    With("Value",    value);

            if (existType != type)
                throw new Message("Extern node does not match existing node type.").
                    With("Location",      this.LastLocation).
                    With("Name",          name).
                    With("Existing",      existing).
                    With("Existing Type", existType).
                    With("New Type",      type).
                    With("Value",         value);

            // Node already exists as an extern or the actual node.
            return;
        }

        // Node doesn't exist, create the extern placeholder.
        IExtern node = Maker.CreateExternNode(type) ??
            throw new Message("Unsupported type for new extern").
                With("Location", this.LastLocation).
                With("Name",     name).
                With("Type",     type);

        this.Actions.Add(new Extern(scope.Receiver, name, node));
        scope.WriteField(name, node);
        if (value is not null) this.AddAssignment(node, value);
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
