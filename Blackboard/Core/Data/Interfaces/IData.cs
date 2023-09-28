using Blackboard.Core.Types;

namespace Blackboard.Core.Data.Interfaces;

/// <summary>Indicates when a class is a data type.</summary>
internal interface IData {

    /// <summary>Gets the type for the type of data.</summary>
    public Type Type { get; }

    /// <summary>Get the value of the data as a string.</summary>
    public string ValueAsString { get; }

    /// <summary>Get the value of the data as an object.</summary>
    public object? ValueAsObject { get; }
}
