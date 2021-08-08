using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has a single parent as the source of the value.</summary>
    /// <typeparam name="T1">The type of the parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class Unary<T1, TResult>: ValueNode<TResult>
        where T1 : IData
        where TResult : IComparable<TResult>, new() {

        /// <summary>This is the parent node to read from.</summary>
        private IValue<T1> source;

        /// <summary>Creates a unary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="source">This is the single parent for the source value.</param>
        /// <param name="value">The default value for this node.</param>
        public Unary(IValue<T1> source = null, TResult value = default) : base(value) {
            this.SetParent(ref this.source, source);
            this.UpdateValue();
        }

        /// <summary>The parent node to get the source value from.</summary>
        public IValue<T1> Parent {
            get => this.source;
            set {
                this.SetParent(ref this.source, value);
                this.UpdateValue();
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents {
            get {
                if (this.source is not null) yield return this.source;
            }
        }

        /// <summary>This handles updating this node's value given the parent's value during evaluation.</summary>
        /// <remarks>This will not be called if the parent is null.</remarks>
        /// <param name="value">The value from the parent.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(T1 value);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() {
            if (this.source is null) return false;
            TResult value = this.OnEval(this.source.Value);
            return this.SetNodeValue(value);
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "("+NodeString(this.source)+")";
    }
}
