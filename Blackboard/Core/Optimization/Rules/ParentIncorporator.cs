using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;

namespace Blackboard.Core.Optimization.Rules;

/// <summary>An optimizer rule for incorporating parents in nodes which implement ICoalescable.</summary>
sealed internal class ParentIncorporator : IRule {

    /// <summary>Gets the name of this rule.</summary>
    /// <returns>The string for this class used as the name of the rule.</returns>
    public override string ToString() => nameof(ParentIncorporator);

    /// <summary>
    /// Find any parents of the same type of node with the given node as its only child
    /// and replace that parent with the parent parents at the same location and in the same order.
    /// </summary>
    /// <param name="node">The node to incorporate parents into.</param>
    static private void incorporateParents(RuleArgs args, ICoalescable node) {
        ParentCollection parents = node.Parents;
        for (int i = 0; i < parents.Count; ++i) {
            IParent? parent = parents[i];

            if (parent is not null &&
                node.GetType().Equals(parent.GetType()) &&
                parent.Children.IsCount(1) &&
                ReferenceEquals(parent.Children.First(), node) &&
                parent is IChild childParent) {

                // Incorporate the parent's parents in place of the parent.
                // Step back the index, so that the new parents are checked.
                args.Logger.Info("  Incorporating parent {0} into {1}.", i, node);
                parents.Remove(i);
                parents.Insert(i, childParent.Parents, childParent);
                args.Nodes.Remove(childParent);
                --i;
                args.Changed = true;
            }
        }
    }

    /// <summary>Incorporates parents as part of the coalesce of nodes.</summary>
    /// <param name="args">The arguments for the optimization rules.</param>
    public void Perform(RuleArgs args) =>
        args.Nodes.OfType<ICoalescable>().Where(node => node.ParentIncorporate).Foreach(node => incorporateParents(args, node));
}
