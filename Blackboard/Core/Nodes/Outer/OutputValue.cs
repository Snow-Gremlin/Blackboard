using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>Event arguments for the change in a value node.</summary>
    /// <typeparam name="T">The type of the value node.</typeparam>
    sealed public class OutputValueEventArgs<T>: S.EventArgs {

        /// <summary>The previous value before the change.</summary>
        public readonly T Previous;

        /// <summary>The current value after the change.</summary>
        public readonly T Current;

        /// <summary>Creates a new event arguments for a value node.</summary>
        /// <param name="prev">The previous value before the change.</param>
        /// <param name="cur">The current value after the change.</param>
        public OutputValueEventArgs(T prev, T cur) {
            this.Previous = prev;
            this.Current = cur;
        }
    }

    /// <summary>A node for listening for changes in values used for outputting to the user.</summary>
    /// <typeparam name="T">The type of the value to hold.</typeparam>
    sealed public class OutputValue<T>: ValueNode<T>, IValueOutput<T, OutputValueEventArgs<T>>
        where T : IComparable<T>, new() {

        /// <summary>The parent source to listen to.</summary>
        private IValueAdopter<T> source;

        /// <summary>Creates a new output value node.</summary>
        /// <param name="source">The initial source to get the value from.</param>
        /// <param name="value">The initial value for this node.</param>
        public OutputValue(IValueAdopter<T> source = null, T value = default) : base(value) {
            this.Parent = source;
            this.UpdateValue();
        }

        /// <summary>The parent node to get the value from.</summary>
        public IValueAdopter<T> Parent {
            get => this.source;
            set {
                this.SetParent(ref this.source, value);
                this.UpdateValue();
            }
        }

        /// <summary>This event is emitted when the value is changed.</summary>
        public event S.EventHandler<OutputValueEventArgs<T>> OnChanged;

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This will update the value.</summary>
        /// <returns>This will always return true.</returns>
        protected override bool UpdateValue() {
            if (this.source is null) return false;
            T prev = this.Value;
            if (this.SetNodeValue(this.source.Value)) {
                this.OnChanged?.Invoke(this, new OutputValueEventArgs<T>(prev, this.Value));
                return true;
            }
            return false;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Output<" + this.Value + ">";
    }
}
