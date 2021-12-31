using System.Linq;
using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a console logger used for debugging the evaluation
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class ConsoleLogger: BaseLogger {

        /// <summary>Creates a new console logger.</summary>
        public ConsoleLogger() { }

        /// <summary>
        /// This will write to this log followed by a new line.
        /// The input has already been formatted, adjusted, and had a newline added.
        /// </summary>
        /// <param name="msg">The formatted and adjusted message to write.</param>
        protected override void AdjustedLog(string msg) => S.Console.Write(msg);

        /// <summary>This will get the name of the logger.</summary>
        /// <returns>The name of the logger.</returns>
        public override string ToString() => "ConsoleLogger";
    }
}
