using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has a single parent as the source of the value.</summary>
    /// <typeparam name="T1">The type of the parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    public abstract class UnaryValue<T1, TResult>: ValueNode<TResult>, IChild
        where T1 : IData
        where TResult : IComparable<TResult> {

        /// <summary>This is a helper for creating unary node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        static public IFuncDef CreateFactory<Tout>(S.Func<IValueParent<T1>, Tout> handle)
            where Tout : UnaryValue<T1, TResult> =>
            new Function<IValueParent<T1>, Tout>(handle);

        /// <summary>This is the parent node to read from.</summary>
        private IValueParent<T1> source;

        /// <summary>Creates a unary value node.</summary>
        /// <remarks>The value is updated right away so the default value may not be used.</remarks>
        /// <param name="source">This is the single parent for the source value.</param>
        public UnaryValue(IValueParent<T1> source = null) =>
            this.SetParent(ref this.source, source);

        /// <summary>The parent node to get the source value from.</summary>
        public IValueParent<T1> Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.source);

        /// <summary>This handles updating this node's value given the parent's value during evaluation.</summary>
        /// <remarks>This will not be called if the parent is null.</remarks>
        /// <param name="value">The value from the parent.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(T1 value);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override TResult CalcuateValue() =>
            this.source is null ? default : this.OnEval(this.source.Value);
    }
}
