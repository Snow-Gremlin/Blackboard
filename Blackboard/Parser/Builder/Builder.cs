using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Formuila;
using Blackboard.Core.Formuila.Actions;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using PP = PetiteParser;
using S = System;

namespace Blackboard.Parser.Builder;

/// <summary>This is a tool for keeping the parse state while building a formula.</summary>
/// <remarks>
/// It contains the set of actions pending to be performed on the Blackboard.
/// This holds onto virtual nodes being added and nodes virtually removed
/// prior to the actions being performed. 
/// </remarks>
sealed internal class Builder : PP.ParseTree.PromptArgs {

    /// <summary>Creates a new formula builder for parsing states.</summary>
    /// <param name="slate">The slate this stack is for.</param>
    /// <param name="logger">The optional logger to output the build steps.</param>
    public Builder(Slate slate, Logger? logger = null) {
        this.Slate   = slate;
        this.Logger  = logger.SubGroup(nameof(Builder));
        this.factory = new Factory(this.Slate, this.Logger);

        this.Nodes       = new BuilderStack<INode>("Node", this.Logger);
        this.Types       = new BuilderStack<Type>("Type", this.Logger);
        this.Identifiers = new BuilderStack<string>("Id", this.Logger);
        this.Existing    = new ExistingNodeSet(this.Logger);
        this.Arguments   = new ArgumentStack(this.Logger);
    }

    /// <summary>The slate for the Blackboard these stacks belongs too.</summary>
    public readonly Slate Slate;

    /// <summary>
    /// The logger to help debug the parser and builder.
    /// This may be null if no logger is being used.
    /// </summary>
    public readonly Logger? Logger;

    /// <summary>The factory for formulas which will perform the actions that have been parsed.</summary>
    private readonly Factory factory;

    /// <summary>Resets the stack back to the initial state.</summary>
    public void Reset() {
        this.Logger.Info("Reset");

        this.factory.Reset();
        
        this.Tokens.Clear();
        this.Nodes.Clear();
        this.Types.Clear();
        this.Identifiers.Clear();
        this.Arguments.Clear();
        this.Existing.Clear();
    }


    /// <summary>Gets the formula containing all the actions.</summary>
    /// <returns>The formula containing all the actions.</returns>
    public Formula BuildFormula() => this.factory.Build();

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

    #region Handler Methods...

    /// <summary>Clears the node stack and type stack without changing pending actions nor scopes.</summary>
    public void Clear() {
        this.Logger.Info("Clear");

        this.Tokens.Clear();
        this.Nodes.Clear();
        this.Types.Clear();
        this.Identifiers.Clear();
        this.Arguments.Clear();
        this.Existing.Clear();
    }

    /// <summary>This is called when a new simple identifier has been defined.</summary>
    public void DefineId() =>
        this.Identifiers.Push(this.LastText);

