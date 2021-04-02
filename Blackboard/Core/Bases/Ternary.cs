using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Bases {

    /// <summary>This is a value node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the third parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class Ternary<T1, T2, T3, TResult>: ValueNode<TResult> {

        /// <summary>This is the first parent node to read from.</summary>
        private IValue<T1> source1;

        /// <summary>This is the second parent node to read from.</summary>
        private IValue<T2> source2;

        /// <summary>This is the third parent node to read from.</summary>
        private IValue<T3> source3;

        /// <summary>Creates a ternary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Ternary(IValue<T1> source1 = null, IValue<T2> source2 = null,
            IValue<T3> source3 = null, TResult value = default) : base(value) {
            this.Parent1 = source1;
            this.Parent2 = source2;
            this.Parent3 = source3;
            this.UpdateValue();
        }

        /// <summary>The first parent node to get the first source value from.</summary>
        public IValue<T1> Parent1 {
            get => this.source1;
            set {
                if (!(this.source1 is null))
                    this.source1.RemoveChildren(this);
                this.source1 = value;
                if (!(this.source1 is null))
                    this.source1.AddChildren(this);
                this.UpdateValue();
            }
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public IValue<T2> Parent2 {
            get => this.source2;
            set {
                if (!(this.source2 is null))
                    this.source2.RemoveChildren(this);
                this.source2 = value;
                if (!(this.source2 is null))
                    this.source2.AddChildren(this);
                this.UpdateValue();
            }
        }

        /// <summary>The third parent node to get the third source value from.</summary>
        public IValue<T3> Parent3 {
            get => this.source3;
            set {
                if (!(this.source3 is null))
                    this.source3.RemoveChildren(this);
                this.source3 = value;
                if (!(this.source3 is null))
                    this.source3.AddChildren(this);
                this.UpdateValue();
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.source1 is null)) yield return this.source1;
                if (!(this.source2 is null)) yield return this.source2;
                if (!(this.source3 is null)) yield return this.source3;
            }
        }

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
            if (this.source1 is null || this.source2 is null) return false;
            TResult value = this.OnEval(this.source1.Value, this.source2.Value, this.source3.Value);
            return this.SetNodeValue(value);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            "("+NodeString(this.source1)+", "+NodeString(this.source2)+", "+NodeString(this.source3)+")";
    }
}
