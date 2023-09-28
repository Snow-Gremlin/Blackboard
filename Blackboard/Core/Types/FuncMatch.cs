using Blackboard.Core.Extensions;
using System.Linq;
using S = System;

namespace Blackboard.Core.Types;

/// <summary>This is a match for a single function.</summary>
/// <remarks>This is used for choosing the most specific method for a function.</remarks>
sealed public class FuncMatch : S.IComparable<FuncMatch> {

    /// <summary>Indicates there was no match for this function.</summary>
    static public readonly FuncMatch NoMatch = new(false, S.Array.Empty<TypeMatch>());

    /// <summary>Creates a new function match.</summary>
    /// <remarks>
    /// These matches are expected to already have been matched against the functions rules.
    /// For example these matches must all actually be matching results.
    /// </remarks>
    /// <param name="matches">The type matches for all the arguments of the function.</param>
    /// <returns>The new function match.</returns>
    static public FuncMatch Match(TypeMatch[] matches) => new(true, matches);

    /// <summary>True indicates there is a match, false otherwise.</summary>
    public readonly bool IsMatch;

    /// <summary>The type matched for the arguments for this function, maybe null.</summary>
    private readonly TypeMatch[] matches;

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
    /// This also assumes that the number of the type matches are the same in both function.
    /// </remarks>
    /// <param name="other">The other match to compare with.</param>
    /// <returns>-1 if match if this is less than the other, 0 if they match, 1 if this is greater than the other.</returns>
    public int CompareTo(FuncMatch? other) {
        if (other is null) return 1;
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
    public override bool Equals(object? obj) =>
        obj is FuncMatch other && this.CompareTo(other) == 0;

    /// <summary>Gets the hash code for this match.</summary>
    /// <returns>The hash code of the match.</returns>
    public override int GetHashCode() =>
        S.HashCode.Combine(this.IsMatch, this.matches);

    /// <summary>Gets the string for the match.</summary>
    /// <returns>The match's string.</returns>
    public override string ToString() => this.IsMatch ?
        "("+this.matches.Join(", ")+")" : "None";
}
