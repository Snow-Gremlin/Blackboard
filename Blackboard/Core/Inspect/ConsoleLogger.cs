using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a console logger used for debugging the evaluation
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class ConsoleLogger: Logger {

        /// <summary>Creates a new console logger.</summary>
        public ConsoleLogger() { }

        protected override bool ProcessEntry(Entry entry) {
            S.Console.Write(entry.ToString());
            return true;
        }

        /// <summary>This will get the name of the logger.</summary>
        /// <returns>The name of the logger.</returns>
        public override string ToString() => "ConsoleLogger";
    }
}
