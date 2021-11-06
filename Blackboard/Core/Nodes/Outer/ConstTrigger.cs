using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>
    /// This node should only be used by the parser while reducing nodes.
    /// This will store a trigger's provode state as a conatnt to take the
    /// place of the trigger this was made a constant of.
    /// </summary>
    sealed public class ConstTrigger: TriggerNode, IConstantable, IConstant{

        /// <summary>Creates a new constant trigger value node.</summary>
        /// <param name="provoked">The provoke state for this trigger.</param>
        public ConstTrigger(bool provoked = false) {
            this.Provoked = provoked;
        }

        /// <summary>This is constant so it will always return true.</summary>
        public override bool IsConstant => true;

        /// <summary>Converts this node to a constant.</summary>
        /// <returns>This returns this constant itself.</returns>
        public override IConstant ToConstant() => this;

        /// <summary>Always returns no parents since constants have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>This will update the trigger during evaluation.</summary>
        /// <returns>True to trigger if the source value is true, false otherwise.</returns>
        protected override bool UpdateTrigger() => this.Provoked;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "ConstTrigger";

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) =>
            this.TypeName + "(" + (this.Provoked ? "provoked" : "") + ")";
    }
}
