using Blackboard.Core.Data.Caps;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can do basic arithmetic.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IArithmetic<T>: IAdditive<T>
        where T : IData {

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        T Abs();

        /// <summary>Gets the absolute value of this data value.</summary>
        /// <returns>The absolute value of this value.</returns>
        T Neg();

        /// <summary>Gets this value incremented by one.</summary>
        /// <returns>This data value plus one.</returns>
        T Inc();

        /// <summary>Gets the difference between this value and the other value.</summary>
        /// <param name="other">The value to subtract from this value.</param>
        /// <returns>The difference between this value and the other value.</returns>
        T Sub(T other);

        /// <summary>Gets the product of this value and the other value.</summary>
        /// <param name="other">The value to multiply this value with.</param>
        /// <returns>The product of this value and the other value.</returns>
        T Mul(T other);

        /// <summary>Gets the division of this value and the other value.</summary>
        /// <param name="other">The value to divide this value with.</param>
        /// <returns>This value divided by the other value.</returns>
        T Div(T other);

        /// <summary>Gets the modulo of this value and the other value.</summary>
        /// <param name="other">The value to mod this value with.</param>
        /// <returns>The modulo of this value and the other value.</returns>
        T Mod(T other);

        /// <summary>Gets this value clamped to the inclusive range of the given min and max.</summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The value clamped between the given values.</returns>
        T Clamp(T min, T max);

        /// <summary>Determines if the this value is negative.</summary>
        /// <returns>True if below zero, false if zero or more.</returns>
        Bool IsNegative();
    }
}
