namespace Blackboard.Core.Data.Interfaces;

/// <summary>This indicates that this Blackboard data type can get basic arithmetic values for a finite range.</summary>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
internal interface IFinite<out T> : IData
    where T : IData {

    /// <summary>Gets this additive identity, which is typically zero.</summary>
    T ZeroValue { get; }

    /// <summary>Gets this multiplicative identity, which is typically one.</summary>
    T OneValue { get; }

    /// <summary>Gets the minimum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The minimum data value.</returns>
    T MinValue { get; }

    /// <summary>Gets the maximum value for this data type.</summary>
    /// <remarks>The current value is not used when getting this identity.</remarks>
    /// <returns>The maximum data value.</returns>
    T MaxValue { get; }
}
