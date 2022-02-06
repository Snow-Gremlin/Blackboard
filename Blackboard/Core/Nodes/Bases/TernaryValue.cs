using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This is a value node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the third parent's value for this node.</typeparam>
    /// <typeparam name="TResult">The type of value this node holds.</typeparam>
    /// <see cref="https://en.wikipedia.org/wiki/Arity#Ternary"/>
    public abstract class TernaryValue<T1, T2, T3, TResult>: ValueNode<TResult>, IChild
        where T1 : IData
        where T2 : IData
        where T3 : IData
        where TResult : IEquatable<TResult> {

        /// <summary>This is a helper for creating ternary node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        static public IFuncDef CreateFactory<Tout>(S.Func<IValueParent<T1>, IValueParent<T2>, IValueParent<T3>, Tout> handle)
            where Tout : TernaryValue<T1, T2, T3, TResult> =>
            new Function<IValueParent<T1>, IValueParent<T2>, IValueParent<T3>, Tout>(handle);

        /// <summary>This is the first parent node to read from.</summary>
        private IValueParent<T1> source1;

        /// <summary>This is the second parent node to read from.</summary>
        private IValueParent<T2> source2;

        /// <summary>This is the third parent node to read from.</summary>
        private IValueParent<T3> source3;

        /// <summary>Creates a ternary value node.</summary>
        /// <param name="source1">This is the first parent for the source value.</param>
        /// <param name="source2">This is the second parent for the source value.</param>
        /// <param name="source3">This is the third parent for the source value.</param>
        public TernaryValue(IValueParent<T1> source1 = null, IValueParent<T2> source2 = null, IValueParent<T3> source3 = null) {
            this.Parent1 = source1;
            this.Parent2 = source2;
            this.Parent3 = source3;
        }

        /// <summary>The first parent node to get the first source value from.</summary>
        public IValueParent<T1> Parent1 {
            get => this.source1;
            set => IChild.SetParent(this, ref this.source1, value);
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public IValueParent<T2> Parent2 {
            get => this.source2;
            set => IChild.SetParent(this, ref this.source2, value);
        }

        /// <summary>The third parent node to get the third source value from.</summary>
        public IValueParent<T3> Parent3 {
            get => this.source3;
            set => IChild.SetParent(this, ref this.source3, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.source1, this.source2, this.source3);

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool ReplaceParent(IParent oldParent, IParent newParent) =>
            IChild.ReplaceParent(this, ref this.source1, oldParent, newParent) |
            IChild.ReplaceParent(this, ref this.source2, oldParent, newParent) |
            IChild.ReplaceParent(this, ref this.source3, oldParent, newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAllParents(List<IParent> newParents) {
            IChild.CheckParentsBeingSet(newParents, false, typeof(IValueParent<T1>), typeof(IValueParent<T2>), typeof(IValueParent<T3>));
            return IChild.SetParent(this, ref this.source1, newParents[0] as IValueParent<T1>) |
                   IChild.SetParent(this, ref this.source2, newParents[1] as IValueParent<T2>) |
                   IChild.SetParent(this, ref this.source3, newParents[2] as IValueParent<T3>);
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
        protected override TResult CalcuateValue() =>
            this.source1 is null || this.source2 is null || this.source2 is null ? default :
            this.OnEval(this.source1.Value, this.source2.Value, this.source3.Value);
    }
}
