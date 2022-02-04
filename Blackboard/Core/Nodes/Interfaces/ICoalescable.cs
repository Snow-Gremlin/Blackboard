namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>
    /// This interface indicates a node can be coalesced while optimizing.
    /// This is used by several optimization rules and may be extended for new optimizations.
    /// </summary>
    public interface ICoalescable: IChild {

        /// <summary>
        /// The identity element for the node which is a constant to use when coalescing the node for optimization.
        /// This identity is used in place of the node if there are no parents.
        /// </summary>
        /// <remarks>This is typically one for multiplication and zero for addition.</remarks>
        /// <see cref="https://en.wikipedia.org/wiki/Identity_element"/>
        public IConstant Identity { get; }

        /// <summary>
        /// Indicates that the parents can be reordered.
        /// To be able to reorder the parents the data type must also be commutative
        /// for the summation or multiplication being used in by the node.
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/Commutative_property"/>
        public bool Commutative { get; }

        /// <summary>
        /// Indicates that parents of the same type as this node may be removed and
        /// all of the parent's parents will be inserted at the same location as the parent node was.
        /// This may only be true for INaryChild nodes which allow multiple parents.
        /// </summary>
        /// <remarks>
        /// This will not work for nodes like average since the average of averages
        /// is different than the average of the input values, therefore,
        /// if we incorporate the parents into this node then the result will change.
        /// For example `Avg(Avg(2, 4), Avg(1, 3, 5)) = Avg(6/2, 9/3) = Avg(3, 2) = 5/2 = 2.5`
        /// but `Avg(2, 4, 1, 3, 5) = 15/5 = 3` which are different values.
        /// </remarks>
        /// <example>
        /// Sum(Sum(a, b), Sum(c, d)) => Sum(a, b, c, d);
        /// </example>
        public bool ParentIncorporate { get; }

        /// <summary>Indicates that the parents may be reduced to the smallest set.</summary>
        /// <remarks>
        /// If true then constant parents will be precomputed
        /// and constant parents equal to the identity will be removed.
        /// </remarks>
        /// <example>
        /// Sum(a, b, 0) => Sum(a, b);
        /// Sum(a) => a;
        /// Sum() => 0;
        /// Mul(a, 0.5, 2.0) => Mul(a, 1.0) => Mul(a) => a;
        /// Mul() => 1;
        /// String(a + "" + b) => String(a + b);
        /// String() => "";
        /// </example>
        public bool ParentReducable { get; }
    }
}
