using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a latching value node.</summary>
    /// <typeparam name="T">The type of the value for this node.</typeparam>
    sealed public class Latch<T> : ValueNode<T>, IChild
        where T : IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<ITriggerParent, IValueParent<T>, Latch<T>>((trigger, source) => new Latch<T>(trigger, source));

        /// <summary>This is the first parent node to read from.</summary>
        private ITriggerParent trigger;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueParent<T> source;

        /// <summary>Creates a latching value node.</summary>
        public Latch() { }

        /// <summary>Creates a latching value node.</summary>
        /// <param name="trigger">This is the parent that indicates the value should be set.</param>
        /// <param name="source">This is the parent to get the value from when the other is provoked.</param>
        /// <param name="value">The initial value for this node.</param>
        public Latch(ITriggerParent trigger = null, IValueParent<T> source = null, T value = default) : base(value) {
            this.Trigger = trigger;
            this.Source = source;
        }

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Latch<T>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Latch";

        /// <summary>The parent node to indicate when the value should be set to the other parent.</summary>
        public ITriggerParent Trigger {
            get => this.trigger;
            set => IChild.SetParent(this, ref this.trigger, value);
        }

        /// <summary>The parent node to get the source value from if the other parent is provoked.</summary>
        public IValueParent<T> Source {
            get => this.source;
            set => IChild.SetParent(this, ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public ParentCollection Parents => new ParentCollection(this, 2).
            With(() => this.trigger, (ITriggerParent parent) => this.trigger = parent).
            With(() => this.source, (IValueParent<T> parent) => this.source  = parent);

        /// <summary>This sets the value of this node.</summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value has changed, false otherwise.</returns>
        public bool SetValue(T value) => this.UpdateValue(value);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override T CalcuateValue() =>
            this.trigger is null || this.source is null ? default :
            this.trigger.Provoked ? this.source.Value : this.Value;
    }
}
