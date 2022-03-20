using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>Performs a trigger when the parent becomes false.</summary>
    sealed public class OnFalse: TriggerNode, IChild {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<Bool>, OnFalse>((input) => new OnFalse(input));

        /// <summary>This is the parent node to read from.</summary>
        private IValueParent<Bool> source;

        /// <summary>Creates a trigger node which triggers when the boolean goes from false to false.</summary>
        public OnFalse() => this.Parent = null;

        /// <summary>Creates a trigger node which triggers when the boolean goes from false to false.</summary>
        /// <param name="source">This is the single parent for the source value.</param>
        public OnFalse(IValueParent<Bool> source = null) => this.Parent = source;

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new OnFalse();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(OnFalse);

        /// <summary>The parent node to get the source value from.</summary>
        public IValueParent<Bool> Parent {
            get => this.source;
            set => IChild.SetParent(this, ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IParentCollection Parents => new FixedParents(this).
            With(() => this.source, (IValueParent<Bool> parent) => this.source = parent);

        /// <summary>This updates the trigger during an evaluation.</summary>
        /// <returns>This always returns true so that any parent change will trigger this node.</returns>
        protected override bool ShouldProvoke() => !this.source.Value.Value;
    }
}
