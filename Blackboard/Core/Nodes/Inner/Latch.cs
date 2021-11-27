using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is a latching value node.</summary>
    /// <typeparam name="T">The type of the value for this node.</typeparam>
    sealed public class Latch<T>: ValueNode<T>, IChild
        where T : IComparable<T> {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<ITriggerParent, IValueParent<T>, Latch<T>>((trigger, source) => new Latch<T>(trigger, source));

        /// <summary>This is the first parent node to read from.</summary>
        private ITriggerParent trigger;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueParent<T> source;

        /// <summary>Creates a latching value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="trigger">This is the parent that indicates the value should be set.</param>
        /// <param name="source">This is the parent to get the value from when the other is provoked.</param>
        public Latch(ITriggerParent trigger = null, IValueParent<T> source = null) {
            this.Trigger = trigger;
            this.Source = source;
        }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Latch";

        /// <summary>The parent node to indicate when the value should be set to the other parent.</summary>
        public ITriggerParent Trigger {
            get => this.trigger;
            set => this.SetParent(ref this.trigger, value);
        }

        /// <summary>The parent node to get the source value from if the other parent is provoked.</summary>
        public IValueParent<T> Source {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.trigger, this.source);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override T CalcuateValue() =>
            this.trigger is null || this.source is null ? default :
            this.trigger.Provoked ? this.source.Value : this.Value;
    }
}
