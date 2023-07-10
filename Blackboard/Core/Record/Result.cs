using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Result;
using System.Collections.Generic;

namespace Blackboard.Core.Record;

/// <summary>The results from a formula being performed.</summary>
sealed public class Result : IReader {
    private readonly Dictionary<string, object> outputData;

    /// <summary>Creates a new results object.</summary>
    internal Result() => outputData = new();

    /// <summary>Sets the value of an named output.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="name">The name given to the output data.</param>
    /// <returns>The value from the output.</returns>
    public void SetValue<T>(string name, T value) where T : IData =>
        outputData[name] = value;
    
    /// <summary>Tries to get data with the given name.</summary>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <param name="data">The output data or null if not found.</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool TryGetData(IEnumerable<string> names, out IData? data) =>
        throw new System.NotImplementedException();
}
