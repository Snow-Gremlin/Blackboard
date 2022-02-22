using System.IO;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a buffer logger used as a temporary text based output, like a virtual console,
    /// for recording the text of the messages followed by a newline which have been logged.
    /// </summary>
    public class BufferLogger: Logger {

        /// <summary>The string buffer to write to.</summary>
        private StringWriter fout;

        /// <summary>Creates a new buffer logger.</summary>
        /// <param name="next">The next logger the entry is passed to.</param>
        public BufferLogger(Logger next = null) : base(next) => this.Clear();

        /// <summary>Clears this logger.</summary>
        public void Clear() => this.fout = new StringWriter();

        /// <summary>This writes the given entry to the buffer.</summary>
        /// <param name="entry">The entry to write.</param>
        /// <returns>Always returns true to continue.</returns>
        protected override bool ProcessEntry(Entry entry) {
            this.fout.WriteLine(entry.ToString());
            return true; 
        }

        /// <summary>This will get the logs.</summary>
        /// <returns>The logs which have been written.</returns>
        public override string ToString() {
            this.fout.Flush();
            return this.fout.ToString();
        }
    }
}
