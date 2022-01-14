using S = System;

namespace Blackboard.Core.Nodes.Attributes {

    /// <summary>
    /// This attribute is for an optimization rule.
    /// The optimization rule will rearrange parents in a type node since they are commutable.
    /// This allows all the constants to be moved together and reduced.
    /// This attribute should be applied to the IData definitions.
    /// </summary>
    [S.AttributeUsage(S.AttributeTargets.Struct, Inherited = true)]
    sealed public class Commutable: S.Attribute {

        /// <summary>Creates a new commutable attribute for IData definitions.</summary>
        /// <param name="initial">The initial value for both sum and mul.</param>
        public Commutable(bool initial = true) {
            this.Sum = initial;
            this.Mul = initial;
        }

        /// <summary>Indicates summation is commutable.</summary>
        public bool Sum { get; set; }

        /// <summary>Indicates multiplication is commutable.</summary>
        public bool Mul { get; set; }
    }
}
