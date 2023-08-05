using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Formuila;
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

    #region Helper Methods...
    
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

    #endregion
    #region Handler Methods...

    /// <summary>Clears the node stack and type stack without changing pending actions nor scopes.</summary>
    public void HandleClear() {
        try {
            this.Logger.Info("Clear");
            this.Tokens.Clear();
            this.Nodes.Clear();
            this.Types.Clear();
            this.Identifiers.Clear();
            this.Arguments.Clear();
            this.Existing.Clear();
        } catch (S.Exception inner) {
            throw new Message("Error clearing stacks at end of command").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This is called when a new simple identifier has been defined.</summary>
    public void HandleDefineId() {
        try {
            this.Identifiers.Push(this.LastText);
        } catch (S.Exception inner) {
            throw new Message("Error defining identifier").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>Pushes a namespace onto the scope stack.</summary>
    public void HandlePushNamespace() {
        try {
            this.factory.PushNamespace(this.LastText);
        } catch (S.Exception inner) {
            throw new Message("Error parsing namespace").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>Pops a namespace off the scope stack.</summary>
    public void HandlePopNamespace() {
        try {
            this.factory.PopNamespace();
        } catch (S.Exception inner) {
            throw new Message("Error closing namespace").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }













    



    /// <summary>This handles defining a new typed named node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeDefine(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        Type   type  = builder.Types.Peek();
        string name  = builder.Identifiers.Pop();
        builder.AddDefine(value, type, name);
    }

    /// <summary>This handles defining a new untyped named node.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarDefine(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        string name  = builder.Identifiers.Pop();
        builder.AddDefine(value, null, name);
    }

    /// <summary>This handles when a trigger is provoked unconditionally.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleProvokeTrigger(Builder.Builder builder) {
        INode target = builder.Nodes.Pop();
        builder.AddProvokeTrigger(target, null);
    }

    /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleConditionalProvokeTrigger(Builder.Builder builder) {
        INode target = builder.Nodes.Pop();
        INode value  = builder.Nodes.Pop();
        INode root   = builder.AddProvokeTrigger(target, value) ??
            throw new Message("Unable to create conditional provoke trigger");

        // Push the condition onto the stack for any following trigger pulls.
        // See comment in `handleAssignment` about pushing cast value back onto the stack.
        builder.Nodes.Push(root);
        builder.Existing.Add(root);
    }

    /// <summary>This handles getting the typed left value and writing it out to the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeGet(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        Type   type  = builder.Types.Peek();
        string name  = builder.Identifiers.Pop();
        builder.AddGetter(type, name, value);
    }

    /// <summary>This handles getting the variable type left value and writing it out to the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarGet(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        string name  = builder.Identifiers.Pop();
        Type   type  = Type.TypeOf(value) ??
            throw new Message("Unable to determine node type for getter.");
        builder.AddGetter(type, name, value);
    }

    /// <summary>This handles getting the typed left value as a temporary value with the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleTypeTemp(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        Type   type  = builder.Types.Peek();
        string name  = builder.Identifiers.Pop();
        builder.AddTemp(type, name, value);
    }

    /// <summary>This handles getting the variable type left value as a temporary value with the given name.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    static private void handleVarTemp(Builder.Builder builder) {
        INode  value = builder.Nodes.Pop();
        string name  = builder.Identifiers.Pop();
        Type   type  = Type.TypeOf(value) ??
            throw new Message("Unable to determine node type for temp.");
        builder.AddTemp(type, name, value);
    }
    



















    
    /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
    public void HandleNewTypeInputNoAssign() {
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
    public void HandleNewTypeInputWithAssign() {
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
    public void HandleNewVarInputWithAssign() {
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
    public void HandleExternNoAssign() {
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
    public void HandleExternWithAssign() {
        try {
            INode  value = this.Nodes.Pop();
            Type   type  = this.Types.Peek();
            string name  = this.Identifiers.Pop();

            if (type == Type.Trigger)
                throw new Message("May not initialize an extern trigger.").
                    With("Name", name).
                    With("Type", type);

            (INode node, bool isExtern) = this.factory.RequestExtern(name, type);
            if (isExtern) {








                this.factory.AddAssignment(node, value);

            }
        } catch (S.Exception inner) {
            throw new Message("Error parsing extern").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles assigning the left value to the right value.</summary>
    public void HandleAssignment() {
        INode value  = this.Nodes.Pop();
        INode target = this.Nodes.Pop();



        INode root   = this.AddAssignment(target, value);

        // TODO: Reevaluate this statement and make sure we're doing it correctly.
        //
        // Push the cast value back onto the stack for any following assignments.
        // By using the cast it makes it more like `X=Y=Z` is `Y=Z; X=Y;` but means that if a double and an int are being set by
        // an int, the int must be assigned first then it can cast to a double for the double assignment, otherwise it will cast
        // to a double but not be able to implicitly cast that double back to an int. For example: if `int X; double Y;` then
        // `Y=X=3;` works and `X=Y=3` will not. One drawback for this way is that if you assign an int to multiple doubles it will
        // construct multiple cast nodes, but with a little optimization to remove duplicate node paths, this isn't an issue. 
        // Alternatively, if we push the value then `X=Y=Z` will be like `Y=Z; X=Z;`, but we won't.
        this.Nodes.Push(root);
        // Add to existing since the first assignment will handle preparing the tree being assigned.
        this.Existing.Add(root);
    }

    /// <summary>This handles performing a type cast of a node.</summary>
    public void HandleCast() {
        INode value = this.Nodes.Pop();
        Type  type  = this.Types.Pop();
        this.Nodes.Push(this.factory.PerformCast(type, value, true));
    }

    /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
    /// <param name="builder">The formula builder being worked on.</param>
    public void HandleMemberAccess() {
        string name  = this.LastText;
        INode  rNode = this.Nodes.Pop();
        if (rNode is not IFieldReader receiver)
            throw new Message("Unexpected node type for a member access. Expected a field reader.").
                With("Node", rNode).
                With("Name", name).
                With("Location", this.LastLocation);

        INode node = receiver.ReadField(name) ??
            throw new Message("No identifier found in the receiver stack.").
                With("Identifier", name).
                With("Location", this.LastLocation);

        if (node is IExtern externNode)
            node = externNode.Shell;

        this.Nodes.Push(node);
    }

    /// <summary>This handles preparing for a method call.</summary>
    public void HandleStartCall() =>
        this.Arguments.Start();

    /// <summary>This handles the end of a method call and creates the node for the method.</summary>
    public void HandleAddArg() =>
        this.Arguments.Add(this.Nodes.Pop());

    /// <summary>This handles finishing a method call and building the node for the method.</summary>
    public void HandleEndCall() {
        INode[] args = this.Arguments.End();
        INode   node = this.Nodes.Pop();
        if (node is not IFuncGroup group)
            throw new Message("Unexpected node type for a method call. Expected a function group.").
                With("Node", node).
                With("Input", args.Types().Strings().Join(", ")).
                With("Location", this.LastLocation);

        INode? result = group.Build(args);
        if (result is null) {
            if (args.Length <= 0)
                throw new Message("Could not perform the function without any inputs.").
                    With("Function", group).
                    With("Location", this.LastLocation);
            throw new Message("Could not perform the function with the given input types.").
                With("Function", group).
                With("Input", args.Types().Strings().Join(", ")).
                With("Location", this.LastLocation);
        }

        this.Nodes.Push(result);
    }

    /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
    public void HandlePushId() {
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

    /// <summary>This handles pushing a bool literal value onto the stack.</summary>
    public void HandlePushBool() =>
        this.parseLiteral("parse a bool", (string text) => Literal.Bool(bool.Parse(text)));

    /// <summary>This handles pushing a binary int literal value onto the stack.</summary>
    public void HandlePushBin() =>
        this.parseLiteral("parse a binary int", (string text) => Literal.Int(S.Convert.ToInt32(text, 2)));

    /// <summary>This handles pushing an octal int literal value onto the stack.</summary>
    public void HandlePushOct() =>
        this.parseLiteral("parse a octal int", (string text) => Literal.Int(S.Convert.ToInt32(text, 8)));

    /// <summary>This handles pushing a decimal int literal value onto the stack.</summary>
    public void HandlePushInt() =>
        this.parseLiteral("parse a decimal int", (string text) => Literal.Int(int.Parse(text)));

    /// <summary>This handles pushing a hexadecimal int literal value onto the stack.</summary>
    public void HandlePushHex() =>
        this.parseLiteral("parse a hex int", (string text) => Literal.Int(int.Parse(text[2..], NumberStyles.HexNumber)));

    /// <summary>This handles pushing a double literal value onto the stack.</summary>
    public void HandlePushDouble() =>
        this.parseLiteral("parse a double", (string text) => Literal.Double(double.Parse(text)));

    /// <summary>This handles pushing a string literal value onto the stack.</summary>
    public void HandlePushString() =>
        this.parseLiteral("decode escaped sequences", (string text) => Literal.String(PP.Formatting.Text.Unescape(text)));

    /// <summary>This handles pushing a type onto the stack.</summary>
    public void HandlePushType() {
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
