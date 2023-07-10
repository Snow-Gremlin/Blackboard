﻿using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Record;
using Blackboard.Core.Result;
using Blackboard.Core.Types;
using System.Collections.Generic;

namespace Blackboard.Core.Extensions;

/// <summary>Extensions for the value reader interface.</summary>
static public class RecordExt {
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
            Type.FromName(data.TypeName) : null;

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
        writer.SetBool(value, names);

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
        writer.SetInt(value, names);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetInt(this IWriter writer, int value, IEnumerable<string> names) =>
        writer.SetValue(new Int(value), names);

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
        writer.SetDouble(value, names);
    
    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetDouble(this IWriter writer, double value, IEnumerable<string> names) =>
        writer.SetValue(new Double(value), names);

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
    static public void SetString(this IWriter writer, string value, IEnumerable<string> names) =>
        writer.SetString(value, names);

    /// <summary>Sets a value for the given named input.</summary>
    /// <param name="writer">The writer to set the value to.</param>
    /// <param name="value">The value to set to that node.</param>
    /// <param name="names">The name of the input node to set.</param>
    static public void SetString(this IWriter writer, string value, params string[] names) =>
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
        writer.SetObject(value, names);

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
            throw new Message("The data found by the given name is not the expected type.").
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
            throw new Message("Unable to get data by the given name.").
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
        reader.GetNode<ITrigger>(names).Provoked;

    
    /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
    static public bool Provoked(this IReader reader, params string[] names) =>
        reader.Provoked(names as IEnumerable<string>);

    /// <summary>Indicates if the trigger is currently provoked while waiting to be evaluated.</summary>
    /// <param name="names">The name of trigger node to get the state from.</param>
    /// <returns>True if a node by that name is found and it is provoked, false otherwise.</returns>
    static public bool Provoked(this IReader reader, IEnumerable<string> names) =>
        reader.GetNode<ITrigger>(names).Provoked;


    /// <summary>This will provoke the node with the given name.</summary>
    /// <param name="writer">The writer to provoke a trigger in.</param>
    /// <param name="names">The name of trigger node to provoke.</param>
    static public void Provoke(this IWriter writer, params string[] names) =>
        writer.Provoke(names as IEnumerable<string>);

    #endregion
}