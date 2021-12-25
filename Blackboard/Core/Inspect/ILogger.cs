namespace Blackboard.Core.Inspect {

    /// <summary>This is the interface for the loggers used for debugging.</summary>
    public interface ILogger {

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The test to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Log(string format, params object[] args);

        /// <summary>This gets a logger which is indents a level deeper than the current logger.</summary>
        public ILogger Sub { get; }
    }
}
