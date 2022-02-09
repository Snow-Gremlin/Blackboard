using Blackboard.Core.Data.Caps;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>This will return the value of one of two parents based on a boolean parent.</summary>
    /// <remarks>This functions just like a typical ternary (i.e. `test ? left : right`) statement.</remarks>
    /// <typeparam name="T">The parent type to select between and return.</typeparam>
    public abstract class Select<T>: Evaluable, IChild
        where T : class, IParent {

        /// <summary>This is a helper for creating a select node factories quickly.</summary>
        /// <param name="handle">The handler for calling the node constructor.</param>
        static protected IFuncDef CreateFactory<Tout>(S.Func<IValueParent<Bool>, T, T, Tout> handle)
            where Tout : Select<T> => new Function<IValueParent<Bool>, T, T, Tout>(handle);

        /// <summary>This is the test node to read the select state from.</summary>
        private IValueParent<Bool> source1;

        /// <summary>This is the first parent to output when the test is true.</summary>
        private T source2;

        /// <summary>This is the second parent to output when the test is false.</summary>
        private T source3;

        /// <summary>Creates a selection value node.</summary>
        /// <param name="test">This is the first parent for the boolean for selection between the other two parents.</param>
        /// <param name="left">This is the second parent to select when the test boolean is true.</param>
        /// <param name="right">This is the third parent to select when the test boolean is false.</param>
        public Select(IValueParent<Bool> test = null, T left = null, T right = null) {
            this.Parent1 = test;
            this.Parent2 = left;
            this.Parent3 = right;
            this.Selected = null;
        }

        /// <summary>The first parent node to get the boolean for selection between the other two parents.</summary>
        public IValueParent<Bool> Parent1 {
            get => this.source1;
            set => IChild.SetParent(this, ref this.source1, value);
        }

        /// <summary>The second parent node to get the second source value from.</summary>
        public T Parent2 {
            get => this.source2;
            set => IChild.SetParent(this, ref this.source2, value);
        }

        /// <summary>The third parent node to get the third source value from.</summary>
        public T Parent3 {
            get => this.source3;
            set => IChild.SetParent(this, ref this.source3, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IParentCollection Parents => new FixedParents(this).
            With(() => this.source1, (IValueParent<Bool> parent) => this.source1 = parent).
            With(() => this.source2, (T parent) => this.source2 = parent).
            With(() => this.source3, (T parent) => this.source3 = parent);

        /// <summary>The node which is currently selected.</summary>
        public T Selected { get; private set; }

        /// <summary>Updates the node's value, provoked state, and any other state.</summary>
        /// <remarks>This should be overridden by the inheriting class so that the results can be further tracked.</remarks>
        /// <returns>True indicates that the selected node has changed, false otherwise.</returns>
        public override bool Evaluate() {
            T newSelected = this.source1.Value.Value ? this.source2 : this.source3;
            if (ReferenceEquals(this.Selected, newSelected)) return false;
            this.Selected = newSelected;
            return true;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
