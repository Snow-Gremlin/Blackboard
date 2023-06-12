using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has two parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    /// <see cref="https://en.wikipedia.org/wiki/Arity#Binary"/>
    public abstract class BinaryValue<T1, T2, TResult> : ValueNode<TResult>, IChild
        where T1 : struct, IData
        where T2 : struct, IData
        where TResult : struct, IEquatable<TResult> {

        /// <summary>This is a helper for creating binary node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        static public IFuncDef CreateFactory<Tout>(S.Func<IValueParent<T1>, IValueParent<T2>, Tout> handle)
            where Tout : BinaryValue<T1, T2, TResult> =>
            new Function<IValueParent<T1>, IValueParent<T2>, Tout>(handle);

        /// <summary>This is the first parent node to read from.</summary>
        private IValueParent<T1>? source1;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueParent<T2>? source2;

        /// <summary>Creates a binary value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        public BinaryValue(IValueParent<T1>? source1 = null, IValueParent<T2>? source2 = null) {
            this.Parent1 = source1;
            this.Parent2 = source2;
        }

        /// <summary>The first parent node to get the first source value from.</summary>
        public IValueParent<T1>? Parent1 {
            get => this.source1;
            set => IChild.SetParent(this, ref this.source1, value);
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public IValueParent<T2>? Parent2 {
            get => this.source2;
            set => IChild.SetParent(this, ref this.source2, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public ParentCollection Parents => new ParentCollection(this, 2).
            With(() => this.source1, parent => this.source1 = parent).
            With(() => this.source2, parent => this.source2 = parent);

        /// <summary>This handles updating this node's value given the parents' values during evaluation.</summary>
        /// <remarks>This will not be called if any of the parents are null.</remarks>
        /// <param name="value1">The value from the first parent.</param>
        /// <param name="value2">The value from the second parent.</param>
        /// <returns>The new value for this node.</returns>
        protected abstract TResult OnEval(T1 value1, T2 value2);

        /// <summary>This updates the value during evaluation.</summary>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected override TResult CalculateValue() =>
            this.source1 is null || this.source2 is null ? default :
            this.OnEval(this.source1.Value, this.source2.Value);
    }
}
