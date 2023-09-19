using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;

namespace Blackboard.Core.Formula.Actions;

/// <summary>
/// This is an action to add an extern node in a field writer.
/// Typically this is for defining a new node into the namespaces reachable from global.
/// </summary>
sealed public class Extern : IAction {

    /// <summary>Creates a new extern define action.</summary>
    /// <remarks>It is assumed that these values have been run through the optimizer and validated.</remarks>
    /// <param name="receiver">This is the receiver that will be written to.</param>
    /// <param name="name">The name to write the node with.</param>
    /// <param name="node">The node being set to the receiver with the given name.</param>
    public Extern(IFieldWriter receiver, string name, IExtern node) {
        if (receiver is null or VirtualNode)
            throw new Message("May not use a null or {0} as the receiver in a {1}.", nameof(VirtualNode), nameof(Extern));

        this.Receiver = receiver;
        this.Name     = name;
        this.Node     = node;
    }

    /// <summary>This is the receiver that will be written to.</summary>
    public readonly IFieldWriter Receiver;

    /// <summary>The name to write the node with.</summary>
    public readonly string Name;

    /// <summary>The node being set to the receiver with the given name.</summary>
    public readonly IExtern Node;

    /// <summary>This will perform the action.</summary>
    /// <param name="slate">The slate for this action.</param>
    /// <param name="result">The result being created and added to.</param>
    /// <param name="logger">The optional logger to debug with.</param>
    public void Perform(Slate slate, Record.Result result, Logger? logger = null) {
        logger.Info("Add Extern: {0}", this);

        INode? existing = this.Receiver.ReadField(this.Name);
        if (existing is not null) {

            Type existType = Type.TypeOf(existing) ??
                throw new Message("Unable to find existing type while setting extern.").
                    With("Name",     this.Name).
                    With("Existing", existing).
                    With("Node",     this.Node);

            Type externType = Type.TypeOf(this.Node) ??
                throw new Message("Unable to find extern type while setting extern.").
                    With("Name",     this.Name).
                    With("Existing", existing).
                    With("Node",     this.Node);

            if (existType != externType)
                throw new Message("Extern node does not match existing node type.").
                    With("Name",          this.Name).
                    With("Existing",      existing).
                    With("Existing Type", existType).
                    With("Node",          this.Node);

            // Node already exists as an extern or the actual node.
            return;
        }

        // Write the extern placeholder node.
        this.Receiver.WriteField(this.Name, this.Node);
    }

    /// <summary>Gets a human readable string for this define.</summary>
    /// <returns>The human readable string for debugging.</returns>
    public override string ToString() => Stringifier.Simple(this);
}
