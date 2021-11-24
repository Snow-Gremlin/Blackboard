using Blackboard.Core.Data.Caps;
using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type is a floating point value.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IFloatingPoint<T>: IArithmetic<T>
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
        Bool IsNAN();
    }
}
