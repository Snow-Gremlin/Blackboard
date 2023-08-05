﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Formuila.Actions;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Optimization;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Formuila;

/// <summary>The collection of actions which have been parsed.</summary>
sealed public class Factory {
    private readonly Slate slate;
    private readonly Logger? logger;
    private readonly LinkedList<IAction> actions;
    private readonly ScopeStack scope;
    private readonly Optimizer optimizer;

    /// <summary>Creates new a new action collection.</summary>
    /// <param name="slate">The slate to create the formula for.</param>
    /// <param name="logger">The optional logger to write debugging information to.</param>
    internal Factory(Slate slate, Logger? logger = null) {
        this.slate     = slate;
        this.logger    = logger;
        this.actions   = new LinkedList<IAction>();
        this.scope     = new ScopeStack(this.slate, this.logger);
        this.optimizer = new Optimizer();
    }

    /// <summary>Clears and resets the collection of actions.</summary>
    public void Reset() {
        this.actions.Clear();
        this.scope.Reset();
    }

    /// <summary>Gets the formula containing all the actions.</summary>
    /// <returns>The formula containing all the actions.</returns>
    public Formula Build() => new(this.slate, this.actions.Append(new Finish()));

    /// <summary>Adds a pending action into this formula.</summary>
    /// <param name="performer">The performer to add.</param>
    private void add(IAction action) {
        this.logger.Info("Add Action: {0}", action);
        this.actions.AddLast(action);
    }

    /// <summary>Pushes a namespace onto the scope stack.</summary>
    /// <param name="name">The name of the namespace to push onto the namespace stack.</param>
    public void PushNamespace(string name) {
        VirtualNode scope = this.scope.Current;

        // Check if the virtual namespace already exists.
        INode? next = scope.ReadField(name);
        if (next is not null) {
            if (next is not VirtualNode nextspace)
                throw new Message("Can not open namespace. Another non-namespace exists by that name.").
                     With("Identifier", name);
            this.scope.Push(nextspace);
            return;
        }

        // Create a new virtual namespace and an action to define the new namespace when this formula is run.
        Namespace newspace = new();
        this.add(new Define(scope.Receiver, name, newspace)); // TODO: Remove action creation from parser
        VirtualNode nextScope = new(name, newspace);
        this.scope.Push(nextScope);
        scope.WriteField(name, nextScope);
    }

    /// <summary>Pops a namespace off the scope stack.</summary>
    public void PopNamespace() => this.scope.Pop();

    /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
    /// <param name="name"></param>
    public INode FindInNamespace(string name) {
        INode node = this.scope.FindId(name) ??
            throw new Message("No identifier found in the scope stack.").
                With("Identifier", name);
        return node is IExtern externNode ?
            externNode.Shell :
            node;
    }

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
                    With("Target",   type).
                    With("Type",     valueType).
                    With("Value",    value);

