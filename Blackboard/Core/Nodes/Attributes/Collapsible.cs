using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Outer;
using S = System;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>
    /// This attribute is for an optimization rule.
    /// The optimization rule will remove a node if it has only one non-nil parent,
    /// then the node should be replaced by the parent itself.
    /// If there are no parents, then a default value is returned.
    /// The default value will also be removed from the parent list
    /// </summary>
    /// <example>
    /// Sum(a, b, 0) => Sum(a, b);
    /// Sum(a) => a;
    /// Sum() => 0.0;
    /// Mul(a, b, 1) => Mul(a, b);
    /// Mul(a) => a;
    /// Mul() => 1.0;
    /// </example>
    [S.AttributeUsage(S.AttributeTargets.Class, Inherited = true)]
    sealed public class Collapsible: S.Attribute {

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The boolean to use if there are no parents.</param>
        public Collapsible(bool defaultValue) : this(new Bool(defaultValue)) { }

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The integer to use if there are no parents.</param>
        public Collapsible(int defaultValue) : this(new Int(defaultValue)) { }

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The double to use if there are no parents.</param>
        public Collapsible(double defaultValue) : this(new Double(defaultValue)) { }

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The string to use if there are no parents.</param>
        public Collapsible(string defaultValue) : this(new String(defaultValue)) { }

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The data to use if there are no parents.</param>
        public Collapsible(IData defaultValue) : this(Literal.Data(defaultValue)) { }

        /// <summary>Creates a new collapsible attribute.</summary>
        /// <param name="defaultValue">The constant to use if there are no parents.</param>
        public Collapsible(IConstant defaultValue) => this.DefaultValue = defaultValue;

        /// <summary>
        /// The constant to use if there are no parents.
        /// This is also used to remove constants which have no effect on the result.
        /// </summary>
        public IConstant DefaultValue { get; }
    }
}
