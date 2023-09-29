using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Collections;

/// <summary>This is a collection of values returned from evaluating nodes.</summary>
sealed internal class Finalization {
    
    /// <summary>The set of provoked triggers which need to be reset.</summary>
    private readonly HashSet<ITrigger> needsReset;

    /// <summary>The set of output nodes which need to emit values.</summary>
    private readonly HashSet<IOutput> needsOutput;

    /// <summary>Creates a new finalization instance.</summary>
    public Finalization() {
        this.needsReset  = new();
        this.needsOutput = new();
    }

    /// <summary>Adds a set of nodes for finalization.</summary>
    /// <param name="nodes">The nodes to add if needing finalization.</param>
    public void Add(params INode?[] nodes) =>
        this.Add(nodes as IEnumerable<INode?>);

    /// <summary>Adds a set of nodes for finalization.</summary>
    /// <param name="nodes">The nodes to add if needing finalization.</param>
    public void Add(IEnumerable<INode?> nodes) =>
        nodes.NotNull().Foreach(this.add);

    /// <summary>Adds a node if it needs to finalization.</summary>
    /// <remarks>
    /// Nodes that have been evaluated but need finalization are trigger nodes which
    /// need to be reset and output nodes which has pending outputs to emit.
    /// </remarks>
    /// <param name="node">This is the node to check and add.</param>
    private void add(INode node) {
        if (node is IOutput  output  && output.Pending)   this.needsOutput.Add(output);
        if (node is ITrigger trigger && trigger.Provoked) this.needsReset.Add(trigger);
    }

    /// <summary>Performs a finalization.</summary>
    /// <remarks>If suspended then this will have no effect.</remarks>
    /// <param name="logger">An optional logger for debugging this finish.</param>
    public void Perform(Logger? logger = null) {
        logger.Notice("finalization: {0}", this);
        if (this.Suspend) return;
        
        // reset the triggers.
        this.needsReset.Reset();
        this.needsReset.Clear();
        
        // emit all pending output events.
        this.needsOutput.Emit();
        this.needsOutput.Clear();
    }

    /// <summary>Indicates if there are any outputs which needs to be emitting or any triggers needing to be reset.</summary>
    public bool HasPending =>
        this.needsReset.Count > 0 || this.needsOutput.Count > 0;

    /// <summary>Indicates that emitting outputs and resetting triggers is suspended.</summary>
    public bool Suspend { get; set; }

    /// <summary>Gets a string for the finalization.</summary>
    /// <returns>The human readable string for the finalization.</returns>
    public override string ToString() {
        List<string> buf = new();
        if (this.Suspend)           buf.Add("suspended");
        if (this.needsReset.Any())  buf.Add("resets: "+this.needsReset.Count);
        if (this.needsOutput.Any()) buf.Add("outputs: "+this.needsOutput.Count);
        return buf.Join(", ");
    }
}
