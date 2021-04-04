﻿using Blackboard.Core.Bases;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Caps {

    /// <summary>This gets the product of all the float parents.</summary>
    public class MulFloat: Nary<double, double> {

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        public MulFloat(params IValue<double>[] parents) :
            base(parents) { }

        /// <summary>Creates a product value node.</summary>
        /// <param name="parents">The initial set of parents to use.</param>
        /// <param name="value">The default value for this node.</param>
        public MulFloat(IEnumerable<IValue<double>> parents = null, double value = default) :
            base(parents, value) { }

        /// <summary>Gets the product of the parent values during evaluation.</summary>
        /// <param name="values">All the parent values to multiply.</param>
        /// <returns>The product of the parent values.</returns>
        protected override double OnEval(IEnumerable<double> values) {
            double result = 1.0;
            foreach (double value in values) result *= value;
            return result;
        }

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "MulFloat"+base.ToString();
    }
}
