using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// Message is a common object used for exceptions and logging which carries
    /// a string of text for the message and additional information for the message.
    /// </summary>
    public class Message {

        /// <summary>Creates a new message with the given format text and arguments.</summary>
        /// <param name="format">The format text to fill in with the given arguments.</param>
        /// <param name="args">The arguments for the format text.</param>
        public Message(string format, params object[] args) {
            this.Format = format;
            this.Arguments = args;
            this.Data = new Dictionary<string, object>();
        }

        /// <summary>The format text for the message.</summary>
        public readonly string Format;

        /// <summary>The arguments for the format test of the message.</summary>
        public readonly object[] Arguments;

        /// <summary>The data for the message.</summary>
        public readonly Dictionary<string, object> Data;

        /// <summary>This gets the text of the message with the arguments mixed in.</summary>
        public string Text => string.Format(this.Format, this.Arguments);

        /// <summary>Adds additional key value pair of data to this exception.</summary>
        /// <param name="key">The key for the additional data.</param>
        /// <param name="value">The value for the additional data.</param>
        /// <returns>This exception so that these calls can be chained.</returns>
        public Message With(string key, object value) {
            this.Data.Add(key, value);
            return this;
        }

        /// <summary>Creates a single string will all the exception message and data.</summary>
        /// <remarks>This is useful for logging and debugging exceptions which contain data.</remarks>
        /// <returns>The string for the whole exception.</returns>
        public override string ToString() {
            List<string> lines = new();
            lines.Add(this.Text);
            foreach ((string key, object value) in this.Data) {
                string strKey = key?.ToString() ?? "null";
                string strVal = value?.ToString() ?? "null";
                strVal = strVal.Replace("\n", "\n   ");
                lines.Add("[" + strKey + ": " + strVal + "]");
            }
            return lines.Join("\n");
        }

        /// <summary>Implicitly convert to an exception when needed.</summary>
        /// <param name="msg">The message to convert.</param>
        public static implicit operator S.Exception(Message msg) {
            // Because VS test explorer doesn't post the data,
            // use the whole message with the data as the exception's message.
            S.Exception exp = new(msg.ToString());
            exp.Data["Message"] = msg.Text;
            foreach ((string key, object value) in msg.Data)
                exp.Data[key] = value;
            return exp;
        }
    }
}
