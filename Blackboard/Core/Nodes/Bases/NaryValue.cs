using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has several parents as the source of the value.</summary>
    /// <typeparam name="TIn">The type of the all the parents' value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    /// <see cref="https://en.wikipedia.org/wiki/Arity#n-ary"/>
    public abstract class NaryValue<TIn, TResult>: ValueNode<TResult>, INaryChild<IValueParent<TIn>>
        where TIn : IData
        where TResult : IComparable<TResult> {

        /// <summary>This is the list of all the parent nodes to read from.</summary>
        private List<IValueParent<TIn>> sources;

        /// <summary>This is a helper for creating unary node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        static public IFuncDef CreateFactory<Tout>(S.Func<IEnumerable<IValueParent<TIn>>, Tout> handle)
            where Tout : NaryValue<TIn, TResult> =>
            new FunctionN<IValueParent<TIn>, Tout>(handle);

        /// <summary>Creates a N-ary value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public NaryValue(params IValueParent<TIn>[] parents) :
            this(parents as IEnumerable<IValueParent<TIn>>) { }

        /// <summary>Creates a N-ary value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public NaryValue(IEnumerable<IValueParent<TIn>> parents = null, TResult value = default) : base(value) {
            this.sources = new List<IValueParent<TIn>>();
            this.AddParents(parents);
            // UpdateValue already called by AddParents.
        }

        /// <summary>This adds parents to this node.</summary>
        /// <remarks>The value is updated after these parents are added.</remarks>
        /// <param name="parents">The set of parents to add.</param>
        public void AddParents(IEnumerable<IValueParent<TIn>> parents) =>
            this.sources.AddParents(parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <remarks>The value is updated after these parents are removed.</remarks>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        public bool RemoveParents(IEnumerable<IValueParent<TIn>> parents) =>
            this.sources.RemoveParents(this, parents);

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => this.sources;

        /// <summary>This handles updating this node's value given the parents' values during evaluation.</summary>
        /// <remarks>Any null parents are ignored.</remarks>
        /// <param name="values">The value from the all the non-null parents.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(IEnumerable<TIn> values);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override TResult CalcuateValue() =>
            this.OnEval(this.sources.NotNull().Values());
    }
}
