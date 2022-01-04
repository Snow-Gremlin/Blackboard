﻿using Blackboard.Core.Nodes.Interfaces;
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
                new ConstantReduction()
                // TODO: Add rule: Find all unneeded nodes such as a single parent sum, product, and, or, etc.
                // TODO: Add rule: Find all unneeded nodes such as a switch with both parents the same.
                // TODO: Add rule: Replace all constants with constants stored on the slate to reduce duplicates.
                // TODO: Add rule: Find and replace repeat branches in the new nodes.
                // TODO: Add rule: Find and replace existing duplicate branches defined on slate using constants and existing nodes.
                // TODO: Add rule: Removed unreachable nodes.
            };
        }

        /// <summary>Performs optimization on the given nodes and surrounding nodes.</summary>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
        /// <remarks>The node to replace the given root with or the given root.</remarks>
        public INode Perform(INode root, HashSet<INode> nodes) {
            foreach (IRule rule in this.rules) root = rule.Perform(root, nodes);
            return root;
        }
    }
}