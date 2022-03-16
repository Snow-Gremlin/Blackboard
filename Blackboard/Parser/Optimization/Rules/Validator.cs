using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Optimization.Rules {

    /// <summary>Validates all the nodes in the action being created.</summary>
    sealed internal class Validator: IRule {

        static private void checkChildren(RuleArgs args, IParent node) {

        }

        static private void checkParent(RuleArgs args, IChild node) {

        }

        /// <summary>Performs validation of the action being created.</summary>
        /// <param name="args">The argument for this rule.</param>
        public void Perform(RuleArgs args) {
            foreach (INode node in args.Nodes) {
                if (node is IParent parent) checkChildren(args, parent);
                if (node is IChild  child)  checkParent(args, child);
            }
        }
    }
}
