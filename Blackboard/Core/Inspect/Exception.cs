using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>This is an exception for Blackboard.</summary>
    public class Exception: S.Exception {

        /// <summary>The full message which created this exception.</summary>
        public readonly Message FullMessage;

        /// <summary>Creates a new exception for the given message.</summary>
        /// <param name="message">The message to create an exception with.</param>
        /// <param name="inner">The inner exception from this message.</param>
        internal Exception(string message, S.Exception? inner = null) :
            this(new Message(message), inner) { }

        /// <summary>Creates a new exception for the given message.</summary>
        /// <remarks>This used when a message is implicitly casted to an exception.</remarks>
        /// <param name="message">The message to create an exception with.</param>
        /// <param name="inner">The inner exception from this message.</param>
        internal Exception(Message message, S.Exception? inner = null):
            base(message.ToString(), inner) {
            this.FullMessage = message;
            foreach ((string key, object? value) in message.Data)
                this.Data[key] = value;
        }

        /// <summary>Gets the string for this exception's message and return a new exception.</summary>
        /// <remarks>
        /// This will change the message inside of this exception and the new exception has the
        /// same message but the new exception will have an updated data and exception message string.
        /// </remarks>
        /// <param name="stringifier">The stringifier to stringify the message with.</param>
        /// <returns>A new exception with the message as a string.</returns>
        public Exception Stringify(Stringifier stringifier) =>
            this.FullMessage.Stringify(stringifier);
    }
}
