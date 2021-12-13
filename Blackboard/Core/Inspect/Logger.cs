using Blackboard.Core.Nodes.Interfaces;
using System.IO;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a logger used for debugging the evaluation
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class Logger: ILogger {

        /// <summary>The string buffer to write to.</summary>
        private StringWriter fout;

        /// <summary>The stringifier for converting nodes to strings.</summary>
        private Stringifier stringifier;

        /// <summary>Creates a new evaluation logger.</summary>
        public Logger() {
            this.fout = new StringWriter();
            this.stringifier = null;
        }

        /// <summary>The stringifier for converting nodes to strings.</summary>
        public Stringifier Stringifier {
            get => this.stringifier ??= Stringifier.Shallow();
            set => this.stringifier = value;
        }

        /// <summary>Clears this logger.</summary>
        public void Clear() => this.fout = new StringWriter();

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The test to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public virtual void Log(string format, params object[] args) {
            for (int i = 0; i < args.Length; i++) {
                if (args[i] is INode node) args[i] = this.Stringifier.Stringify(node);
            }
            this.fout.WriteLine(format, args);
        }

        /// <summary>This will get the logs.</summary>
        /// <returns>The logs which have been written.</returns>
        public override string ToString() {
            this.fout.Flush();
            return this.fout.ToString();
        }
    }
}
