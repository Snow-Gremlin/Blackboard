using S = System;

namespace BlackboardTools {

    /// <summary>This is an exception for expected exceptions.</summary>
    public class CommandException: S.Exception {
        
        /// <summary>Creates a new exception.</summary>
        /// <param name="message">The message for the exception.</param>
        public CommandException(string message): base(message) { }
    }
}
