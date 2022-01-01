using Blackboard.Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core {

    /// <summary>The exceptions for the core of blackboard.</summary>
    public class Exception: S.Exception {

        /// <summary>Creates a new exception.</summary>
        /// <param name="message">The message for this exception.</param>
        public Exception(string message) : base(message) { }

        /// <summary>Creates a new exception.</summary>
        /// <param name="message">The message for this exception.</param>
        /// <param name="inner">The inner exception to this exception.</param>
        public Exception(string message, S.Exception inner) : base(message, inner) { }

        /// <summary>Adds additional key value pair of data to this exception.</summary>
        /// <param name="key">The key for the additional data.</param>
        /// <param name="value">The value for the additional data.</param>
        /// <returns>This exception so that these calls can be chained.</returns>
        public Exception With(string key, object value) {
            this.Data.Add(key, value);
            return this;
        }

        /// <summary>Creates a single string will all the exception message and data.</summary>
        /// <remarks>This is useful for logging and debugging exceptions which contain data.</remarks>
        /// <returns>The string for the whole exception.</returns>
        public override string ToString() {
            List<string> lines = new();
            S.Exception ex = this;
            while (ex != null) {
                lines.Add(ex.Message);
                foreach (DictionaryEntry entry in ex.Data) {
                    string strKey = entry.Key?.ToString() ?? "null";
                    string strVal = entry.Value?.ToString() ?? "null";
                    lines.Add("[" + strKey + ": " + strVal + "]");
                }
                ex = ex.InnerException;
            }
            return lines.Join("\n");
        }
    }
}
