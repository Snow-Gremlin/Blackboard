namespace Blackboard.Core.Inspect {

    /// <summary>A logger for logging with an indent.</summary>
    sealed public class SubLogger : ILogger {

        /// <summary>The indent to apply to the level.</summary>
        private const string defaultIndent = "   ";

        /// <summary>The base logger to write out to.</summary>
        private readonly ILogger logger;

        /// <summary>The indent to pre-pend to lines based on the current push.</summary>
        private readonly string indent;

        /// <summary>Creates a new logger which is indented.</summary>
        /// <param name="logger">The base logger to output to.</param>
        /// <param name="indent">The indent to apply. If empty the default will be used.</param>
        public SubLogger(ILogger logger, string indent = null) {
            if (string.IsNullOrEmpty(indent)) indent = defaultIndent;
            if (logger is SubLogger sub) {
                this.logger = sub.logger;
                this.indent = sub.indent + indent;
            } else {
                this.logger = logger;
                this.indent = indent;
            }
        }

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The test to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Log(string format, params object[] args) =>
            this.logger.Log(this.indent + format, args);

        /// <summary>Creates a new logger indented one level.</summary>
        public ILogger Sub => new SubLogger(this);
    }
}
