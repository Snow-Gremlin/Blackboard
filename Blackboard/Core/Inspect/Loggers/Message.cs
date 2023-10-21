using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Inspect.Loggers;

/// <summary>
/// Message is a common object used for exceptions and logging which carries
/// a string of text for the message and additional information for the message.
/// </summary>
sealed public class Message {

    /// <summary>Creates a new message with the given format text and arguments.</summary>
    /// <param name="format">The format text to fill in with the given arguments.</param>
    /// <param name="args">The arguments for the format text.</param>
    public Message(string format, params object?[] args) {
        this.Format    = format;
        this.Arguments = args;
        this.Data      = new();
    }

    /// <summary>Creates a copy of this message.</summary>
    /// <remarks>
    /// This can only make shallow copies of the objects in the arguments and data.
    /// Be careful to not modify the objects in the arguments and data because the
    /// change would effect the clone as well.
    /// </remarks>
    /// <returns>The copied message.</returns>
    public Message Clone() {
        Message copy = new(this.Format, this.Arguments.Clone());
        foreach ((string key, object? value) in this.Data)
            copy.Data[key] = value;
        return copy;
    }

    /// <summary>The format text for the message.</summary>
    public string Format;

    /// <summary>The arguments for the format test of the message.</summary>
    public object?[] Arguments;

    /// <summary>The data for the message.</summary>
    public readonly Dictionary<string, object?> Data;

    /// <summary>This gets the text of the message with the arguments mixed in.</summary>
    public string Text => string.Format(this.Format, this.Arguments);

    /// <summary>Adds additional key value pair of data to this message.</summary>
    /// <param name="key">The key for the additional data.</param>
    /// <param name="value">The value for the additional data.</param>
    /// <returns>This message so that these calls can be chained.</returns>
    public Message With(string key, object? value) {
        this.Data.Add(key, value);
        return this;
    }

    /// <summary>Adds a collection of additional key value pair of data to this message.</summary>
    /// <param name="data">The collection of key value pairs to add.</param>
    /// <returns>This message so that these calls can be chained.</returns>
    public Message With(IEnumerable<(string key, object value)> data) {
        foreach ((string key, object value) in data)
            this.Data.Add(key, value);
        return this;
    }

    /// <summary>This applies the given stringifier to the arguments and data.</summary>
    /// <param name="stringifier">The stringifier to apply or null to use shallow.</param>
    /// <returns>The logger which will stringify values in the message.</returns>
    internal Message Stringify(Stringifier? stringifier = null) {
        stringifier ??= Stringifier.Shallow();
        for (int i = this.Arguments.Length-1; i >= 0; --i)
            this.Arguments[i] = stringifier.StringifyObject(this.Arguments[i]);
        foreach ((string key, object? value) in this.Data)
            this.Data[key] = stringifier.StringifyObject(value);
        return this;
    }

    /// <summary>Creates a single string will all the message and data.</summary>
    /// <remarks>This is useful for logging and debugging exceptions which contain data.</remarks>
    /// <returns>The string for the whole message.</returns>
    public override string ToString() {
        List<string> lines = new() { this.Text };
        foreach ((string key, object? value) in this.Data) {
            string strKey = key?.ToString() ?? "null";
            string strVal = value switch {
                null          => "null",
                S.Exception e => e.Message,
                Message   msg => msg.ToString(),
                _             => value.ToString() ?? "null",
            };
            strVal = strVal.Replace("\n", "\n   ");
            lines.Add("[" + strKey + ": " + strVal + "]");
        }
        return lines.Join("\n");
    }
}
