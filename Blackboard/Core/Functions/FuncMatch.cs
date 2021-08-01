using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is a match for a single function.</summary>
    /// <remarks>This is used for choosing the most specific method for a function.</remarks>
    public class FuncMatch : S.IComparable<FuncMatch> {

        /// <summary>Indicates there was no match for this function.</summary>
        static public readonly FuncMatch NoMatch = new(false, null);

        /// <summary>Creates a new function match and checking if it has no matches.</summary>
        /// <param name="needOneNoCast">Indicates if there must be at least one non-cast argument.</param>
        /// <param name="matches">The type matches for all the arguments of the function.</param>
        /// <returns>The new function match or no match if no match could be made.</returns>
        static public FuncMatch Create(bool needOneNoCast, params TypeMatch[] matches) =>
            Create(needOneNoCast, matches as IEnumerable<TypeMatch>);

        /// <summary>Creates a new function match and checking if it has no matches.</summary>
        /// <param name="needOneNoCast">Indicates if there must be at least one non-cast argument.</param>
        /// <param name="matches">The type matches for all the arguments of the function.</param>
        /// <returns>The new function match or no match if no match could be made.</returns>
        static public FuncMatch Create(bool needOneNoCast, IEnumerable<TypeMatch> matches) {
            int length = 0;
            foreach (TypeMatch match in matches) {
                if (!match.IsMatch) return NoMatch;
                length++;
            }

            if (length > 0 && needOneNoCast) {
                bool hasOnlyCasts = true;
                foreach (TypeMatch match in matches) {
                    if (!match.NeedsCast) {
                        hasOnlyCasts = false;
                        break;
                    }
                }
                if (hasOnlyCasts) return NoMatch;
            }

            return new(true, matches.ToArray());
        }

        /// <summary>True indicates there is a match, false otherwise.</summary>
        public readonly bool IsMatch;

        /// <summary>The type matched for the arguments for this function, maybe null.</summary>
        private TypeMatch[] matches;

        /// <summary>Creates a new function match results.</summary>
        /// <param name="isMatch">True if this is a match, false otherwise.</param>
        /// <param name="matches">These are the type matches for the arguments.</param>
        private FuncMatch(bool isMatch, TypeMatch[] matches) {
            this.IsMatch = isMatch;
            this.matches = matches;
        }

        /// <summary>This limits the comparison from -1 to 1 inclusive.</summary>
        /// <param name="cmp">The comparison to limit.</param>
        /// <returns>-1 to 1 inclusive comparison result.</returns>
        static private int limitCompare(int cmp) => cmp > 0 ? 1 : cmp < 0 ? -1 : 0;

        /// <summary>Compares this match with another match.</summary>
        /// <remarks>
        /// This does not compare IsMatch since it is assumed only matching types will be compared.
        /// This also assumes that the number of the type matches are te same in both function.
        /// </remarks>
        /// <param name="other">The other match to compare with.</param>
        /// <returns>-1 if match if this is less than the other, 0 if they match, 1 if this is greater than the other.</returns>
        public int CompareTo(FuncMatch other) {
            int length = this.matches.Length;
            int sum = 0;
            for (int i = 0; i < length; i++) {
                int cmp = this.matches[i].CompareTo(other.matches[i]);
                sum += limitCompare(cmp);
            }
            return limitCompare(sum);
        }

        /// <summary>Checks if this match is equal to the given object.</summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is FuncMatch other && this.CompareTo(other) == 0;

        /// <summary>Gets the hash code for this match.</summary>
        /// <returns>The hash code of the match.</returns>
        public override int GetHashCode() =>
            S.HashCode.Combine(this.IsMatch, this.matches);

        /// <summary>Gets the string for the match.</summary>
        /// <returns>The match's string.</returns>
        public override string ToString() => this.IsMatch ?
            "("+string.Join<TypeMatch>(", ", this.matches)+")" : "None";
    }
}
