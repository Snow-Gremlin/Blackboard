namespace Blackboard.Core.Data.Interfaces;

/// <summary>An interface for reading the base value.</summary>
/// <typeparam name="T1">The type of data that this part of.</typeparam>
/// <typeparam name="T2">The C# base type in the data.</typeparam>
internal interface IBaseValue<T1, T2> : IData
    where T1 : IData, IBaseValue<T1, T2> {

    /// <summary>Gets the C# base value in the data.</summary>
    public T2 BaseValue { get; }

    /// <summary>This creates a new instance of the data with the given value.</summary>
    /// <param name="baseValue">The value to create data for.</param>
    /// <returns>The new data set for the given value.</returns>
    public T1 Wrap(T2 baseValue);
}
