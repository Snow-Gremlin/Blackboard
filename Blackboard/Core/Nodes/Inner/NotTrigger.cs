using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Gets a boolean value for the inverse of the trigger.</summary>
    sealed public class NotTrigger: ValueNode<Bool> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<ITriggerAdopter, NotTrigger>((input) => new NotTrigger(input));

        /// <summary>This is the parent node to read from.</summary>
        private ITriggerAdopter source;

        /// <summary>Creates a trigger node which triggers when the boolean goes from false to true.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public NotTrigger(ITriggerAdopter source = null) {
            this.Parent = source;
        }

        /// <summary>The parent node to get the source value from.</summary>
        public ITriggerAdopter Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>Updates this value during evaluation.</summary>
        /// <returns>This always returns true.</returns>
        protected override bool UpdateValue() {
            if (this.source is null) return false;
            bool value = !this.source.Provoked;
            return this.SetNodeValue(new Bool(value));
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "NotTrigger";
    }
}
