﻿using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for a node which has a value.</summary>
    /// <typeparam name="T">The type of the value being held.</typeparam>
    public abstract class ValueNode<T>: Evaluatable, IValueParent<T>, IConstantable
        where T : IComparable<T> {

        /// <summary>Creates a new value node.</summary>
        /// <param name="value">The initial value of the node.</param>
        public ValueNode(T value = default) => this.Value = value ?? default;

        /// <summary>Converts this node to a constant.</summary>
        /// <returns>A consant of this node.</returns>
        public virtual IConstant ToConstant() => new Literal<T>(this.Value);

        /// <summary>This gets the data being stored in this node.</summary>
        /// <returns>The data being stored.</returns>
        public IData Data => this.Value;

        /// <summary>The value being held by this node.</summary>
        public T Value { get; private set; }

        /// <summary>Sets the node value to the given value.</summary>
        /// <param name="value">The new value to set to this node.</param>
        /// <returns>True if the value changed, false if the value did not change.</returns>
        protected bool SetNodeValue(T value) {
            if (this.Value.CompareTo(value) == 0) return false;
            this.Value = value;
            return true;
        }

        /// <summary>Updates the node's value during node evaluation.</summary>
        /// <returns>
        /// True indicates that the value changed, false otherwise.
        /// When the value has changed all the children are returned from the evaulation,
        /// otherwise no children are returned.
        /// </returns>
        abstract protected bool UpdateValue();

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of all the children if the value changed during update.
        /// If the value hasn't changed then no children are returned.
        /// </returns>
        sealed public override IEnumerable<IEvaluatable> Eval() =>
            this.UpdateValue() ? this.Children.OfType<IEvaluatable>() : Enumerable.Empty<IEvaluatable>();
    }
}
