using Blackboard.Core.Data.Caps;
using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type is a floating point value.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IFloatingPoint<T>: IData
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

        /// <summary>This gets the Atan2 where this value is the Y input.</summary>
        /// <param name="x">This is the X input value.</param>
        /// <returns>The Atan2 of this and the other value.</returns>
        T Atan2(T x);

        /// <summary>This gets the logarithm of this value using the other value as the base.</summary>
        /// <param name="newBase">The value to use as the base of the log.</param>
        /// <returns>The result of the logarithm.</returns>
        T Log(T newBase);

        /// <summary>This performs the given function on this value.</summary>
        /// <param name="func">The function to run on this value.</param>
        /// <returns>The resulting value from this value being used in the given function.</returns>
        T DoubleMath(S.Func<double, double> func);
    }
}
