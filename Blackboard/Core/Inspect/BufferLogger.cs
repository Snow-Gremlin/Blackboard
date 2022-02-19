using System.IO;
using System.Linq;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a buffer logger used for debugging the evaluation,
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class BufferLogger: Logger {

        /// <summary>The string buffer to write to.</summary>
        private StringWriter fout;

        /// <summary>Creates a new buffer logger.</summary>
        public BufferLogger() => this.fout = new StringWriter();

        /// <summary>Clears this logger.</summary>
        public void Clear() => this.fout = new StringWriter();

        protected override bool ProcessEntry(Entry entry) {
            this.fout.Write(entry.ToString());
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
