using S = System;

namespace Blackboard.Core {

    /// <summary>The exceptions for the core of blackboard.</summary>
    public class Exception: System.Exception {

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
            string strVal = value?.ToString() ?? "null";
            // TODO: Figure out a better way to make this show up in unit-tests.
            //this.Data.Add(key, strVal);
            //return this;
            return new Exception(this.Message + " [" + key + ": " + strVal + "]");
        }
    }
}
