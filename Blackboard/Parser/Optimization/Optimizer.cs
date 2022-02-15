﻿using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Optimization.Rules;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization {

    /// <summary>
    /// A tool for running rules on the new nodes for a formula to optimize, validate,
    /// and reduce the formula as much as possible. This will save memory, make
    /// comparisons faster, and improve performance of Blackboard's graph in slate.
    /// </summary>
    sealed internal class Optimizer {
        private List<IRule> rules;

        /// <summary>Creates a new optimizer.</summary>
        public Optimizer() {
            this.rules = new List<IRule>() {
                new ConstantReduction(),
                new Coalescer(),

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
                //         - <TBA>
                //   - [ ] And:
                //         - <TBA>
                //   - [ ] Any:
                //         - <TBA>
                //   - [ ] BitwiseAnd:
                //         - <TBA>
                //   - [ ] BitwiseOr:
                //         - <TBA>
                //   - [ ] BitwiseXor:
                //         - <TBA>
                //   - [ ] OnlyOne:
                //         - <TBA>
                //   - [ ] Or:
                //         - <TBA>
                //   - [ ] Xor:
                //         - <TBA>
                
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
                //         - <TBA>
                //   - [ ] Neg:
                //         - If parent is a Neg, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
                //         - <TBA>
                //   - [ ] Not:
                //         - If parent is a Not, remove both this node and parent, and replace with parent's parent (recheck for deep nesting)
                //         - If parent is NotEqual, remove both this node and parent, and replace with an Equal with the parent's parents
                //         - Same as previous but for other comparisons, e.g. `Not(GreaterThan(a, b))` => `LessThanOrEqual(a, b)`
                //         - <TBA>
                //   - [ ] NotEqual
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
        public INode Optimize(Slate slate, INode root, HashSet<INode> nodes, ILogger logger = null) {
            RuleArgs args = new(slate, root, nodes, logger);
            while (args.Changed) {
                args.Changed = false;
                foreach (IRule rule in this.rules)
                    rule.Perform(args);
            }
            return args.Root;
        }
    }
}
