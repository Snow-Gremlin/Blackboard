using S = System;

namespace Blackboard.Core {

    /// <summary>This is a match for a single type.</summary>
    /// <remarks>This is used for choosing the most specific method for a function.</remarks>
    sealed public class TypeMatch: S.IComparable<TypeMatch> {

        /// <summary>Indicates there was no match for this type.</summary>
        static public readonly TypeMatch NoMatch = new(false, false, 0);

        /// <summary>Indicates there is an inheritance match for this type.</summary>
        /// <param name="steps">The number of inheritance steps are needed.</param>
        /// <returns>This new inheritance type match.</returns>
        static public TypeMatch Inherit(int steps) => new(true, false, steps);

        /// <summary>Indicates there is an implicit cast match for this type.</summary>
        /// <param name="steps">The number of implicit cast steps are needed.</param>
        /// <returns>This new implicit cast type match.</returns>
        static public TypeMatch Cast(int steps) => new(true, true, steps);

        /// <summary>True if there is a match, false otherwise.</summary>
        public readonly bool IsMatch;

        /// <summary>True if a cast was neeeded, false if inheritance.</summary>
        public readonly bool NeedsCast;

        /// <summary>The number of inheritance steps away needed.</summary>
        public readonly int Steps;

        /// <summary>Creates a new type match.</summary>
        /// <param name="isMatch">Treu if there is a match, false otherwise.</param>
        /// <param name="needsCast">True if a cast, false if inheritance.s</param>
        /// <param name="steps">The number of inheritance steps.</param>
        private TypeMatch(bool isMatch, bool needsCast, int steps) {
            this.IsMatch = isMatch;
            this.NeedsCast = needsCast;
            this.Steps = steps;
        }

        /// <summary>Compares this match with another match.</summary>
        /// <remarks>This does not compare IsMatch since it is assumed only matching types will be compared.</remarks>
        /// <param name="other">The other match to compare with.</param>
        /// <returns>
        /// -1 if match if this is less than the other,
        ///  0 if they match,
        ///  1 if this is greater than the other.
        /// </returns>
        public int CompareTo(TypeMatch other) {
            int cmp = this.NeedsCast.CompareTo(other.NeedsCast);
            if (cmp == 0) cmp = this.Steps.CompareTo(other.Steps);
            return cmp;
        }

        /// <summary>Checks if this match is equal to the given object.</summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is TypeMatch other && this.CompareTo(other) == 0;

        /// <summary>Gets the hash code for this match.</summary>
        /// <returns>The hash code of the match.</returns>
        public override int GetHashCode() =>
            S.HashCode.Combine(this.IsMatch, this.NeedsCast, this.Steps);

        /// <summary>Gets the string for the match.</summary>
        /// <returns>The match's string.</returns>
        public override string ToString() => this.IsMatch ?
            (this.NeedsCast ? "Cast" : "Inherit")+"("+this.Steps+")" : "None";
    }
}