        IFuncGroup? castGroup = Maker.GetCastMethod(this.slate, type);
        return castGroup?.Build(value) ??
            throw new Message("Unsupported type for new definition cast").
                With("Type",     type);
    }

    /// <summary>Creates a define action and adds it to the builder.</summary>
    /// <param name="value">The value to define the node with.</param>
    /// <param name="type">The type of the node to define or null to use the value type.</param>
    /// <param name="name">The name to write the node with to the current scope.</param>
    /// <param name="newNodes">Any new nodes that were created for this action.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode AddDefine(INode value, Type? type, string name, HashSet<INode> newNodes) {
        this.logger.Info("Add Define:");
        INode root = type is null ? value : this.PerformCast(type, value);
        root = this.optimizer.Optimize(this.slate, root, newNodes, this.logger);
        
        VirtualNode scope = this.scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {
            if (existing is not IExtern)
                throw new Message("A node already exists with the given name.").
                    With("Name", name).
                    With("Type", type);
            scope.RemoveFields(name);
        }

        if (root is IInput)
            root = Maker.CreateShell(root) ??
                throw new Message("The root for a define could not be shelled.").
                    With("Name", name).
                    With("Root", root);

        this.add(new Define(scope.Receiver, name, root, newNodes));
        scope.WriteField(name, root);
        return root;
    }

    /// <summary>Creates a trigger provoke action and adds it to the builder.</summary>
    /// <param name="target">The target trigger to effect.</param>
    /// <param name="value">The conditional value to trigger with or null if unconditional.</param>
    /// <param name="newNodes">Any new nodes that were created for this action.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode? AddProvokeTrigger(INode target, INode? value, HashSet<INode> newNodes) {
        this.logger.Info("Add Provoke Trigger:");
        if (target is not ITriggerInput input)
            throw new Message("Target node is not an input trigger.").
                With("Target", target).
                With("Value",  value);

        // If there is no condition, add an unconditional provoke.
        if (value is null) {
            IAction assign = Provoke.Create(target) ??
                throw new Message("Unexpected node types for a unconditional provoke.").
                    With("Target", target);

            this.add(assign);
            return null;
        }

        INode root = this.PerformCast(Type.Trigger, value);
        root = this.optimizer.Optimize(this.slate, root, newNodes, this.logger);
        IAction condAssign = Provoke.Create(input, root, newNodes) ??
            throw new Message("Unexpected node types for a conditional provoke.").
                With("Target", target).
                With("Value",  value);

        this.add(condAssign);
        return root;
    }

    /// <summary>Creates an assignment action and adds it to the builder if possible.</summary>
    /// <param name="target">The node to assign the value to.</param>
    /// <param name="value">The value to assign to the given target node.</param>
    /// <param name="newNodes">Any new nodes that were created for this action.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode AddAssignment(INode target, INode value, HashSet<INode> newNodes) {
        this.logger.Info("Add Assignment:");
        if (target is not IInput)
            throw new Message("May not assign to a node which is not an input.").
                With("Input", target).
                With("Value", value);

        // Check if the base types match. Don't need to check that the type is
        // a data type or trigger since only those can be reduced to constants.
        Type targetType = Type.TypeOf(target) ??
            throw new Message("Unable to find target type.").
                With("Input", target).
                With("Value", value);

        INode root = this.PerformCast(targetType, value);
        root = this.optimizer.Optimize(this.slate, root, newNodes, this.logger);
        IAction assign = Maker.CreateAssignAction(targetType, target, root, newNodes) ??
            throw new Message("Unsupported types for an assignment action.").
                With("Type",  targetType).
                With("Input", target).
                With("Value", value);

        this.add(assign);
        return root;
    }

    /// <summary>Creates a getter action and adds it to the builder if possible.</summary>
    /// <param name="targetType">The target type of the value to get.</param>
    /// <param name="name">The name to output the value to.</param>
    /// <param name="value">The value to get and write to the given name.</param>
    /// <param name="newNodes">Any new nodes that were created for this action.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode AddGetter(Type targetType, string name, INode value, HashSet<INode> newNodes) {
        this.logger.Info("Add Getter:");

        // Check if the base types match. Don't need to check that the type is
        // a data type or trigger since only those can be reduced to constants.
        INode? root = this.PerformCast(targetType, value);
        root = this.optimizer.Optimize(this.slate, root, newNodes, this.logger);
        string[] names = this.scope.Names.Append(name).ToArray();

        // TODO: Add a way to check if the getter by the names is already set or part of a path.

        IAction getter = Maker.CreateGetterAction(targetType, names, root, newNodes) ??
            throw new Message("Unsupported type for a getter action.").
                With("Type",  targetType).
                With("Name",  names.Join(".")).
                With("Value", value);

        this.add(getter);
        return root;
    }

    /// <summary>Creates a temporary node and adds it to the builder if possible.</summary>
    /// <param name="targetType">The target type of the value for the temporary value.</param>
    /// <param name="name">The name to output the value to.</param>
    /// <param name="value">The value to get and write to the given name.</param>
    /// <param name="newNodes">Any new nodes that were created for this action.</param>
    /// <returns>The root of the value branch which was used in the assignment.</returns>
    public INode AddTemp(Type targetType, string name, INode value, HashSet<INode> newNodes) {
        this.logger.Info("Add Temp:");

        VirtualNode scope = this.scope.Current;
        if (scope.ContainsField(name))
            throw new Message("A node already exists with the given name.").
                With("Name", name).
                With("Type", targetType);

        INode root = targetType is null ? value : this.PerformCast(targetType, value);
        root = this.optimizer.Optimize(this.slate, root, newNodes, this.logger);

        this.add(new Temp(name, root, newNodes));
        this.scope.Current.WriteField(name, root);
        return root;
    }

    /// <summary>Creates a new input node with the given name in the local scope.</summary>
    /// <param name="name">The name to create the input for.</param>
    /// <param name="type">The type of input to create.</param>
    /// <returns>The newly created input.</returns>
    public INode CreateInput(string name, Type type) {
        this.logger.Info("Create Input:");

        IInput node = Maker.CreateInputNode(type) ??
            throw new Message("Unsupported type for new typed input").
                With("Name", name).
                With("Type", type);

        VirtualNode scope = this.scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {
            if (existing is not IExtern)
                throw new Message("A node already exists with the given name.").
                    With("Name", name).
                    With("Type", type);
            scope.RemoveFields(name);
        }

        this.add(new Define(scope.Receiver, name, node));
        scope.WriteField(name, node);
        return node;
    }

    /// <summary>
    /// Checks for an existing node and, if none exists, creates an extern node
    /// as a placeholder with the given name in the local scope.
    /// </summary>
    /// <param name="name">The name to create the extern for.</param>
    /// <param name="type">The type of extern to create.</param>
    /// <returns>The existing with false, or the newly created extern node with true.</returns>
    public (INode, bool) RequestExtern(string name, Type type) {
        this.logger.Info("Request Extern:");
        VirtualNode scope = this.scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {

            Type existType = Type.TypeOf(existing) ??
                throw new Message("Unable to find existing type.").
                    With("Name",     name).
                    With("Existing", existing).
                    With("New Type", type);

            if (existType != type)
                throw new Message("Extern node does not match existing node type.").
                    With("Name",          name).
                    With("Existing",      existing).
                    With("Existing Type", existType).
                    With("New Type",      type);

            // Node already exists as an extern or the actual node.
            return (existing, false);
        }

        // Node doesn't exist, create the extern placeholder.
        IExtern node = Maker.CreateExternNode(type) ??
            throw new Message("Unsupported type for new extern").
                With("Name", name).
                With("Type", type);

        this.add(new Extern(scope.Receiver, name, node));
        scope.WriteField(name, node);
        return (node, true);
    }

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => this.StackString();

    /// <summary>Gets the human readable string of the current actions.</summary>
    /// <param name="indent">The indent to apply to all but the first line being returned.</param>
    /// <returns>The human readable string.</returns>
    public string ActionsToString(string indent) =>
        this.actions.Count <= 0 ? "[]" :
        "[\n" + indent + this.actions.Strings().Indent(indent).Join(",\n" + indent) + "]";

    /// <summary>Gets the formula debug string.</summary>
    /// <param name="showActions">Indicates that pending actions should be shown.</param>
    /// <param name="showGlobal">Indicates that the namespaces starting from the global should be shown.</param>
    /// <param name="showScope">Indicates that the scope stack should be shown.</param>
    /// <returns>A human readable debug string.</returns>
    public string StackString(bool showActions = true, bool showGlobal = true, bool showScope = true) {
        const string indent = "  ";
        List<string> parts = new();
        if (showActions)   parts.Add("Actions: " + this.ActionsToString(indent));
        if (showGlobal)    parts.Add("Global: "  + this.scope.Global.ToString());
        if (showScope)     parts.Add("Scope: "   + this.scope);
        return parts.Join("\n");
    }
}
