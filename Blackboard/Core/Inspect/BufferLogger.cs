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
        public BufferLogger(Logger next) : this(true, false, false, next) { }

        /// <summary>Creates a new buffer logger.</summary>
        /// <param name="indent">Indicates the message should be indented by group depth.</param>
        /// <param name="showLevel">Indicates the message should shows the level string for the message.</param>
        /// <param name="showGroup">Indicates the message should show the group of the message.</param>
        /// <param name="next">The next logger the entry is passed to.</param>
        public BufferLogger(bool indent = true, bool showLevel = false, bool showGroup = false, Logger? next = null) : base(next) {
            this.Indent    = indent;
            this.ShowLevel = showLevel;
            this.ShowGroup = showGroup;
            this.fout      = new StringWriter();
        }

        /// <summary>Indicates the message should be indented by group depth.<</summary>
        public bool Indent;

        /// <summary>Indicates the message should shows the level string for the message.</summary>
        public bool ShowLevel;

        /// <summary>Indicates the message should show the group of the message.</summary>
        public bool ShowGroup;

        /// <summary>Clears this logger.</summary>
        public void Clear() => this.fout = new StringWriter();

        /// <summary>This writes the given entry to the buffer.</summary>
        /// <param name="entry">The entry to write.</param>
        /// <returns>Always returns true to continue.</returns>
        protected override bool ProcessEntry(Entry entry) {
            // Not using WriteLine so that output doesn't have "\r" in it.
            this.fout.Write(entry.ToString(this.Indent, this.ShowLevel, this.ShowGroup)+"\n");
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
