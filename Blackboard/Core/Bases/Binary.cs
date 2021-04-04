using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Bases {

    /// <summary>This is a value node which has two parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class Binary<T1, T2, TResult>: ValueNode<TResult> {

        /// <summary>This is the first parent node to read from.</summary>
        private IValue<T1> source1;

        /// <summary>This is the second parent node to read from.</summary>
        private IValue<T2> source2;

        /// <summary>Creates a binary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Binary(IValue<T1> source1 = null, IValue<T2> source2 = null, TResult value = default) : base(value) {
            this.Parent1 = source1;
            this.Parent2 = source2;
            this.UpdateValue();
        }

        /// <summary>The first parent node to get the first source value from.</summary>
        public IValue<T1> Parent1 {
            get => this.source1;
            set {
                this.source1 = this.SetParent(this.source1, value);
                this.UpdateValue();
            }
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public IValue<T2> Parent2 {
            get => this.source2;
            set {
                this.source2 = this.SetParent(this.source2, value);
                this.UpdateValue();
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (!(this.source1 is null)) yield return this.source1;
                if (!(this.source2 is null)) yield return this.source2;
            }
        }

        /// <summary>This handles updating this node's value given the parents' values during evaluation.</summary>
        /// <remarks>This will not be called if any of the parents are null.</remarks>
        /// <param name="value1">The value from the first parent.</param>
        /// <param name="value2">The value from the second parent.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(T1 value1, T2 value2);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            if (this.source1 is null || this.source2 is null) return false;
            TResult value = this.OnEval(this.source1.Value, this.source2.Value);
            return this.SetNodeValue(value);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() =>
            "("+NodeString(this.source1)+", "+NodeString(this.source2)+")";
    }
}
