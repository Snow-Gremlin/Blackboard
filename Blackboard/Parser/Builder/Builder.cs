using Blackboard.Core;
using Blackboard.Core.Extensions;
using Blackboard.Core.Formula.Factory;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using S = System;

namespace Blackboard.Parser.Builder;

/// <summary>This is a tool for keeping the parse state while building a formula.</summary>
/// <remarks>
/// It contains the set of actions pending to be performed on the Blackboard.
/// This holds onto virtual nodes being added and nodes virtually removed
/// prior to the actions being performed. 
/// </remarks>
sealed internal class Builder : PetiteParser.ParseTree.PromptArgs {

    /// <summary>Creates a new formula builder for parsing states.</summary>
    /// <param name="factory">The factory this builder writes to.</param>
    public Builder(Factory factory) {
        this.factory     = factory;
        this.nodes       = new();
        this.types       = new();
        this.identifiers = new();
        this.arguments   = new();
    }

    /// <summary>The factory for formulas which will perform the actions that have been parsed.</summary>
    private readonly Factory factory;

    /// <summary>The stack of nodes which are currently being parsed but haven't been consumed yet.</summary>
    private readonly BuilderStack<INode> nodes;

    /// <summary>A stack of types which have been read during the parse.</summary>
    private readonly BuilderStack<Type> types;

    /// <summary>A stack of identifiers which have been read but not used yet during the parse.</summary>
    private readonly BuilderStack<string> identifiers;

    /// <summary>The stack of argument lists used for building up function calls.</summary>
    private readonly ArgumentStack arguments;

    /// <summary>Resets the stack back to the initial state.</summary>
    public void Reset() {
        this.factory.Reset();
        this.nodes.Clear();
        this.types.Clear();
        this.identifiers.Clear();
        this.arguments.Clear();
    }

    #region Helper Methods...
    
    /// <summary>A helper handler for parsing literals.</summary>
    /// <param name="usage">The string describing what this parse is doing.</param>
    /// <param name="parseMethod">The method to convert a string into a literal node.</param>
    private void parseLiteral(string usage, S.Func<string, INode> parseMethod) {
        string text = this.LastText;
        try {
            this.nodes.Push(parseMethod(text));
        } catch (S.Exception ex) {
            throw new BlackboardException("Failed to " + usage + ".").
                With("Location", this.LastLocation).
                With("Error",    ex).
                With("Text",     text);
        }
    }

    #endregion
    #region Handler Methods...

    /// <summary>Handles processing a function by the given name with the given number of arguments.</summary>
    /// <param name="count">The number of arguments for the function.</param>
    /// <param name="name">The name of the function that was given.</param>
    /// <param name="funcGroup">The function to perform.</param>
    public void HandleProcesses(int count, string name, IFuncGroup funcGroup) {
        INode[] inputs = this.nodes.Pop(count).Actualize().ToArray();
        INode result = funcGroup.Build(inputs) ??
                throw new BlackboardException("Could not perform the operation with the given input.").
                    With("Location",  this.LastLocation).
                    With("Operation", name).
                    With("Input",     inputs.Types().Strings().Join(", "));

        this.nodes.Push(result);
    }

