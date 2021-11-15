using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the third parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class Ternary<T1, T2, T3, TResult>: ValueNode<TResult>
        where T1 : IData
        where T2 : IData
        where T3 : IData
        where TResult : IComparable<TResult>, new() {

        /// <summary>This is the first parent node to read from.</summary>
        private IValueAdopter<T1> source1;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueAdopter<T2> source2;

        /// <summary>This is the third parent node to read from.</summary>
        private IValueAdopter<T3> source3;

        /// <summary>Creates a ternary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Ternary(IValueAdopter<T1> source1 = null, IValueAdopter<T2> source2 = null,
            IValueAdopter<T3> source3 = null, TResult value = default) : base(value) {
            this.SetParent(ref this.source1, source1);
            this.SetParent(ref this.source2, source2);
            this.SetParent(ref this.source3, source3);
            this.UpdateValue();
        }

        /// <summary>The first parent node to get the first source value from.</summary>
        public IValueAdopter<T1> Parent1 {
            get => this.source1;
            set {
                this.SetParent(ref this.source1, value);
                this.UpdateValue();
            }
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public IValueAdopter<T2> Parent2 {
            get => this.source2;
            set {
                this.SetParent(ref this.source2, value);
                this.UpdateValue();
            }
        }

        /// <summary>The third parent node to get the third source value from.</summary>
        public IValueAdopter<T3> Parent3 {
            get => this.source3;
            set {
                this.SetParent(ref this.source3, value);
                this.UpdateValue();
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => INode.NotNull(this.source1, this.source2, this.source3);

        /// <summary>This handles updating this node's value given the parents' values during evaluation.</summary>
        /// <remarks>This will not be called if any of the parents are null.</remarks>
        /// <param name="value1">The value from the first parent.</param>
        /// <param name="value2">The value from the second parent.</param>
        /// <param name="value3">The value from the third parent.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(T1 value1, T2 value2, T3 value3);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            if (this.source1 is null || this.source2 is null || this.source3 is null) return false;
            TResult value = this.OnEval(this.source1.Value, this.source2.Value, this.source3.Value);
            return this.SetNodeValue(value);
        }
    }
}
