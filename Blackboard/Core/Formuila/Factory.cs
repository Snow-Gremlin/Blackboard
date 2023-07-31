using Blackboard.Core.Extensions;
using Blackboard.Core.Formuila.Actions;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
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

    /// <summary>Creates new a new action collection.</summary>
    /// <param name="slate">The slate to create the formula for.</param>
    /// <param name="logger">The optional logger to write debugging information to.</param>
    internal Factory(Slate slate, Logger? logger = null) {
        this.slate   = slate;
        this.logger  = logger;
        this.actions = new LinkedList<IAction>();
        this.scope   = new ScopeStack(this.slate, this.logger);
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



    /// <summary>
    /// Checks for an existing node and, if none exists, creates an extern node
    /// as a placeholder with the given name in the local scope.
    /// </summary>
    /// <param name="name">The name to create the extern for.</param>
    /// <param name="type">The type of extern to create.</param>
    /// <param name="value">The optional initial default value the extern node.</param>
    public void RequestExtern(string name, Type type, INode? value = null) {
        this.logger.Info("Request Extern:");

        if (value is not null && type == Type.Trigger)
            throw new Message("May not initialize an extern trigger.").
                With("Name", name).
                With("Type", type);

        VirtualNode scope = this.scope.Current;
        INode? existing = scope.ReadField(name);
        if (existing is not null) {

            Type existType = Type.TypeOf(existing) ??
                throw new Message("Unable to find existing type.").
                    With("Name",     name).
                    With("Existing", existing).
                    With("New Type", type).
                    With("Value",    value);

            if (existType != type)
                throw new Message("Extern node does not match existing node type.").
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
                With("Name", name).
                With("Type", type);

        this.add(new Extern(scope.Receiver, name, node));
        scope.WriteField(name, node);
        if (value is not null) this.AddAssignment(node, value);
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
