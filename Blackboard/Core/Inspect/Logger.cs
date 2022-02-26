using Blackboard.Core.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly Logger next;
        private S.Func<Entry, bool> processor;

        /// <summary>Creates a new base logger.</summary>
        /// <param name="next">The next logger the entry is passed to.</param>
        public Logger(Logger next = null) : this(next, null) { }

        /// <summary>Creates a new base logger.</summary>
        /// <param name="next">The next logger the entry is passed to.</param>
        /// <param name="processor">The processor that this logger should run on an entry.</param>
        protected Logger(Logger next = null, S.Func<Entry, bool> processor = null) {
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
        private void log(Entry entry) {
            if (this.ProcessEntry(entry)) this.next?.log(entry);
        }

        #region Logger Creators...

        /// <summary>This stops any entry with a lower level than the given level from continuing.</summary>
        /// <param name="level">The minimum level of the message to permit.</param>
        /// <returns>The new logger with the set level.</returns>
        public Logger SetLevel(Level level) => new(this, (Entry entry) => level <= entry.Level);

        /// <summary>This adds the given data to any message logged with this logger.</summary>
        /// <param name="key">The key of the data to add.</param>
        /// <param name="value">The value of the data to add.</param>
        /// <returns>The logger which will add this data to messages.</returns>
        public Logger With(string key, object value) => new(this, (Entry entry) => {
            entry.AddProcessing((Message msg) => {
                msg.With(key, value);
                return msg;
            });
            return true;
        });

        /// <summary>This adds a label to the message which can be used for filtering.</summary>
        /// <param name="label">The label to add a message.</param>
        /// <returns>The logger which will add these labels.</returns>
        public Logger Label(string label) => new(this, (Entry entry) => {
            entry.AddLabel(label);
            return true;
        });

        /// <summary>This filters any entry which contains the given label or label part.</summary>
        /// <param name="label">The label or label part which must be found.</param>
        /// <returns>The logger which will remove any message which matches the given label or label part.</returns>
        public Logger Filter(string label) => new(this, (Entry entry) =>
            !entry.Labels.Contains(label));

        /// <summary>This filters any entry which matches the given regular expression.</summary>
        /// <param name="pattern">The regular expression to match.</param>
        /// <returns>The logger which will remove any message which matches the given label or label part.</returns>
        public Logger PatternFilter(string pattern) {
            Regex reg = null;
            return new(this, (Entry entry) => {
                reg ??= new(pattern, RegexOptions.Compiled);
                return !reg.IsMatch(entry.Labels);
            });
        }

        /// <summary>This processes a message by applying the given stringifier to the arguments and data.</summary>
        /// <param name="stringifier">The stringifier to apply or null to use shallow.</param>
        /// <returns>The logger which will stringify values in the message.</returns>
        public Logger Stringify(Stringifier stringifier = null) => new(this, (Entry entry) => {
            entry.AddProcessing((Message msg) => {
                msg.Stringify(stringifier);
                return msg;
            });
            return true;
        });

        /// <summary>This creates a logger to outputs the messages to a console.</summary>
        /// <returns>The logger which will write messages to the console</returns>
        public ConsoleLogger Console() => new(this);

        /// <summary>This creates a logger to outputs the messages to a string buffer.</summary>
        /// <returns>The logger which will write messages to a string buffer.</returns>
        public BufferLogger Buffered() => new(this);

        /// <summary>This creates a logger which collects the message entries in a list.</summary>
        /// <returns>The logger which will collect entries in a list.</returns>
        public CollectorLogger Collector() => new(this);

        #endregion
        #region Message Methods...

        /// <summary>This will write to this log.</summary>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        public void Log(Level level, S.Func<Message> fetcher) => this.log(new Entry(level, fetcher));

        /// <summary>This will write to this log.</summary>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="msg">The message to log.</param>
        public void Log(Level level, Message msg) => this.Log(level, () => msg.Clone());

        /// <summary>This will write to this log.</summary>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Log(Level level, string format, params object[] args) => this.Log(level, () => new Message(format, args));

        /// <summary>This will write an information log.</summary>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        public void Info(S.Func<Message> fetcher) => this.Log(Level.Info, fetcher);

        /// <summary>This will write an information log.</summary>
        /// <param name="msg">The message to log.</param>
        public void Info(Message msg) => this.Log(Level.Info, msg);

        /// <summary>This will write an information log.</summary>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Info(string format, params object[] args) => this.Log(Level.Info, format, args);

        /// <summary>This will write a notice log.</summary>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        public void Notice(S.Func<Message> fetcher) => this.Log(Level.Notice, fetcher);

        /// <summary>This will write a notice log.</summary>
        /// <param name="msg">The message to log.</param>
        public void Notice(Message msg) => this.Log(Level.Notice, msg);

        /// <summary>This will write a notice log.</summary>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Notice(string format, params object[] args) => this.Log(Level.Notice, format, args);

        /// <summary>This will write a warning log.</summary>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        public void Warning(S.Func<Message> fetcher) => this.Log(Level.Warning, fetcher);

        /// <summary>This will write a warning log.</summary>
        /// <param name="msg">The message to log.</param>
        public void Warning(Message msg) => this.Log(Level.Warning, msg);

        /// <summary>This will write a warning log.</summary>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Warning(string format, params object[] args) => this.Log(Level.Warning, format, args);

        /// <summary>This will write an error log.</summary>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        public void Error(S.Func<Message> fetcher) => this.Log(Level.Error, fetcher);

        /// <summary>This will write an error log.</summary>
        /// <param name="msg">The message to log.</param>
        public void Error(Message msg) => this.Log(Level.Error, msg);

        /// <summary>This will write an error log.</summary>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        public void Error(string format, params object[] args) => this.Log(Level.Error, format, args);

        #endregion

        /// <summary>This will get the name of the logger.</summary>
        /// <returns>The name of the logger.</returns>
        public override string ToString() => nameof(Logger);
    }
}
