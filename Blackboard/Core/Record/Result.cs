using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using PetiteParser.Formatting;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Record;

/// <summary>The results from a formula being performed.</summary>
sealed public class Result : IReader, IWriter {
    private readonly Dictionary<string, object> outputData;

    /// <summary>Creates a new results object.</summary>
    internal Result() => this.outputData = new();

    /// <summary>Indicates if there are any outputs.</summary>
    public bool HasOutput => this.AllOutputNames().Any();

    /// <summary>Gets the number of output objects in the results.</summary>
    public int OutputCount => this.AllOutputNames().Count();

    /// <summary>Gets the output names found at the given path.</summary>
    /// <param name="names">The names of the path to get the names from.</param>
    /// <returns>The names at the given depth of the path.</returns>
    public IEnumerable<string> OutputNames(params string[] names) =>
        this.OutputNames(names as IEnumerable<string>);
    
    /// <summary>Gets the output names found at the given path.</summary>
    /// <param name="names">The names of the path to get the names from.</param>
    /// <returns>The names at the given depth of the path.</returns>
    public IEnumerable<string> OutputNames(IEnumerable<string> names) {
        Dictionary<string, object> dic = this.outputData;
        foreach (string name in names) {
            dic = dic.TryGetValue(name, out object? value) ?
                value is Dictionary<string, object> next ? next :
                    throw new Message("Path does not exist. Name in path is not a namespace.").
                        With("Name", name).
                        With("Path", names.Join(".")).
                        With("Found", value) :
                throw new Message("Path does not exist. Name in path is missing.").
                    With("Name", name).
                    With("Path", names.Join("."));
        }
        return dic.Keys;
    }

    /// <summary>Gets the full names for all the outputs.</summary>
    /// <returns>The output's full names.</returns>
    public IEnumerable<string> AllOutputNames() =>
        this.allOutputs().Select(p => p.name);

    /// <summary>Gets the full names for all the outputs.</summary>
    /// <returns>The output's full names.</returns>
    private IEnumerable<(string name, object value)> allOutputs() {
        static IEnumerable<(string, object)> namesInDic(Dictionary<string, object> outputData, string prefix) {
            foreach (KeyValuePair<string, object> pair in outputData) {
                if (pair.Value is Dictionary<string, object> dic) {
                    foreach ((string, object) pair2 in namesInDic(dic, prefix+pair.Key+"."))
                        yield return pair2;
                } else yield return (prefix+pair.Key, pair.Value);
            }
        }
        return namesInDic(this.outputData, "");
    }

    /// <summary>Tries to get data with the given name.</summary>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <param name="data">The output data or null if not found.</param>
    /// <returns>True if found, false otherwise.</returns>
    public bool TryGetData(IEnumerable<string> names, out IData? data) {
        (Dictionary<string, object> dic, string lastName) = this.find(names);
        if (dic.TryGetValue(lastName, out object? v) && v is IData value) {
            data = value;
            return true;
        }
        data = null;
        return false;
    }

    /// <summary>Tries to get provoke state with the given name.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <param name="provoked">True if provoked, false otherwise, null if not found.</param>
    /// <returns>True if the trigger node exists, false otherwise.</returns>
    public bool TryGetProvoked(IEnumerable<string> names, out bool provoked) {
        (Dictionary<string, object> dic, string lastName) = this.find(names);
        if (dic.TryGetValue(lastName, out object? v) && v is bool p) {
            provoked = p;
            return true;
        }
        provoked = false;
        return false;
    }
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <typeparam name="T">The type of the value to set to the input.</typeparam>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    public void SetValue<T>(T value, IEnumerable<string> names) where T : IData {
        (Dictionary<string, object> dic, string lastName) = this.find(names, true);
        if (dic.ContainsKey(lastName))
            throw new Message("Value already exists so can not be set.").
                With("Path", names.Join(".")).
                With("Value", value);
        dic[lastName] = value;
    }

    /// <summary>This will provoke the node with the given name.</summary>
    /// <param name="names">The name of trigger node to provoke.</param>
    /// <param name="provoke">True to provoke, false to reset.</param>
    public void SetTrigger(IEnumerable<string> names, bool provoke = true) {
        (Dictionary<string, object> dic, string lastName) = this.find(names, true);
        if (dic.ContainsKey(lastName))
            throw new Message("Value already exists so can not be provoked.").
                With("Path", names.Join("."));
        dic[lastName] = provoke;
    }

    /// <summary>Finds the last namespace and last name in the path.</summary>
    /// <param name="names">The names in the path.</param>
    /// <param name="createMissing">True to create missing namespaces, false otherwise.</param>
    /// <returns>The last namespace and name that is found.</returns>
    private (Dictionary<string, object>, string) find(IEnumerable<string> names, bool createMissing = false) {
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
        return !first ? (dic, lastName) :
            throw new Message("Path was empty.");
    }

    /// <summary>Remove the value at the given path if it exists.</summary>
    /// <param name="names">The names in the path to remove.</param>
    /// <returns>True if the value was removed, false otherwise.</returns>
    public bool Remove(IEnumerable<string> names) {
        Dictionary<string, object> dic = this.outputData;
        bool first = true;
        string lastName = "";
        foreach (string name in names) {
            if (first) first = false;
            else if (dic.TryGetValue(lastName, out object? value) &&
                value is Dictionary<string, object> next) dic = next;
            else return false;
            lastName = name;
        }
        return !first ? dic.Remove(lastName) :
            throw new Message("Path was empty.");
    }

    /// <summary>Gets the string for the value stored in the results.</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static private string valueToString(object value) =>
        value switch {
            IData data     => data.ValueAsString,
            bool  provoked => provoked ? "provoked" : "unprovoked",
            _              => "unknown"
        };

    /// <summary>Gets a string with all the outputs in the results.</summary>
    /// <returns>The outputs as a string for this result.</returns>
    public override string ToString() {
        List<string> lines = new();
        foreach ((string name, object value) in this.allOutputs())
            lines.Add(name+" = "+valueToString(value));
        return lines.JoinLines();
    }
}
