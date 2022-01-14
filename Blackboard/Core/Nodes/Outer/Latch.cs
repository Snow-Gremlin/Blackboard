using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a latching value node.</summary>
    /// <typeparam name="T">The type of the value for this node.</typeparam>
    sealed public class Latch<T>: ValueNode<T>, IChild
        where T : IEquatable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<ITriggerParent, IValueParent<T>, Latch<T>>((trigger, source) => new Latch<T>(trigger, source));

        /// <summary>This is the first parent node to read from.</summary>
        private ITriggerParent trigger;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueParent<T> source;

        /// <summary>Creates a latching value node.</summary>
        /// <param name="trigger">This is the parent that indicates the value should be set.</param>
        /// <param name="source">This is the parent to get the value from when the other is provoked.</param>
        /// <param name="value">The initial value for this node.</param>
        public Latch(ITriggerParent trigger = null, IValueParent<T> source = null, T value = default) : base(value) {
            this.Trigger = trigger;
            this.Source = source;
        }

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
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.trigger, this.source);

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            IChild.ReplaceParent(this, ref this.trigger, oldParent, newParent) |
            IChild.ReplaceParent(this, ref this.source,  oldParent, newParent);

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
