using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Bases {

    /// <summary>This is a value node which has several parents as the source of the value.</summary>
    /// <typeparam name="TIn">The type of the all the parents' value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class Nary<TIn, TResult>: ValueNode<TResult> {

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private List<IValue<TIn>> sources;

        /// <summary>Creates a N-ary value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public Nary(params IValue<TIn>[] parents) :
            this(parents as IEnumerable<IValue<TIn>>) { }

        /// <summary>Creates a N-ary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public Nary(IEnumerable<IValue<TIn>> parents = null, TResult value = default) : base(value) {
            this.sources = new List<IValue<TIn>>();
            this.AddParents(parents);
            this.UpdateValue();
        }

        /// <summary>This adds parents to this node.</summary>
        /// <remarks>The value is updated after these parents are added.</remarks>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(params IValue<IValue<TIn>>[] parents) =>
            this.AddParents(parents as IEnumerable<IValue<TIn>>);

        /// <summary>This adds parents to this node.</summary>
        /// <remarks>The value is updated after these parents are added.</remarks>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<IValue<TIn>> parents) {
            this.sources.AddRange(parents);
            foreach (IValue<TIn> parent in parents)
                parent.AddChildren(this);
            this.UpdateValue();
        }

        /// <summary>This removes the given parents from this node.</summary>
        /// <remarks>The value is updated after these parents are removed.</remarks>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(params IValue<TIn>[] parents) =>
            this.RemoveParents(parents as IEnumerable<IValue<TIn>>);

        /// <summary>This removes the given parents from this node.</summary>
        /// <remarks>The value is updated after these parents are removed.</remarks>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<IValue<TIn>> parents) {
            bool anyRemoved = false;
            foreach (IValue<TIn> parent in parents) {
                if (this.sources.Remove(parent)) {
                    parent.RemoveChildren(this);
                    anyRemoved = true;
                }
            }
            if (anyRemoved) this.UpdateValue();
            return anyRemoved;
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<INode> Parents => this.sources;

        /// <summary>This handles updating this node's value given the parents' values during evaluation.</summary>
        /// <remarks>Any null parents are ignored.</remarks>
        /// <param name="values">The value from the all the non-null parents.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(IEnumerable<TIn> values);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override bool UpdateValue() =>
            this.SetNodeValue(this.OnEval(this.sources.NotNull().Values()));

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "("+NodeString(this.sources)+")";
    }
}
