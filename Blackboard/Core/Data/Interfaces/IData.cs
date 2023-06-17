namespace Blackboard.Core.Data.Interfaces;

/// <summary>Indicates when a class is a data type.</summary>
public interface IData {

    /// <summary>Gets the name for the type of data.</summary>
    /// <remarks>This should match the name in Types for the corresponding type entry.</remarks>
    public string TypeName { get; }

    /// <summary>Get the value of the data as a string.</summary>
    public string ValueAsString { get; }

    /// <summary>Get the value of the data as an object.</summary>
    public object? ValueAsObject { get; }
}
