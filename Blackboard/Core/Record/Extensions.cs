using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using System.Collections.Generic;

namespace Blackboard.Core.Record;

/// <summary>Extensions for the value reader interface.</summary>
static public class Extensions {
    #region Type

    /// <summary>Gets the type of the value at the given data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to get the type of.</param>
    /// <returns>The type of the data or null if doesn't exist or not a data type.</returns>
    static public Type? GetType(this IReader reader, params string[] names) =>
        reader.GetType(names as IEnumerable<string>);

    /// <summary>Gets the type of the value at the given data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to get the type of.</param>
    /// <returns>The type of the data or null if doesn't exist or not a data type.</returns>
    static public Type? GetType(this IReader reader, IEnumerable<string> names) =>
        reader.TryGetData(names, out IData? data) && data is not null ?
            Type.FromName(data.Type.Name) : null;

    #endregion
    #region Bool

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasBool(this IReader reader, params string[] names) =>
        reader.HasBool(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasBool(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Bool>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public bool GetBool(this IReader reader, params string[] names) =>
        reader.GetBool(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public bool GetBool(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Bool>(names).Value;
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetBool(this IWriter writer, bool value, params string[] names) =>
        writer.SetBool(value, names as IEnumerable<string>);

    /// <summary>Sets a value for the given named input.</summary>
    /// <remarks>This will not cause an evaluation, if the value changed then updates will be pended.</remarks>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetBool(this IWriter writer, bool value, IEnumerable<string> names) =>
        writer.SetValue(new Bool(value), names);

    #endregion
    #region Int

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasInt(this IReader reader, params string[] names) =>
        reader.HasInt(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasInt(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Int>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public int GetInt(this IReader reader, params string[] names) =>
        reader.GetInt(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public int GetInt(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Int>(names).Value;

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetInt(this IWriter writer, int value, params string[] names) =>
        writer.SetInt(value, names as IEnumerable<string>);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetInt(this IWriter writer, int value, IEnumerable<string> names) =>
        writer.SetValue(new Int(value), names);

    #endregion
    #region Uint

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasUint(this IReader reader, params string[] names) =>
        reader.HasUint(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasUint(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Uint>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public uint GetUint(this IReader reader, params string[] names) =>
        reader.GetUint(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public uint GetUint(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Uint>(names).Value;

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetUint(this IWriter writer, uint value, params string[] names) =>
        writer.SetUint(value, names as IEnumerable<string>);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetUint(this IWriter writer, uint value, IEnumerable<string> names) =>
        writer.SetValue(new Uint(value), names);

    #endregion
    #region Double

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasDouble(this IReader reader, params string[] names) =>
        reader.HasDouble(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasDouble(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Double>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public double GetDouble(this IReader reader, params string[] names) =>
        reader.GetDouble(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public double GetDouble(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Double>(names).Value;
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetDouble(this IWriter writer, double value, params string[] names) =>
        writer.SetDouble(value, names as IEnumerable<string>);
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetDouble(this IWriter writer, double value, IEnumerable<string> names) =>
        writer.SetValue(new Double(value), names);

    #endregion
    #region Float

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasFloat(this IReader reader, params string[] names) =>
        reader.HasFloat(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasFloat(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Float>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public float GetFloat(this IReader reader, params string[] names) =>
        reader.GetFloat(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public float GetFloat(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Float>(names).Value;
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetFloat(this IWriter writer, float value, params string[] names) =>
        writer.SetDouble(value, names as IEnumerable<string>);
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetFloat(this IWriter writer, float value, IEnumerable<string> names) =>
        writer.SetValue(new Float(value), names);

    #endregion
    #region String

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasString(this IReader reader, params string[] names) =>
        reader.HasString(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasString(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<String>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public string GetString(this IReader reader, params string[] names) =>
        reader.GetString(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public string GetString(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<String>(names).Value;

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetString(this IWriter writer, string value, params string[] names) =>
        writer.SetString(value, names as IEnumerable<string>);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetString(this IWriter writer, string value, IEnumerable<string> names) =>
        writer.SetValue(new String(value), names);

    #endregion
    #region Object

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasObject(this IReader reader, params string[] names) =>
        reader.HasObject(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasObject(this IReader reader, IEnumerable<string> names) =>
        reader.HasValue<Object>(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public object? GetObject(this IReader reader, params string[] names) =>
        reader.GetObject(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public object? GetObject(this IReader reader, IEnumerable<string> names) =>
        reader.GetValue<Object>(names).Value;
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetObject(this IWriter writer, object value, params string[] names) =>
        writer.SetObject(value, names as IEnumerable<string>);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetObject(this IWriter writer, object value, IEnumerable<string> names) =>
        writer.SetValue(new Object(value), names);

    #endregion
    #region Value As

    /// <summary>Gets the value as an object from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value as an object from the data.</returns>
    static public object? GetValueAsObject(this IReader reader, params string[] names) =>
        reader.GetValueAsObject(names as IEnumerable<string>);

    /// <summary>Gets the value as an object from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value as an object from the data.</returns>
    static public object? GetValueAsObject(this IReader reader, IEnumerable<string> names) =>
        reader.GetData(names).ValueAsObject;

    /// <summary>Gets the value as a string from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value as a string from the data.</returns>
    static public object? GetValueAsString(this IReader reader, params string[] names) =>
        reader.GetValueAsString(names as IEnumerable<string>);

    /// <summary>Gets the value as a string from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value as a string from the data.</returns>
    static public object? GetValueAsString(this IReader reader, IEnumerable<string> names) =>
        reader.GetData(names).ValueAsString;

    #endregion
    #region Value

    /// <summary>Indicates if some data exists for the given name and type.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasValue<T>(this IReader reader, params string[] names)
        where T : IData =>
        reader.HasValue<T>(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name and type.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasValue<T>(this IReader reader, IEnumerable<string> names)
        where T : IData =>
        reader.TryGetData(names, out IData? data) && data is T;

    /// <summary>Gets the value from a named data.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public T GetValue<T>(this IReader reader, params string[] names)
        where T : IData =>
        reader.GetValue<T>(names as IEnumerable<string>);

    /// <summary>Gets the value from a named data.</summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public T GetValue<T>(this IReader reader, IEnumerable<string> names)
        where T : IData {
        IData data = reader.GetData(names);
        return data is T value ? value :
            throw new BlackboardException("The data found by the given name is not the expected type.").
                With("Name", names.Join(".")).
                With("Found Type", data.GetType()).
                With("Expected Type", typeof(T));
    }

    /// <summary>Sets a value for the given named input.</summary>
    /// <typeparam name="T">The type of the value to set to the input.</typeparam>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetValue<T>(this IWriter writer, T value, params string[] names) where T : IData =>
        writer.SetValue(value, names);

    #endregion
    #region Data

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasData(this IReader reader, params string[] names) =>
        reader.HasData(names as IEnumerable<string>);

    /// <summary>Indicates if some data exists for the given name.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to check.</param>
    /// <returns>True if the data exists, false otherwise.</returns>
    static public bool HasData(this IReader reader, IEnumerable<string> names) =>
        reader.TryGetData(names, out IData? _);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public IData GetData(this IReader reader, params string[] names) =>
        reader.GetData(names);

    /// <summary>Gets the value from a named data.</summary>
    /// <param name="reader">The value reader to read from.</param>
    /// <param name="names">The name of the data to read the value from.</param>
    /// <returns>The value from the data.</returns>
    static public IData GetData(this IReader reader, IEnumerable<string> names) =>
        reader.TryGetData(names, out IData? data) && data is not null ? data :
            throw new BlackboardException("Unable to get data by the given name.").
                With("Name", names.Join("."));

    #endregion
    #region Provoked

    /// <summary>Indicates if the provoked flag by the given name exists.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a provoked flag exists, false otherwise.</returns>
    static public bool HasProvoked(this IReader reader, params string[] names) =>
        reader.HasProvoked(names as IEnumerable<string>);
    
    /// <summary>Indicates if the trigger by the given name exists.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a provoked flag exists, false otherwise.</returns>
    static public bool HasProvoked(this IReader reader, IEnumerable<string> names) =>
        reader.TryGetProvoked(names, out bool _);

    /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
    static public bool Provoked(this IReader reader, params string[] names) =>
        reader.Provoked(names as IEnumerable<string>);

    /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
    static public bool Provoked(this IReader reader, IEnumerable<string> names) =>
        reader.TryGetProvoked(names, out bool provoked) ? provoked :
            throw new BlackboardException("Unable to get provoked state by the given name.").
                With("Name", names.Join("."));

    /// <summary>This will provoke the node with the given name.</summary>
    /// <param name="writer">The writer to provoke a trigger in.</param>
    /// <param name="provoke">True to provoke, false to reset.</param>
    /// <param name="names">The name of trigger node to provoke.</param>
    static public void SetTrigger(this IWriter writer, bool provoke, params string[] names) =>
        writer.SetTrigger(names, provoke);

    #endregion
    #region Node

    /// <summary>Determines if the node with the given name exists.</summary>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>True if a node by the given name exists, false otherwise</returns>
    static public bool HasNode(this INodeReader reader, params string[] names) =>
        reader.HasNode(names as IEnumerable<string>);

    /// <summary>Determines if the node with the given name exists.</summary>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>True if a node by the given name exists, false otherwise</returns>
    static public bool HasNode(this INodeReader reader, IEnumerable<string> names) =>
        reader.TryGetNode(names, out INode? _);
    
    /// <summary>Determines if the node with the given name and type exists.</summary>
    /// <typeparam name="T">The type of the node to check for.</typeparam>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>True if a node by the given name exists, false otherwise</returns>
    static public bool HasNode<T>(this INodeReader reader, params string[] names) where T : INode =>
        reader.HasNode<T>(names as IEnumerable<string>);

    /// <summary>Determines if the node with the given name and type exists.</summary>
    /// <typeparam name="T">The type of the node to check for.</typeparam>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>True if a node by the given name exists, false otherwise</returns>
    static public bool HasNode<T>(this INodeReader reader, IEnumerable<string> names) where T : INode =>
        reader.TryGetNode(names, out INode? node) && node is T;

    /// <summary>Gets the node with the given name.</summary>
    /// <typeparam name="T">The expected type of node to get.</typeparam>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>The node with the given name and type.</returns>
    static public T GetNode<T>(this INodeReader reader, params string[] names) where T : INode =>
        reader.GetNode<T>(names as IEnumerable<string>);

    /// <summary>Gets the node with the given name.</summary>
    /// <remarks>This will throw an exception if no node by that name exists or the found node is the incorrect type.</remarks>
    /// <typeparam name="T">The expected type of node to get.</typeparam>
    /// <param name="names">The name of the node to get.</param>
    /// <returns>The node with the given name and type.</returns>
    static public T GetNode<T>(this INodeReader reader, IEnumerable<string> names) where T : INode =>
        reader.TryGetNode(names, out INode? node) ?
            node is T result ? result :
            throw new BlackboardException("The node found by the given name is not the expected type.").
                With("Name", names.Join(".")).
                With("Found", node).
                With("Expected Type", typeof(T)) :
            throw new BlackboardException("Unable to get a node by the given name.").
                With("Name", names.Join(".")).
                With("Value Type", typeof(T));

    /// <summary>Tries to get the node with the given node.</summary>
    /// <param name="names">The name of the node to get.</param>
    /// <param name="node">The returned node for the given name or null.</param>
    /// <returns>True if the node was found, false otherwise.</returns>
    static public bool TryGetNode(this INodeReader reader, out INode? node, params string[] names) =>
        reader.TryGetNode(names, out node);

    #endregion
}
