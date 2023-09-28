namespace Blackboard.Core.Data.Interfaces;

/// <summary>This indicates that this Blackboard data type can be divided by another of the same type.</summary>
/// <remarks>The division denominator identity is the same as the multiplication identity.</remarks>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
/// <see cref="https://en.wikipedia.org/wiki/Division_(mathematics)"/>
internal interface IDivisible<T> : IMultiplicative<T>
    where T : IData {

    /// <summary>Gets the division of this value and the other value.</summary>
    /// <param name="other">The value to divide this value with.</param>
    /// <returns>This value divided by the other value, or NaN if the denominator is zero.</returns>
    T Div(T other);

    /// <summary>Gets the modulo of this value and the other value.</summary>
    /// <param name="other">The value to mod this value with.</param>
    /// <returns>The modulo of this value and the other value.</returns>
    T Mod(T other);
}