    /// <summary>Pushes a namespace onto the scope stack.</summary>
    public void PushNamespace() {
        try {
            this.factory.PushNamespace(this.LastText);
        } catch (S.Exception inner) {
            throw new Message("Error parsing namespace").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>Pops a namespace off the scope stack.</summary>
    public void PopNamespace() => this.factory.PopNamespace();

    /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
    public void PushId() {
        try {
            string name = this.LastText;
            this.Logger.Info("Id = \"{0}\"", name);
            INode node = this.factory.FindInNamespace(name);
            this.Nodes.Push(node);
            this.Existing.Add(node);
        } catch (S.Exception inner) {
            throw new Message("Error parsing identifier").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
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

        this.factory.Add(new Define(scope.Receiver, name, root, newNodes));
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

            this.factory.Add(assign);
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

        this.factory.Add(condAssign);
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

        this.factory.Add(assign);
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

        this.factory.Add(getter);
        return root;
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

        this.factory.Add(new Temp(name, root, newNodes));
        this.Scope.Current.WriteField(name, root);
        return root;
    }










    
    /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
    public void NewTypeInputNoAssign() {
        try {
            string name = this.Identifiers.Pop();
            Type type   = this.Types.Peek();
            this.factory.CreateInput(name, type);
        } catch (S.Exception inner) {
            throw new Message("Error parsing input").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
    public void NewTypeInputWithAssign() {
        try {
            INode  value  = this.Nodes.Pop();
            Type   type   = this.Types.Peek();
            string name   = this.Identifiers.Pop();
            INode  target = this.factory.CreateInput(name, type);
            this.AddAssignment(target, value);
        } catch (S.Exception inner) {
            throw new Message("Error parsing input").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This creates a new input node and assigns it with an initial value.</summary>
    public void NewVarInputWithAssign() {
        try {
            INode  value = this.Nodes.Pop();
            string name  = this.Identifiers.Pop();
            Type   type  = Type.TypeOf(value) ??
                throw new Message("Unable to determine node type for new variable with assignment.");
            INode target = this.factory.CreateInput(name, type);
            this.AddAssignment(target, value);
        } catch (S.Exception inner) {
            throw new Message("Error parsing input").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>
    /// Checks for an existing node and, if none exists, creates an extern node
    /// as a placeholder with the given name in the local scope.
    /// </summary>
    public void RequestExtern() {
        try {
            string name = this.Identifiers.Pop();
            Type   type = this.Types.Peek();
            this.factory.RequestExtern(name, type);
        } catch (S.Exception inner) {
            throw new Message("Error parsing extern").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>
    /// Checks for an existing node and, if none exists, creates an extern node
    /// as a placeholder with the given name in the local scope.
    /// If the external node is created it will be assigned to a default value.
    /// </summary>
    public void RequestExternWithAssign() {
        try {
            INode  value = this.Nodes.Pop();
            Type   type  = this.Types.Peek();
            string name  = this.Identifiers.Pop();

            if (type == Type.Trigger)
                throw new Message("May not initialize an extern trigger.").
                    With("Name", name).
                    With("Type", type);

            (INode node, bool isExtern) = this.factory.RequestExtern(name, type);
            if (isExtern) this.AddAssignment(node, value);
        } catch (S.Exception inner) {
            throw new Message("Error parsing extern").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }
    
    /// <summary>A helper handler for parsing literals.</summary>
    /// <param name="usage">The string describing what this parse is doing.</param>
    /// <param name="parseMethod">The method to convert a string into a literal node.</param>
    private void parseLiteral(string usage, S.Func<string, INode> parseMethod) {
        string text = this.LastText;
        try {
            this.Nodes.Push(parseMethod(text));
        } catch (S.Exception ex) {
            throw new Message("Failed to " + usage + ".").
                With("Error", ex).
                With("Text", text).
                With("Location", this.LastLocation);
        }
    }

    /// <summary>This handles pushing a bool literal value onto the stack.</summary>
    public void PushBool() =>
        this.parseLiteral("parse a bool", (string text) => Literal.Bool(bool.Parse(text)));

    /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
    public void PushBin() =>
        this.parseLiteral("parse a binary int", (string text) => Literal.Int(S.Convert.ToInt32(text, 2)));

    /// <summary>This handles pushing an octal int literal value onto the stack.</summary>
    public void PushOct() =>
        this.parseLiteral("parse a octal int", (string text) => Literal.Int(S.Convert.ToInt32(text, 8)));

    /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
    public void PushInt() =>
        this.parseLiteral("parse a decimal int", (string text) => Literal.Int(int.Parse(text)));

    /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
    public void PushHex() =>
        this.parseLiteral("parse a hex int", (string text) => Literal.Int(int.Parse(text[2..], NumberStyles.HexNumber)));

    /// <summary>This handles pushing a double literal value onto the stack.</summary>
    public void PushDouble() =>
        this.parseLiteral("parse a double", (string text) => Literal.Double(double.Parse(text)));

    /// <summary>This handles pushing a string literal value onto the stack.</summary>
    public void PushString() =>
        this.parseLiteral("decode escaped sequences", (string text) => Literal.String(PP.Formatting.Text.Unescape(text)));

    /// <summary>This handles pushing a type onto the stack.</summary>
    public void PushType() {
        string text = this.LastText;
        Type t = Type.FromName(text) ??
            throw new Message("Unrecognized type name.").
                With("Text",     text).
                With("Location", this.LastLocation);
        this.Types.Push(t);
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
        if (showActions || showGlobal || showScope)
            parts.Add(this.factory.StackString(showActions, showGlobal, showScope));
        if (showNodes)     parts.Add("Stack: "     + this.Nodes.ToString(indent, false));
        if (showTypes)     parts.Add("Types: "     + this.Types.ToString(indent, true));
        if (showIds)       parts.Add("Ids: "       + this.Identifiers.ToString(indent, true));
        if (showArguments) parts.Add("Arguments: " + this.Arguments.ToString(indent));
        if (showExisting)  parts.Add("Existing: "  + this.Existing.ToString(indent));
        return parts.Join("\n");
    }
}
