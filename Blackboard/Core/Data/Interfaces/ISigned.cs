using Blackboard.Core.Data.Caps;

namespace Blackboard.Core.Data.Interfaces;

/// <summary>This indicates that this Blackboard data type is signed so can be negative.</summary>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
internal interface ISigned<out T> : IData
    where T : IData {

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    T Abs();

    /// <summary>Gets the absolute value of this data value.</summary>
    /// <returns>The absolute value of this value.</returns>
    T Neg();

    /// <summary>Determines if the this value is negative.</summary>
    /// <returns>True if below zero, false if zero or more.</returns>
    Bool IsNegative();
}
