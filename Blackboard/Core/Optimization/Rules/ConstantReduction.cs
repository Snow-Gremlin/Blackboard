using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Optimization.Rules;

/// <summary>
/// This is an optimization rule for finding constant branches and replacing them with literals.
/// This may leave nodes further down the constant branch in the nodes set.
/// This will also look up the stored constant in the slate and use that or
/// store the constant if one by that value doesn't exist yet.
/// </summary>
sealed internal class ConstantReduction : IRule {

    /// <summary>Gets the name of this rule.</summary>
    /// <returns>The string for this class used as the name of the rule.</returns>
    public override string ToString() => nameof(ConstantReduction);

    /// <summary>Finds constant branches and replaces the branch with literals.</summary>
    /// <param name="args">The rule arguments used while optimizing.</param>
    /// <param name="node">The node of the tree to constant optimize.</param>
    /// <remarks>The node to replace the given one in the parent or null to not replace.</remarks>
    static private void reduceNode(RuleArgs args, INode node) {
        // Check if the node can be turned into a constant.
        if (!node.IsConstant()) return;

        // It is a constant branch, so convert it to a literal constant. 
        args.UpdateValue(node);
        IConstant? con = node.ToConstant();
        if (con is null) return;

        // Look up constant in slate, if one doesn't exist, this will add it.
        con = args.Slate.FindAddConstant(con);
        if (ReferenceEquals(con, node)) return;

        args.Logger.Info("  Replace {0} with constant {1}", node, con);
        args.Changed = true;
        args.PostReachable(node).Foreach(args.Removed.Add);
        args.Replace(node, con);
    }

    /// <summary>Finds all constant branches and replaces each branch with literals.</summary>
    /// <param name="args">The arguments for the optimization rules.</param>
    public void Perform(RuleArgs args) =>
        args.PreReachable(args.Root).Foreach(node => reduceNode(args, node));
}
