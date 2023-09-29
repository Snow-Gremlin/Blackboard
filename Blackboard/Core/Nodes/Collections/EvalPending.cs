using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Collections;

/// <summary>The list of evaluable nodes pending to update.</summary>
sealed internal class EvalPending {

    /// <summary>The group of nodes with the same depth.</summary>
    sealed private class DepthGroup {
        public readonly HashSet<IEvaluable> nodes;

        /// <summary>The node depth for all contained nodes.</summary>
        public readonly int Depth;

        /// <summary>Creates a new depth group with the given node in it.</summary>
        /// <param name="first">The first node to add and get the depth from.</param>
        public DepthGroup(IEvaluable first) {
            this.Depth = first.Depth;
            this.nodes = new() { first };
        }
        
        /// <summary>Adds a new node if it doesn't already exist.</summary>
        /// <param name="node">The node to add.</param>
        public void Add(IEvaluable node) => this.nodes.Add(node);

        /// <summary>Indicates if this set is empty.</summary>
        public bool Empty => this.nodes.Count <= 0;

        /// <summary>Gets the number of nodes in this depth.</summary>
        public int Count => this.nodes.Count;

        /// <summary>Removes and returns a node from this set.</summary>
        /// <remarks>Set may not be empty, otherwise this will throw an exception.</remarks>
        /// <returns>The node removed from the set.</returns>
        public IEvaluable TakeOne() {
            IEvaluable e = this.nodes.First();
            this.nodes.Remove(e);
            return e;
        }
    }

    /// <summary>A comparer for comparing the depth groups by the depth value.</summary>
    sealed private class DepthComparer : IComparer<DepthGroup> {

        /// <summary>Compares two depth groups.</summary>
        /// <param name="x">The first depth group.</param>
        /// <param name="y">The second depth group.</param>
        /// <returns>The result of the comparisons of the two given depths.</returns>
        public int Compare(DepthGroup? x, DepthGroup? y) {
            int xDepth = x?.Depth ?? int.MinValue;
            int yDepth = y?.Depth ?? int.MinValue;
            return xDepth.CompareTo(yDepth);
        }
    }

    private readonly SortedSet<DepthGroup> pending;

    /// <summary>Creates a new list of pending nodes.</summary>
    public EvalPending() => this.pending = new(new DepthComparer());

    /// <summary>Indicates if there are any pending nodes.</summary>
    public bool HasPending => this.pending.Count > 0;

    /// <summary>Determines the total count of all the nodes being stored.</summary>
    public int Count => this.pending.Sum(group => group.Count);
    
    /// <summary>This inserts and groups unique evaluable nodes.</summary>
    /// <param name="nodes">The set of nodes to insert.</param>
    public void Insert(params IEvaluable[] nodes) =>
        this.Insert((IEnumerable<IEvaluable>)nodes);

    /// <summary>This inserts and groups unique evaluable nodes.</summary>
    /// <param name="nodes">The set of nodes to insert.</param>
    public void Insert(IEnumerable<IEvaluable> nodes) =>
        nodes.Foreach(this.insertNode);

    /// <summary>This sort-inserts the given node.</summary>
    /// <param name="node">The node to insert.</param>
    private void insertNode(IEvaluable node) {
        DepthGroup group = new(node);
        if (this.pending.TryGetValue(group, out DepthGroup? existing))
            existing.Add(node);
        else this.pending.Add(group);
    }

    /// <summary>This inserts and groups unique evaluable nodes.</summary>
    /// <param name="nodes">The set of nodes to insert.</param>
    public void Insert(EvalPending nodes) =>
        nodes.pending.Foreach(this.insertGroup);
    
    /// <summary>This sort-inserts the given node group.</summary>
    /// <param name="group">The group to insert.</param>
    private void insertGroup(DepthGroup group) {
        if (this.pending.TryGetValue(group, out DepthGroup? existing))
            group.nodes.Foreach(existing.Add);
        else this.pending.Add(group);
    }

    /// <summary>Gets all the pending nodes.</summary>
    public IEnumerable<IEvaluable> Nodes => this.pending.SelectMany(group => group.nodes);

    /// <summary>Removes the first pending node and returns it.</summary>
    /// <returns>The node which was removed.</returns>
    public IEvaluable? TakeFirst() {
        DepthGroup? group = this.pending.FirstOrDefault();
        if (group is null) return null;

        IEvaluable? e = group.TakeOne();
        if (group.Empty) this.pending.Remove(group);
        return e;
    }

    /// <summary>This updates the depth values of the pending nodes.</summary>
    /// <param name="logger">The logger to debug the update with.</param>
    public void UpdateDepths(Logger? logger = null) {
        logger.Info("Start Update (pending: {0})", this.Count);

        while (this.HasPending) {
            IEvaluable? node = this.TakeFirst();
            if (node is null) break;

            // Determine the depth that this node should be at based on its parents.
            int depth = node.MinimumAllowedDepth();

            // If the depth has changed then its children also need to be updated.
            bool changed = false;
            if (node.Depth != depth) {
                node.Depth = depth;
                this.Insert(node.Children.OfType<Evaluable>());
                changed = true;
            }

            logger.Info("  Updated (changed: {0}, depth: {1}, node: {2}, remaining: {3})", changed, node.Depth, node, this.Count);
        }

        logger.Info("End Update");
    }
    
    /// <summary>Updates and propagates the changes from the given inputs through the blackboard nodes.</summary>
    /// <param name="finalization">The set to add nodes to which need finalization.</param>
    /// <param name="logger">The logger to debug the evaluate with.</param>
    public void Evaluate(Finalization finalization, Logger? logger = null) {
        logger.Info("Start Eval (pending: {0})", this.Count);

        while (this.HasPending) {
            IEvaluable? node = this.TakeFirst();
            if (node is null) break;

            bool changed = false;
            if (node.Evaluate()) {
                this.Insert(node.Children.NotNull().OfType<IEvaluable>());
                finalization.Add(node);
                changed = true;
            }

            logger.Info("  Evaluated (changed: {0}, depth: {1}, node: {2}, remaining: {3})", changed, node.Depth, node, this.Count);
        }

        logger.Info("End Eval ({0})", finalization);
    }

    /// <summary>Gets the string for the list of pending nodes.</summary>
    /// <returns>The string of nodes.</returns>
    public override string ToString() => this.Nodes.Join("\n");
}
