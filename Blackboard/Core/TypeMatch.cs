using S = System;

namespace Blackboard.Core {

    /// <summary>This is a match for a single type.</summary>
    /// <remarks>This is used for choosing the most specific method for a function.</remarks>
    sealed public class TypeMatch: S.IComparable<TypeMatch> {

        /// <summary>This is the possible types that a match or non-match can have.</summary>
        private enum Kind {

            /// <summary>Indicates there was no match found.</summary>
            NoMatch,

            /// <summary>Indicates that the match was with inheritance so no cast is needed.</summary>
            Inherit,

            /// <summary>Indicates that the match requires an implicit cast.</summary>
            Implicit,

            /// <summary>Indicates that there was no match but can be cast explicitly.</summary>
            Explicit
        }

        /// <summary>Indicates there was no match for this type.</summary>
        static public readonly TypeMatch NoMatch = new(Kind.NoMatch, 0);

        /// <summary>Indicates there is an inheritance match for this type.</summary>
        /// <param name="steps">The number of inheritance steps are needed.</param>
        /// <returns>This new inheritance type match.</returns>
        static public TypeMatch Inherit(int steps) => new(Kind.Inherit, steps);

        /// <summary>Indicates there is an implicit cast match for this type.</summary>
        /// <param name="steps">The number of implicit cast steps are needed.</param>
        /// <returns>This new implicit cast type match.</returns>
        static public TypeMatch Implicit(int steps) => new(Kind.Implicit, steps);

        /// <summary>Indicates there is an explicit cast non-match for this type.</summary>
        /// <returns>This new explicit cast type non-match.</returns>
        static public TypeMatch Explicit() => new(Kind.Explicit, 0);

        /// <summary>True if a cast was needed, false if inheritance.</summary>
        private readonly Kind matchKind;

        /// <summary>The number of inheritance steps away needed.</summary>
        public readonly int Steps;

        /// <summary>Creates a new type match.</summary>
        /// <param name="matchKind">The kind of match or non-match.</param>
        /// <param name="steps">The number of inheritance steps.</param>
        private TypeMatch(Kind matchKind, int steps) {
            this.matchKind = matchKind;
            this.Steps = steps;
        }

        /// <summary>True if there is an inherit match or an implicit cast match, false otherwise.</summary>
        public bool IsMatch => this.matchKind is Kind.Inherit or Kind.Implicit;

        /// <summary>True if there is an inheritance match, false otherwise.</summary>
        public bool IsInherit => this.matchKind is Kind.Inherit;

        /// <summary>True if there is an implicit cast match, false otherwise.</summary>
        public bool IsImplicit => this.matchKind is Kind.Implicit;

        /// <summary>True if there is an explicit cast match, false otherwise.</summary>
        public bool IsExplicit => this.matchKind is Kind.Explicit;

        /// <summary>True if there is an implicit or explicit cast match, false otherwise.</summary>
        public bool IsAnyCast => this.matchKind is Kind.Implicit or Kind.Explicit;

        /// <summary>Compares this match with another match.</summary>
        /// <remarks>This does not compare IsMatch since it is assumed only matching types will be compared.</remarks>
        /// <param name="other">The other match to compare with.</param>
        /// <returns>
        /// -1 if match if this is less than the other,
        ///  0 if they match,
        ///  1 if this is greater than the other.
        /// </returns>
        public int CompareTo(TypeMatch other) {
            int cmp = this.IsImplicit.CompareTo(other.IsImplicit);
            if (cmp == 0) cmp = this.Steps.CompareTo(other.Steps);
            return cmp;
        }

        /// <summary>Checks if this match is equal to the given object.</summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is TypeMatch other && this.matchKind == other.matchKind && this.Steps == other.Steps;

        /// <summary>Gets the hash code for this match.</summary>
        /// <returns>The hash code of the match.</returns>
        public override int GetHashCode() =>
            S.HashCode.Combine(this.matchKind, this.Steps);

        /// <summary>Gets the string for the match.</summary>
        /// <returns>The match's string.</returns>
        public override string ToString() =>
            this.matchKind switch {
                Kind.NoMatch  => "None",
                Kind.Inherit  => "Inherit"  + "(" + this.Steps + ")",
                Kind.Implicit => "Implicit" + "(" + this.Steps + ")",
                Kind.Explicit => "Explicit",
                _             => "Unknown(" + this.matchKind + ", " + this.Steps + ")"
            };
    }
}
