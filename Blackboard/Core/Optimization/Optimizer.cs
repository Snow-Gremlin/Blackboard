using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Optimization.Rules;
using System.Collections.Generic;

namespace Blackboard.Core.Optimization;

/// <summary>
/// A tool for running rules on the new nodes for a formula to optimize, validate,
/// and reduce the formula as much as possible. This will save memory, make
/// comparisons faster, and improve performance of Blackboard's graph in slate.
/// </summary>
sealed internal class Optimizer {
    private readonly IReadOnlyList<IRule> rules;

    /// <summary>Creates a new optimizer.</summary>
    public Optimizer() =>
        this.rules = new List<IRule>() {
            new ConstantReduction(),
            new ParentIncorporator(),
            new ConstantConsolidator(),

            // TODO: Add ruled for node specific reductions:
            //   - TODO: Group these based on what they do so we can determine if they
            //           need to have a common abstract/template class. Just need to know how
            //           we're going to handle these optimizations. It might require additions
            //           to the nodes, if we can determine how/what should go into those optimization
            //           and what should be left out.
            //   - TODO: Use the following old list to fill out new lists
            //        - Combine all constants in communicative nodes, such as sum (not string concatenation), products, ANDs, ORs, etc.
            //        - Remove all 1 constants in products, all 0 constants in sums, all true constants in ANDs, etc.
            //        
            //   - [ ] All:
            //         - <TBD>
            //   - [ ] And:
            //         - <TBD>
            //   - [ ] Any:
            //         - <TBD>
            //   - [ ] BitwiseAnd:
            //         - <TBD>
            //   - [ ] BitwiseOr:
            //         - <TBD>
            //   - [ ] BitwiseXor:
            //         - <TBD>
            //   - [ ] OnlyOne:
            //         - <TBD>
            //   - [ ] Or:
            //         - <TBD>
            //   - [ ] Xor:
            //         - <TBD>
                
            // TODO: Add ruled for node specific reductions:
            //   - [ ] Mul:
            //         - Pre-multiply all constants
            //         - Remove 1 constants
            //         - If there is only one parent in the mul, remove mul node and replace with single parent
            //   - [ ] SelectTrigger, SelectValue:
            //         - Remove selects with both parents the same, replace with parent.
            //         - Remove selects if the condition is constant, replaces with selected content.
            //   - [ ] Sum:
            //         - Pre-add all constants, if communicative (i.e. not string concatenations)
            //         - Remove 0 constants for numbers and empty string constants for strings
            //         - If there is only one parent in the sum, remove sum node and replace with single parent
            //   - [ ] LeftShift, RightShift:
            //         - If the first value is a 0 constant, then replace with that constant.
            //         - If the second value is a 0 constant, then return the first parent.
            //   - [ ] Div:
            //         - If the first parent is 0 constant, replace 
            //         - If the second parent is 1 constant return first parent.
            //   - [ ] Implies
            //         - If the first value is false constant, replace with a true constant.
            //         - If the second value is a true constant, replace with a true constant.
            //         - TODO: Add math simplification rules for if a parent is a NOT
            //

            // TODO: Add advanced mathematics simplification rules:
            //   - Simplify ANDs of ORs, DeMorgan's rule to reduce NOTs in ANDs and ORs, Sums of Products, etc
            //   - [ ] BitwiseNot:
            //         - <TBD>
            //   - [ ] Neg:
            //         - If parent is a Neg, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
            //         - If a value is being negated in a sum and the same value exists in the sum they can both be removed.
            //         - If parent is a sub, then replace it with a reversed sub, `Neg(Sub(X, Y)) => Sub(Y, X)`
            //         - <TBD>
            //   - [ ] Not:
            //         - If parent is a Not, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
            //         - If parent is NotEqual, remove both this node and parent, and replace with an Equal with the parent's parents
            //         - Same as previous but for other comparisons, e.g. `Not(GreaterThan(a, b))` => `LessThanOrEqual(a, b)`
            //         - <TBD>
            //   - [ ] NotEqual
            //         - <TBD>
            //   - [ ] Sub:
            //         - If parent is a Sub, remove both this node and parent, and replace with sum (recheck for deep nesting)
            //         - If sub is a parent in a sum, check for a new combination which cancels out the sub and combine subs
            //           with a neg sum if there are multiple subs.
            //         - <TBD>

            // TODO: Add rule for reusing existing nodes:
            //   - [ ] Find and replace repeat branches in the new nodes.
            //   - [ ] Find and replace existing duplicate branches defined on slate using constants and existing nodes.
            new RemoveUnreachable(),
            new ActionValidator()
        };

    /// <summary>Performs optimization on the given nodes and surrounding nodes.</summary>
    /// <param name="slate">The slate the formula is for.</param>
    /// <param name="root">The root node of the tree to optimize.</param>
    /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
    /// <param name="logger">The logger to debug and inspect the optimization.</param>
    /// <remarks>The node to replace the given root with or the given root.</remarks>
    public INode Optimize(Slate slate, INode root, HashSet<INode> nodes, Logger? logger = null) {
        string opName = nameof(Optimize);
        Logger? opLogger = logger.Stringify(Stringifier.Deep())?.Group(opName);
        opLogger.Info("Start {0}:", opName);

        int i = 0;
        const int maxCycles = 100;
        bool changed = true;
        while (changed) {
            changed = false;

            foreach (IRule rule in this.rules) {
                string ruleName = rule.ToString() ?? "null";
                Logger? ruleLogger = opLogger.Group(ruleName);
                ruleLogger.Info("Start {0}: {1}", ruleName, root);

                // Perform the rule on the given root and nodes.
                RuleArgs args = new(slate, root, nodes, ruleLogger);
                rule.Perform(args);

                // Clean up and prepare for next rule or to be finished.
                if (args.Removed.Count > 0) {
                    if (args.Removed.Contains(args.Root))
                        throw new BlackboardException("Optimization rule {0} removed the root without updating the root.", ruleName).
                            With("Old Root", root).
                            With("Root", args.Root).
                            With("Nodes", args.Nodes).
                            With("Removed", args.Removed);

                    args.Nodes.RemoveWhere(args.Removed.Contains);
                    args.Removed.Clear();
                    args.Changed = true;
                }

                if (args.Changed) {
                    changed = true;
                    root = args.Root;
                    ruleLogger.Info("Done {0}: Changed => {1}", ruleName, args.Root);
                } else ruleLogger.Info("Done {0}", ruleName);
            }

            i++;
            if (i >= maxCycles)
                throw new BlackboardException("Optimization took more than {0} cycles.", maxCycles).
                    With("Root", root).
                    With("Nodes", nodes);
        }

        opLogger.Info("Done {0}.\n", opName);
        return root;
    }
}