    /// <summary>Clears the node stack and type stack without changing pending actions nor scopes.</summary>
    public void HandleClear() {
        try {
            this.Tokens.Clear();
            this.nodes.Clear();
            this.types.Clear();
            this.identifiers.Clear();
            this.arguments.Clear();
        } catch (S.Exception inner) {
            throw new BlackboardException("Error clearing stacks at end of command").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This is called when a new simple identifier has been defined.</summary>
    public void HandleDefineId() {
        try {
            this.identifiers.Push(this.LastText);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error defining identifier").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>Pushes a namespace onto the scope stack.</summary>
    public void HandlePushNamespace() {
        try {
            this.factory.PushNamespace(this.LastText);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing namespace").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>Pops a namespace off the scope stack.</summary>
    public void HandlePopNamespace() {
        try {
            this.factory.PopNamespace();
        } catch (S.Exception inner) {
            throw new BlackboardException("Error closing namespace").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles defining a new typed named node.</summary>
    public void HandleTypeDefine() {
        try {
            INode  value = this.nodes.Pop();
            Type   type  = this.types.Peek();
            string name  = this.identifiers.Pop();
            this.factory.AddDefine(value, type, name);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error creating a typed define").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles defining a new untyped named node.</summary>
    public void HandleVarDefine() {
        try {
            INode  value = this.nodes.Pop();
            string name  = this.identifiers.Pop();
            this.factory.AddDefine(value, null, name);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error creating a variable typed define").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles when a trigger is provoked unconditionally.</summary>
    public void HandleProvokeTrigger() {
        try {
            INode target = this.nodes.Pop();
            this.factory.AddProvokeTrigger(target, null);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error provoking a trigger").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles when a trigger should only be provoked if a condition returns true.</summary>
    public void HandleConditionalProvokeTrigger() {
        try {
            INode target = this.nodes.Pop();
            INode value  = this.nodes.Pop();
            INode root   = this.factory.AddProvokeTrigger(target, value) ??
                throw new BlackboardException("Unable to create conditional provoke trigger");

            // Push the condition onto the stack for any following trigger pulls.
            // See comment in `handleAssignment` about pushing cast value back onto the stack.
            this.nodes.Push(root);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error conditionally provoking a trigger").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles getting the typed left value and writing it out to the given name.</summary>
    public void HandleTypeGet() {
        try {
            INode  value = this.nodes.Pop();
            Type   type  = this.types.Peek();
            string name  = this.identifiers.Pop();
            this.factory.AddGetter(type, name, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error getting typed value").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles getting the variable type left value and writing it out to the given name.</summary>
    public void HandleVarGet() {
        try {
            INode  value = this.nodes.Pop();
            string name  = this.identifiers.Pop();
            Type   type  = Type.TypeOf(value) ??
                throw new BlackboardException("Unable to determine node type for getter.");
            this.factory.AddGetter(type, name, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error getting variable typed value").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles getting the typed left value as a temporary value with the given name.</summary>
    public void HandleTypeTemp() {
        try {
            INode  value = this.nodes.Pop();
            Type   type  = this.types.Peek();
            string name  = this.identifiers.Pop();
            this.factory.AddTemp(type, name, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error creating a typed temp").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles getting the variable type left value as a temporary value with the given name.</summary>
    public void HandleVarTemp() {
        try {
            INode  value = this.nodes.Pop();
            string name  = this.identifiers.Pop();
            Type   type  = Type.TypeOf(value) ??
                throw new BlackboardException("Unable to determine node type for temp.");
            this.factory.AddTemp(type, name, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error creating a variable typed temp").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }
    
    /// <summary>This creates a new input node of a specific type without assigning the value.</summary>
    public void HandleNewTypeInputNoAssign() {
        try {
            string name = this.identifiers.Pop();
            Type   type = this.types.Peek();
            this.factory.CreateInput(name, type);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing input").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This creates a new input node of a specific type and assigns it with an initial value.</summary>
    public void HandleNewTypeInputWithAssign() {
        try {
            INode  value  = this.nodes.Pop();
            Type   type   = this.types.Peek();
            string name   = this.identifiers.Pop();
            INode  target = this.factory.CreateInput(name, type);
            this.factory.AddAssignment(target, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing input").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This creates a new input node and assigns it with an initial value.</summary>
    public void HandleNewVarInputWithAssign() {
        try {
            INode  value = this.nodes.Pop();
            string name  = this.identifiers.Pop();
            Type   type  = Type.TypeOf(value) ??
                throw new BlackboardException("Unable to determine node type for new variable with assignment.");
            INode target = this.factory.CreateInput(name, type);
            this.factory.AddAssignment(target, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing input").
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
            string name = this.identifiers.Pop();
            Type   type = this.types.Peek();
            this.factory.RequestExtern(name, type);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing extern").
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
            INode  value = this.nodes.Pop();
            Type   type  = this.types.Peek();
            string name  = this.identifiers.Pop();

            if (type == Type.Trigger)
                throw new BlackboardException("May not initialize an extern trigger.").
                    With("Name", name).
                    With("Type", type);

            (INode node, bool isExtern) = this.factory.RequestExtern(name, type);
            if (isExtern) this.factory.AddAssignment(node, value);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing extern with assign").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles assigning the left value to the right value.</summary>
    public void HandleAssignment() {
        try {
            INode value  = this.nodes.Pop();
            INode target = this.nodes.Pop();
            INode root   = this.factory.AddAssignment(target, value);

            // TODO: Reevaluate this statement and make sure we're doing it correctly.
            //
            // Push the cast value back onto the stack for any following assignments.
            // By using the cast it makes it more like `X=Y=Z` is `Y=Z; X=Y;` but means that if a double and an int are being set by
            // an int, the int must be assigned first then it can cast to a double for the double assignment, otherwise it will cast
            // to a double but not be able to implicitly cast that double back to an int. For example: if `int X; double Y;` then
            // `Y=X=3;` works and `X=Y=3` will not. One drawback for this way is that if you assign an int to multiple doubles it will
            // construct multiple cast nodes, but with a little optimization to remove duplicate node paths, this isn't an issue. 
            // Alternatively, if we push the value then `X=Y=Z` will be like `Y=Z; X=Z;`, but we won't.
            this.nodes.Push(root);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error in assignment").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles performing a type cast of a node.</summary>
    public void HandleCast() {
        try {
            INode value = this.nodes.Pop();
            Type  type  = this.types.Pop();
            INode root  = this.factory.PerformCast(type, value, true);
            this.nodes.Push(root);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error while casting").
                With("Location", this.LastLocation).
                With("Error",    inner);
        }
    }

    /// <summary>This handles accessing an identifier to find the receiver for the next identifier.</summary>
    public void HandleMemberAccess() {
        string name  = this.LastText;
        INode  rNode = this.nodes.Pop();
        if (rNode is not IFieldReader receiver)
            throw new BlackboardException("Unexpected node type for a member access. Expected a field reader.").
                With("Node", rNode).
                With("Name", name).
                With("Location", this.LastLocation);

        INode node = receiver.ReadField(name) ??
            throw new BlackboardException("No identifier found in the receiver stack.").
                With("Identifier", name).
                With("Location", this.LastLocation);

        if (node is IExtern externNode)
            node = externNode.Shell;

        this.nodes.Push(node);
    }

    /// <summary>This handles preparing for a method call.</summary>
    public void HandleStartCall() =>
        this.arguments.Start();

    /// <summary>This handles the end of a method call and creates the node for the method.</summary>
    public void HandleAddArg() =>
        this.arguments.Add(this.nodes.Pop());

    /// <summary>This handles finishing a method call and building the node for the method.</summary>
    public void HandleEndCall() {
        INode[] args = this.arguments.End();
        INode   node = this.nodes.Pop();
        if (node is not IFuncGroup group)
            throw new BlackboardException("Unexpected node type for a method call. Expected a function group.").
                With("Node", node).
                With("Input", args.Types().Strings().Join(", ")).
                With("Location", this.LastLocation);

        INode? result = group.Build(args);
        if (result is null) {
            if (args.Length <= 0)
                throw new BlackboardException("Could not perform the function without any inputs.").
                    With("Function", group).
                    With("Location", this.LastLocation);
            throw new BlackboardException("Could not perform the function with the given input types.").
                With("Function", group).
                With("Input", args.Types().Strings().Join(", ")).
                With("Location", this.LastLocation);
        }

        this.nodes.Push(result);
    }

    /// <summary>This handles looking up a node by an id and pushing the node onto the stack.</summary>
    public void HandlePushId() {
        try {
            string name = this.LastText;
            INode node = this.factory.FindInNamespace(name);
            this.nodes.Push(node);
        } catch (S.Exception inner) {
            throw new BlackboardException("Error parsing identifier").
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
        this.parseLiteral("decode escaped sequences", (string text) => Literal.String(PetiteParser.Formatting.Text.Unescape(text)));

    /// <summary>This handles pushing a type onto the stack.</summary>
    public void HandlePushType() {
        string text = this.LastText;
        Type t = Type.FromName(text) ??
            throw new BlackboardException("Unrecognized type name.").
                With("Text",     text).
                With("Location", this.LastLocation);
        this.types.Push(t);
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
            parts.Add(this.factory.StackString(showActions, showGlobal, showScope, showExisting));
        if (showNodes)     parts.Add("Stack: "     + this.nodes.ToString(indent, false));
        if (showTypes)     parts.Add("Types: "     + this.types.ToString(indent, true));
        if (showIds)       parts.Add("Ids: "       + this.identifiers.ToString(indent, true));
        if (showArguments) parts.Add("Arguments: " + this.arguments.ToString(indent));
        return parts.Join("\n");
    }
}
