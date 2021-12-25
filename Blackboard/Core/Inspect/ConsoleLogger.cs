using System.Linq;
using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>
    /// This is a console logger used for debugging the evaluation
    /// and update of nodes and actions in the slate.
    /// </summary>
    public class ConsoleLogger: ILogger {

        /// <summary>The stringifier for converting nodes to strings.</summary>
        private Stringifier stringifier;

        /// <summary>Creates a new evaluation logger.</summary>
        public ConsoleLogger() =>
            this.stringifier = null;

        /// <summary>The stringifier for converting nodes to strings.</summary>
        public Stringifier Stringifier {
            get => this.stringifier ??= Stringifier.Shallow();
            set => this.stringifier = value;
        }

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="format">The test to write.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public virtual void Log(string format, params object[] args) =>
            S.Console.WriteLine(format, this.Stringifier.Stringify(args).ToArray());

        /// <summary>Creates a new logger indented one level.</summary>
        public ILogger Sub => new SubLogger(this);

        /// <summary>This will get the name of the logger.</summary>
        /// <returns>The name of the logger.</returns>
        public override string ToString() => "ConsoleLogger";
    }
}
