using Blackboard.Core.Data.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Record;

/// <summary>An object in a tree of values which can be written to.</summary>
public interface IWriter {

    /// <summary>Sets a value for the given named input.</summary>
    /// <typeparam name="T">The type of the value to set to the input.</typeparam>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    public void SetValue<T>(T value, IEnumerable<string> names) where T : IData;

    /// <summary>This will provoke the node with the given name.</summary>
    /// <param name="names">The name of trigger node to provoke.</param>
    public void Provoke(IEnumerable<string> names);
}
