using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// An abstract logger to simplify making other loggers for debugging the evaluation,
    /// and update of nodes and actions in the slate.
    /// </summary>
    /// <remarks>
    /// This will not output anything by default, it must be passed through
    /// a logger which writes the message to some output.
    /// </remarks>
    public class Logger {
        private readonly Logger? next;
        private readonly S.Func<Entry, bool>? processor;

        /// <summary>Creates a new base logger.</summary>
        /// <param name="next">The next logger the entry is passed to.</param>
        public Logger(Logger? next = null) : this(next, null) { }

        /// <summary>Creates a new base logger.</summary>
        /// <param name="next">The next logger the entry is passed to.</param>
        /// <param name="processor">The processor that this logger should run on an entry.</param>
        internal Logger(Logger? next = null, S.Func<Entry, bool>? processor = null) {
            this.next = next;
            this.processor = processor;
        }

        /// <summary>This processes the given entry and determines if it should continue.</summary>
        /// <param name="entry">The entry to process.</param>
        /// <returns>True to continue processing this entry, false to drop the entry.</returns>
        virtual protected bool ProcessEntry(Entry entry) =>
            this.processor is null || this.processor(entry);

        /// <summary>This will write to this log.</summary>
        /// <param name="entry">The entry to process and log.</param>
        internal void Log(Entry entry) {
            if (this.ProcessEntry(entry)) this.next?.Log(entry);
        }

        /// <summary>This will get the name of the logger.</summary>
        /// <returns>The name of the logger.</returns>
        public override string ToString() => nameof(Logger);
    }
}
