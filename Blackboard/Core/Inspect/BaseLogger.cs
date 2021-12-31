using System.Linq;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// An abstract logger to simplify making other loggers for debugging the evaluation,
    /// and update of nodes and actions in the slate.
    /// </summary>
    abstract public class BaseLogger: ILogger {

        /// <summary>The stringifier for converting nodes to strings.</summary>
        private Stringifier stringifier;

        /// <summary>Creates a new base logger.</summary>
        protected BaseLogger() =>
            this.stringifier = null;

        /// <summary>The stringifier for converting nodes to strings.</summary>
        public Stringifier Stringifier {
            get => this.stringifier ??= Stringifier.Shallow();
            set => this.stringifier = value;
        }

        /// <summary>
        /// This will write to this log followed by a new line.
        /// The input has already been formatted, adjusted, and had a newline added.
        /// </summary>
        /// <param name="msg">The formatted and adjusted message to write.</param>
        protected abstract void AdjustedLog(string msg);

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The log to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public virtual void Log(string format, params object[] args) =>
            this.AdjustedLog(string.Format(format + "\n", this.Stringifier.Stringify(args).ToArray()));

        /// <summary>Creates a new logger indented one level.</summary>
        public ILogger Sub => new SubLogger(this);
    }
}
