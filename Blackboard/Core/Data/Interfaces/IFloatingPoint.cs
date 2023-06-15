using Blackboard.Core.Data.Caps;
using S = System;

namespace Blackboard.Core.Data.Interfaces;

/// <summary>This indicates that this Blackboard data type is a floating point value.</summary>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
public interface IFloatingPoint<T> : IData
    where T : IData {

    /// <summary>This gets the linear interpolation between to points using this value as a factor.</summary>
    /// <param name="min">The minimum value for a factor of zero or less.</param>
    /// <param name="max">The maximum value for a factor of one or more.</param>
    /// <returns>The result of the linear interpolation.</returns>
    T Lerp(T min, T max);

    /// <summary>This rounds this value to the given decimals.</summary>
    /// <param name="decimals">The integer value to round to.</param>
    /// <returns>This value rounded to the given decimals.</returns>
    T Round(Int decimals);

    /// <summary>This performs the given function on this value.</summary>
    /// <param name="func">The function to run on this value.</param>
    /// <returns>The resulting value from this value being used in the given function.</returns>
    T DoubleMath(S.Func<double, double> func);

    /// <summary>This performs the given function on this value.</summary>
    /// <param name="other">The value to use as the second input to the function.</param>
    /// <param name="func">The function to run on this and the given value.</param>
    /// <returns>The resulting value from this and the other value being used in the given function.</returns>
    T DoubleMath(T other, S.Func<double, double, double> func);

    /// <summary>Determines if this value is positive or negative infinity.</summary>
    /// <returns>True if the number is either positive or negative infinity, false otherwise.</returns>
    Bool IsInfinity();

    /// <summary>Determines if this value is not a number.</summary>
    /// <returns>True if the number is not a number, false otherwise.</returns>
    Bool IsNaN();

    /// <summary>Determines if the value is not infinite.</summary>
    /// <returns>True if the number is finite, false otherwise.</returns>
    Bool IsFinite();

    /// <summary>Determines if the value is positive infinity.</summary>
    /// <returns>True if the number is positive infinity, false otherwise.</returns>
    Bool IsPositiveInfinity();

    /// <summary>Determines if the value is negative infinity.</summary>
    /// <returns>True if the number is negative infinity, false otherwise.</returns>
    Bool IsNegativeInfinity();

    /// <summary>Determines if the value is in the normal range of doubles with full precision.</summary>
    /// <returns>True if the number is normal, false otherwise.</returns>
    /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
    Bool IsNormal();

    /// <summary>Determines if the value is not in the normal range of double so have reduced precision.</summary>
    /// <returns>True if the subnormal is normal, false otherwise.</returns>
    /// <see cref="https://en.wikipedia.org/wiki/Subnormal_number"/>
    Bool IsSubnormal();

    /// <summary>This gets the positive infinity value.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    T InfinityValue { get; }

    /// <summary>Determines if this value is not a number.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    T NaNValue { get; }
}
