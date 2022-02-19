using System.Linq;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// An abstract logger to simplify making other loggers for debugging the evaluation,
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class Logger {

        private readonly Logger inner;

        /// <summary>Creates a new base logger.</summary>
        /// <param name="inner">The next logger the entry is passed to.</param>
        protected Logger(Logger inner = null) {
            this.inner = inner;
        }

        /// <summary>This processes the given entry and determines if it should continue.</summary>
        /// <param name="entry">The entry to process.</param>
        /// <returns>True to continue processing this entry, false to drop the entry.</returns>
        virtual protected bool ProcessEntry(Entry entry) => true;

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The log to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Log(string format, params object[] args) {
        }
    }
}
