using Blackboard.Core.Extensions;
using System.Linq;

namespace Blackboard.Core.Inspect {

    /// <summary>This is a logger which applied a stringifier to an entry.</summary>
    internal class StringifierLogger: Logger {

        /// <summary>The stringifier for converting nodes to strings.</summary>
        private Stringifier stringifier;

        /// <summary>Creates a new base logger.</summary>
        /// <param name="inner">The next logger the entry is passed to.</param>
        public StringifierLogger(Stringifier stringifier, Logger inner) : base(inner) =>
            this.stringifier = stringifier;

        /// <summary>Processes the given message by applying the stringifier to the arguments and data.</summary>
        /// <param name="msg">The message to stringify.</param>
        /// <returns>The given message after its been stringified.</returns>
        private Message process(Message msg) {
            this.stringifier ??= Stringifier.Shallow();

            foreach ((object arg, int index) in this.stringifier.Stringify(msg.Arguments).WithIndex())
                msg.Arguments[index] = arg;

            foreach ((string key, object value) in msg.Data.Keys.Zip(this.stringifier.Stringify(msg.Data.Values)))
                msg.Data[key] = value;

            return msg;
        }
         
        /// <summary>Processes the entry by adding the stringifier processing to the entry.</summary>
        /// <param name="entry">The entry to add stringify processing to.</param>
        /// <returns>Will always return true.</returns>
        protected override bool ProcessEntry(Entry entry) {
            entry.AddProcessing(this.process);
            return true;
        }
    }
}
