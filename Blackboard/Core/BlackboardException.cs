using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core;

/// <summary>This is an exception for Blackboard.</summary>
public class BlackboardException : S.Exception {

    /// <summary>Creates a new exception for the given message.</summary>
    /// <param name="message">The message to create an exception with.</param>
    /// <param name="inner">The inner exception from this message.</param>
    public BlackboardException(string message, S.Exception? inner = null) :
        base(message, inner) { }

    /// <summary>Creates a new exception for the given message.</summary>
    /// <remarks>Uses a shallow stringifier to get strings for the arguments.</remarks>
    /// <param name="message">The formatting message to create an exception with.</param>
    /// <param name="args">The arguments to fill in the formatting message.</param>
    public BlackboardException(string message, params object?[] args) :
        this(null, null, message, (IEnumerable<object?>)args) { }

    /// <summary>Creates a new exception for the given message.</summary>
    /// <param name="stringifier">The stringifier to get string for the arguments or shallow if null.</param>
    /// <param name="message">The formatting message to create an exception with.</param>
    /// <param name="args">The arguments to fill in the formatting message.</param>
    public BlackboardException(Stringifier? stringifier, string message, params object?[] args) :
        this(null, stringifier, message, args) { }

    /// <summary>Creates a new exception for the given message.</summary>
    /// <remarks>Uses a shallow stringifier to get strings for the arguments.</remarks>
    /// <param name="inner">The inner exception from this message.</param>
    /// <param name="message">The formatting message to create an exception with.</param>
    /// <param name="args">The arguments to fill in the formatting message.</param>
    public BlackboardException(S.Exception? inner, string message, params object?[] args) :
        this(inner, null, message, args) { }

    /// <summary>Creates a new exception for the given message.</summary>
    /// <param name="inner">The inner exception from this message.</param>
    /// <param name="stringifier">The stringifier to get string for the arguments or shallow if null.</param>
    /// <param name="message">The message to create an exception with.</param>
    /// <param name="args">The arguments to fill in the formatting message.</param>
    public BlackboardException(S.Exception? inner, Stringifier? stringifier, string message, params object?[] args) :
        this(inner, stringifier, message, (IEnumerable<object?>)args) { }

    /// <summary>Creates a new exception for the given message.</summary>
    /// <param name="inner">The inner exception from this message.</param>
    /// <param name="stringifier">The stringifier to get string for the arguments or shallow if null.</param>
    /// <param name="message">The message to create an exception with.</param>
    /// <param name="args">The arguments to fill in the formatting message.</param>
    public BlackboardException(S.Exception? inner, Stringifier? stringifier, string message, IEnumerable<object?> args) :
        base(string.Format(message, (stringifier ?? Stringifier.Shallow()).StringifyObject(args)), inner) { }

    /// <summary>Adds additional key value pair of data to this exception.</summary>
    /// <param name="key">The key for the additional data.</param>
    /// <param name="value">The value for the additional data.</param>
    /// <param name="stringifier">The stringifier to apply or null to use shallow.</param>
    /// <returns>This message so that these calls can be chained.</returns>
    public BlackboardException With(string key, object? value, Stringifier? stringifier = null) {
        stringifier ??= Stringifier.Shallow();
        this.Data.Add(key, stringifier.StringifyObject(value));
        return this;
    }

    /// <summary>Adds a collection of additional key value pair of data to this exception.</summary>
    /// <param name="data">The collection of key value pairs to add.</param>
    /// <param name="stringifier">The stringifier to apply or null to use shallow.</param>
    /// <returns>This exception so that these calls can be chained.</returns>
    public BlackboardException With(IEnumerable<(string key, object value)> data, Stringifier? stringifier = null) {
        stringifier ??= Stringifier.Shallow();
        foreach ((string key, object value) in data) this.With(key, value, stringifier);
        return this;
    }

    /// <summary>Gets the message for this exception.</summary>
    public override string Message {
        get {
            string msg = base.Message;
            if (this.Data.Count > 0) {
                List<string> parts = new();
                foreach (KeyValuePair<string, object?> pair in this.Data)
                    parts.Add(pair.Key + ": " + pair.Value);
                msg += " {" + parts.Join(", ") + "}";
            }
            return msg;
        }
    }
}
