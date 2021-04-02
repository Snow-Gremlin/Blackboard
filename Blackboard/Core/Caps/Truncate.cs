using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System;

namespace Blackboard.Core.Caps {

    public class Truncate: Unary<double, int> {

        protected override int OnEval(double value) => (int)value;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Truncate"+base.ToString();
    }
}
