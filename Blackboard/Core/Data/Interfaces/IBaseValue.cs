namespace Blackboard.Core.Data.Interfaces;

/// <summary>An interface for reading the base value.</summary>
/// <typeparam name="T">The C# base type in the data.</typeparam>
public interface IBaseValue<T> : IData {

    /// <summary>Gets the C# base value in the data.</summary>
    public T BaseValue { get; }
}
