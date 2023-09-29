using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Optimization.Rules;

/// <summary>Removes any nodes which are unreachable from the root.</summary>
sealed internal class RemoveUnreachable : IRule {

    /// <summary>Gets the name of this rule.</summary>
    /// <returns>The string for this class used as the name of the rule.</returns>
    public override string ToString() => nameof(RemoveUnreachable);

    /// <summary>Finds unreachable nodes to remove from the given set of nodes.</summary>
    /// <param name="args">The arguments for the optimization rules.</param>
    public void Perform(RuleArgs args) {
        HashSet<INode> reached = new(args.PostReachable(args.Root));
        int removed = args.Nodes.Count - reached.Count;
        if (removed <= 0) return;

        args.Nodes.WhereNot(reached.Contains).Foreach(args.Removed.Add);
        args.Logger.Info("  Removed {0} unreachable nodes. {1} nodes were reachable.", removed, reached.Count);
    }
}
