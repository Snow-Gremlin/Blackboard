using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a trigger when the parent becomes true.</summary>
    sealed public class OnTrue: TriggerNode {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueAdopter<Bool>, OnTrue>((input) => new OnTrue(input));

        /// <summary>This is the parent node to read from.</summary>
        private IValueAdopter<Bool> source;

        /// <summary>Creates a trigger node which triggers when the boolean goes from false to true.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public OnTrue(IValueAdopter<Bool> source = null) {
            this.Parent = source;
        }

        /// <summary>The parent node to get the source value from.</summary>
        public IValueAdopter<Bool> Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This will update the trigger during evaluation.</summary>
        /// <returns>True to trigger if the source value is true, false otherwise.</returns>
        protected override bool UpdateTrigger() => this.source.Value.Value;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "OnTrue";

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) {
            string tail = nodeDepth > 0 ?
                INode.NodePrettyString(showFuncs, nodeDepth-1, this.source) :
                (this.Provoked ? "provoked" : "");
            return this.TypeName + "(" + tail + ")";
        }
    }
}
