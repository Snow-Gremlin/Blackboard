using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization {

    /// <summary>
    /// A tool for running rules on the new nodes for a formula to optimize
    /// and reduce the formula as much as possible. This will save memory, make
    /// comparisons faster, and improve performance of Blackboard's graph in slate.
    /// </summary>
    sealed internal class Optimizer: IRule {
        private List<IRule> rules;

        /// <summary>Creates a new optimizer.</summary>
        public Optimizer() {
            this.rules = new List<IRule>() {
                new ConstantReduction(),

                // TODO: Add ruled for node specific reductions:
                //   - TODO: Group these based on what they do so we can determine if they
                //           need to have a common abstract/template class. Just need to know how
                //           we're going to handle these optimizations. It might require additions
                //           to the nodes, if we can determine how/what should go into those optimization
                //           and what should be left out.
                //   - TODO: Use the following old list to fill out new lists
                //        - Combine all constants in communicative nodes, such as sum (not string concatenation), products, ANDs, ORs, etc.
                //        - Remove all 1 constants in products, all 0 constants in sums, all true constants in ANDs, etc.
                //        - Find all unneeded nodes such as a single parent sum, product, and, or, etc.
                //        - Find all unneeded nodes such as a switch with both parents the same.
                //   - [ ] All:
                //         - <TBA>
                //   - [ ] And:
                //         - <TBA>
                //   - [ ] Any:
                //         - <TBA>
                //   - [ ] Average:
                //         - If there is only one parent, replace node with that parent
                //   - [ ] BitwiseAnd:
                //         - <TBA>
                //   - [ ] BitwiseNot:
                //         - <TBA>
                //   - [ ] BitwiseOr:
                //         - <TBA>
                //   - [ ] BitwiseXor:
                //         - <TBA>
                //   - [ ] Div:
                //         - <TBA>
                //   - [ ] Implies
                //         - <TBA>
                //   - [ ] LeftShift
                //         - <TBA>
                //   - [ ] Max:
                //         - If there is only one parent, replace node with that parent
                //   - [ ] Min:
                //         - If there is only one parent, replace node with that parent
                //   - [ ] Mul:
                //         - Pre-multiply all constants
                //         - Remove 1 constants
                //         - If there is only one parent in the mul, remove mul node and replace with single parent
                //   - [ ] OnChange:
                //         - <TBA>
                //   - [ ] OnFalse:
                //         - <TBA>
                //   - [ ] OnlyOne:
                //         - <TBA>
                //   - [ ] OnTrue:
                //         - <TBA>
                //   - [ ] Or:
                //         - <TBA>
                //   - [ ] RightShift:
                //         - <TBA>
                //   - [ ] SelectTrigger:
                //         - <TBA>
                //   - [ ] SelectValue:
                //         - <TBA>
                //   - [ ] Sum:
                //         - Pre-add all constants, if communicative (i.e. not string concatenations)
                //         - Remove 0 constants for numbers and empty string constants for strings
                //         - If there is only one parent in the sum, remove sum node and replace with single parent
                //   - [ ] Xor:
                //         - If there is only one parent, replace node with that parent
                //         - <TBA>

                // TODO: Add advanced mathematics simplification rules:
                //   - Simplify ANDs of ORs, DeMorgan's rule to reduce NOTs in ANDs and ORs, Sums of Products, etc
                //   - [ ] NotEqual
                //         - <TBA>
                //   - [ ] Neg:
                //         - If parent is a Neg, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
                //         - <TBA>
                //   - [ ] Not:
                //         - If parent is a Not, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
                //         - If parent is NotEqual, remove both this node and parent, and replace with an Equal with the parent's parents
                //         - Same as previous but for other comparisons, e.g. `Not(GreaterThan(a, b))` => `LessThanOrEqual(a, b)`
                //         - <TBA>
                //   - [ ] Sub:
                //         - If parent is a Sub, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
                //         - <TBA>

                // TODO: Add rule for reusing existing nodes:
                //   - [ ] Find and replace repeat branches in the new nodes.
                //   - [ ] Find and replace existing duplicate branches defined on slate using constants and existing nodes.
                new RemoveUnreachable()
            };
        }

        /// <summary>Performs optimization on the given nodes and surrounding nodes.</summary>
        /// <param name="slate">The slate the formula is for.</param>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        /// <remarks>The node to replace the given root with or the given root.</remarks>
        public INode Perform(Slate slate, INode root, HashSet<INode> nodes, ILogger logger = null) {
            foreach (IRule rule in this.rules)
                root = rule.Perform(slate, root, nodes, logger) ?? root;
            return root;
        }
    }
}
