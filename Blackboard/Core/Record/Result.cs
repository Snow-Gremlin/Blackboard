using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Result;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Blackboard.Core.Record;

/// <summary>The results from a formula being performed.</summary>
sealed public class Result : IReader, IWriter {
    private readonly Dictionary<string, object> outputData;

    /// <summary>Creates a new results object.</summary>
    internal Result() => outputData = new();

    /// <summary>Tries to get provoke state with the given name.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <param name="provoked">True if provoked, false otherwise, null if not found.</param>
    /// <returns>True if the trigger node exists, false otherwise.</returns>
    public bool TryGetProvoked(IEnumerable<string> names, out bool provoked) {
        provoked = false;
        if (!this.TryGetNode(names, out INode? node)) return false;
        if (node is not ITrigger trigger) return false;
        provoked = trigger.Provoked;
        return true;
    }
        
    /// <summary>Tries to get data with the given name.</summary>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <param name="data">The output data or null if not found.</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool TryGetData(IEnumerable<string> names, out IData? data) {
        data = null;
        if (!this.TryGetNode(names, out INode? node)) return false;
        data = (node as IValue<IData>)?.Data;
        return data is not null;
    }
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <remarks>
    /// This will not cause an evaluation,
    /// if the value changed then updates will be pended.
    /// </remarks>
    /// <typeparam name="T">The type of the value to set to the input.</typeparam>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    public void SetValue<T>(T value, IEnumerable<string> names) where T : IData =>
        this.SetValue(value, this.GetNode<IValueInput<T>>(names));
    
    /// <summary>This will provoke the node with the given name.</summary>
    /// <remarks>
    /// This will not cause an evaluation,
    /// if the value changed then updates will be pended.
    /// </remarks>
    /// <param name="names">The name of trigger node to provoke.</param>
    public void Provoke(IEnumerable<string> names) =>
        this.Provoke(this.GetNode<ITriggerInput>(names));


    private (Dictionary<string, object>, string) find(IEnumerable<string> names, bool createMissing) {
        Dictionary<string, object> dic = this.outputData;
        bool first = true;
        string lastName = "";
        foreach (string name in names) {
            if (first) first = false;
            else {
                if (dic.TryGetValue(lastName, out object? value)) {
                    dic = value is Dictionary<string, object> next ? next :
                        throw new Message("Path does not exist. Name in path is not a namespace.").
                            With("Name", lastName).
                            With("Path", names.Join(".")).
                            With("Found", value);
                } else if (createMissing) {
                    Dictionary<string, object> next = new();
                    dic.Add(lastName, next);
                    dic = next;
                } else throw new Message("Path does not exist. Name in path is missing.").
                    With("Name", lastName).
                    With("Path", names.Join("."));
            }
            lastName = name;
        }
        return (dic, lastName);
    }
}
