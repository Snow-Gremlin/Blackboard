using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a trigger which can be provoked from user output.</summary>
    sealed public class OutputTrigger: TriggerNode, ITriggerOutput {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<ITriggerAdopter, OutputTrigger>((ITriggerAdopter source) => new OutputTrigger(source));

        /// <summary>The parent source to listen to.</summary>
        private ITriggerAdopter source;

        /// <summary>Creates a new output trigger.</summary>
        /// <param name="source">The initial source trigger to listen to.</param>
        public OutputTrigger(ITriggerAdopter source = null) {
            this.Parent = source;
        }

        /// <summary>The parent trigger node to listen to.</summary>
        public ITriggerAdopter Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>This event is emitted when the trigger has been provoked.</summary>
        public event S.EventHandler OnProvoked;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool UpdateTrigger() {
            if (this.source.Provoked) {
                this.OnProvoked?.Invoke(this, S.EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Output";

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) {
            string tail = nodeDepth <= 0 ? "" :
                INode.NodePrettyString(showFuncs, nodeDepth-1, this.source);
            return this.TypeName + "<trigger>[" + (this.Provoked ? "provoked" : "") + "](" + tail + ")";
        }
    }
}
