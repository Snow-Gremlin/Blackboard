using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>An entry contains a message in the process of being logged.</summary>
    public class Entry {

        /// <summary>Gets the string for the given level.</summary>
        /// <param name="level">The level to get the string for.</param>
        /// <returns>The string for the given level.</returns>
        static public string LevelToString(Level level) =>
            level switch {
                Level.Info    => "Info",
                Level.Notice  => "Notice",
                Level.Warning => "Warning",
                Level.Error   => "Error",
                _             => "Unknown",
            };

        private S.Func<Message> fetcher;
        private List<string> sources;

        /// <summary>Creates a new entry for a deferred message.</summary>
        /// <param name="level">The level of this deferred message.</param>
        /// <param name="fetcher">
        /// This is the function used for constructing the message if
        /// and when this entry is being logged and not filtered out.
        /// </param>
        internal Entry(Level level, S.Func<Message> fetcher) {
            if (fetcher is null)
                throw new S.ArgumentNullException(nameof(fetcher),
                    "A message entry must have a non-null function for creating or getting a message.");
            this.fetcher = fetcher;
            this.Level = level;
            this.sources = new List<string>();
        }

        /// <summary>Gets the message in this entry.</summary>
        /// <remarks>
        /// Messages are deferred to allow more lower level messages like info to be logged
        /// even if the message creation is slow. If the lower level messages are being
        /// filtered out, then the effort of the message creation is avoided.
        /// </remarks>
        public Message Message {
            get {
                Message msg = this.fetcher();
                this.fetcher = () => msg;
                return msg;
            }
        }

        /// <summary>The level of this message.</summary>
        public readonly Level Level;

        /// <summary>The set of sources this message has passed through.</summary>
        public IReadOnlyList<string> Sources { get; }

        /// <summary>Adds a source to the list of sources.</summary>
        /// <param name="source">The source to add to the list.</param>
        internal void AddSource(string source) =>
            this.sources.Add(source);

        /// <summary>Adds a message processor for changing the message while it is being gotten or created.</summary>
        /// <remarks>The processor should not be null but will have no effect if it is.</remarks>
        /// <param name="processor">The processor function to modify a message with.</param>
        internal void AddProcessing(S.Func<Message, Message> processor) {
            if (processor is null) return;
            S.Func<Message> oldFetcher = this.fetcher;
            this.fetcher = () => {
                Message msg = oldFetcher();
                return msg is null ? null : processor(msg);
            };
        }

        /// <summary>Gets the message with the level prepended to it.</summary>
        /// <returns>The string for this entry.</returns>
        public override string ToString() =>
            LevelToString(this.Level) + ": " + this.Message.ToString();
    }
}
