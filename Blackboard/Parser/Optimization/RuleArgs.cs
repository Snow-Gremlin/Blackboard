using Blackboard.Core;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Optimization {

    /// <summary>The arguments for rules of optimization.</summary>
    internal class RuleArgs {

        /// <summary>The slate the formula is for.</summary>
        public readonly Slate Slate;

        /// <summary>The logger to debug and inspect the optimization.</summary>
        /// <remarks>May be null to not log during optimization.</remarks>
        public readonly Logger Logger;

        /// <summary>The new nodes for a formula which need to be optimized.</summary>
        public readonly HashSet<INode> Nodes;

        /// <summary>The root node of the tree to optimize.</summary>
        public INode Root;

        /// <summary>Indicates optimization has changed and a second pass needs to e run.</summary>
        public bool Changed;

        /// <summary>Creates a new rule arguments.</summary>
        /// <param name="slate">The slate the formula is for.</param>
        /// <param name="root">The root node of the tree to optimize.</param>
        /// <param name="nodes">The new nodes for a formula which need to be optimized.</param>
        /// <param name="logger">The logger to debug and inspect the optimization.</param>
        public RuleArgs(Slate slate, INode root, HashSet<INode> nodes, Logger logger = null) {
            this.Slate   = slate;
            this.Logger  = logger;
            this.Nodes   = nodes;
            this.Root    = root;
            this.Changed = true;
        }

        /// <summary>
        /// This evaluates down the branch starting from the given node
        /// to get the correct value prior to converting it to a constant.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        public void UpdateValue(INode node) {
            if (node is IChild child) {
                foreach (INode parent in child.Parents.Nodes) {
                    if (this.Nodes.Contains(node))
                        this.UpdateValue(parent);
                }
            }
            if (node is IEvaluable eval)
                eval.Evaluate();
        }
    }
}
