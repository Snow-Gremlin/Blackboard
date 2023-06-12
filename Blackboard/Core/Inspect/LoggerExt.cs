using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Inspect {

    /// <summary>Null-aware extensions to add to or post to a given logger.</summary>
    static public class LoggerExt {
        #region Logger Creators...

        /// <summary>This stops any entry with a lower level than the given level from continuing.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="level">The minimum level of the message to permit.</param>
        /// <returns>The new logger with the set level.</returns>
        static public Logger SetMinimumLevel(this Logger inner, Level level) =>
            inner is null ? null :
            new(inner, (Entry entry) => level <= entry.Level);

        /// <summary>This adds the given data to any message logged with this logger.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="key">The key of the data to add.</param>
        /// <param name="value">The value of the data to add.</param>
        /// <returns>The logger which will add this data to messages.</returns>
        static public Logger With(this Logger inner, string key, object value) =>
            inner is null ? null :
            new(inner, (Entry entry) => {
                entry.AddProcessing((Message msg) => {
                    msg.With(key, value);
                    return msg;
                });
                return true;
            });

        /// <summary>This adds a new group which is internal to the current entry.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="label">The label to add to the entry path.</param>
        /// <returns>The logger for these sub-entries.</returns>
        static public Logger SubGroup(this Logger inner, string label) =>
            inner is null ? null :
            new(inner, (Entry entry) => {
                entry.AddToGroup(label);
                return true;
            });

        /// <summary>Removes any entry which starts with the given labels.</summary>
        /// <param name="labels">The labels of the entries to remove.</param>
        /// <returns>The logger which filters the group.</returns>
        static public Logger FilterGroup(this Logger inner, params string[] labels) =>
            inner is null ? null :
            new(inner, (Entry entry) =>
            !entry.MatchGroups(false, labels));

        /// <summary>Removes all sub-entry to the group with the given labels.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="labels">The labels of the entry to remove sub-entries from.</param>
        /// <returns>The logger which filters the group.</returns>
        static public Logger FilterSubGroups(this Logger inner, params string[] labels) =>
            inner is null ? null :
            new(inner, (Entry entry) =>
            !entry.MatchGroups(true, labels));

        /// <summary>Removes all entries which do not start with the given labels.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="labels">The labels of the entries to keep.</param>
        /// <returns>The logger to select only a group</returns>
        static public Logger SelectGroup(this Logger inner, params string[] labels) =>
            inner is null ? null :
            new(inner, (Entry entry) =>
            entry.MatchGroups(false, labels));

        /// <summary>This processes a message by applying the given stringifier to the arguments and data.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <param name="stringifier">The stringifier to apply or null to use shallow.</param>
        /// <returns>The logger which will stringify values in the message.</returns>
        static public Logger? Stringify(this Logger? inner, Stringifier? stringifier = null) =>
            inner is null ? null :
            new(inner, (Entry entry) => {
                entry.AddProcessing((Message msg) => {
                    msg.Stringify(stringifier);
                    return msg;
                });
                return true;
            });

        /// <summary>This creates a logger to outputs the messages to a console.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <returns>The logger which will write messages to the console</returns>
        static public ConsoleLogger Console(this Logger inner) =>
            inner is null ? null : new(inner);

        /// <summary>This creates a logger to outputs the messages to a string buffer.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <returns>The logger which will write messages to a string buffer.</returns>
        static public BufferLogger Buffered(this Logger inner) =>
            inner is null ? null : new(inner);

        /// <summary>This creates a logger which collects the message entries in a list.</summary>
        /// <param name="inner">The logger to call into from the returned logger.</param>
        /// <returns>The logger which will collect entries in a list.</returns>
        static public CollectorLogger Collector(this Logger inner) =>
            inner is null ? null : new(inner);

        #endregion
        #region Message Methods...

        /// <summary>This will write to this log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        static public void Log(this Logger logger, Level level, S.Func<Message> fetcher) =>
            logger?.Log(new Entry(level, fetcher));

        /// <summary>This will write to this log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="msg">The message to log.</param>
        static public void Log(this Logger logger, Level level, Message msg) =>
            logger?.Log(level, () => msg.Clone());

        /// <summary>This will write to this log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="level">The level to log this message at.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        static public void Log(this Logger logger, Level level, string format, params object[] args) =>
            logger?.Log(level, () => new Message(format, args));

        /// <summary>This will write an information log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        static public void Info(this Logger logger, S.Func<Message> fetcher) =>
            logger?.Log(Level.Info, fetcher);

        /// <summary>This will write an information log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="msg">The message to log.</param>
        static public void Info(this Logger logger, Message msg) =>
            logger?.Log(Level.Info, msg);

        /// <summary>This will write an information log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        static public void Info(this Logger logger, string format, params object[] args) =>
            logger?.Log(Level.Info, format, args);

        /// <summary>This will write a notice log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        static public void Notice(this Logger logger, S.Func<Message> fetcher) =>
            logger?.Log(Level.Notice, fetcher);

        /// <summary>This will write a notice log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="msg">The message to log.</param>
        static public void Notice(this Logger logger, Message msg) =>
            logger?.Log(Level.Notice, msg);

        /// <summary>This will write a notice log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        static public void Notice(this Logger logger, string format, params object[] args) =>
            logger?.Log(Level.Notice, format, args);

        /// <summary>This will write a warning log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        static public void Warning(this Logger logger, S.Func<Message> fetcher) =>
            logger?.Log(Level.Warning, fetcher);

        /// <summary>This will write a warning log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="msg">The message to log.</param>
        static public void Warning(this Logger logger, Message msg) =>
            logger?.Log(Level.Warning, msg);

        /// <summary>This will write a warning log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        static public void Warning(this Logger logger, string format, params object[] args) =>
            logger?.Log(Level.Warning, format, args);

        /// <summary>This will write an error log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="fetcher">A factory for creating the message if and when it will be logged.</param>
        static public void Error(this Logger logger, S.Func<Message> fetcher) =>
            logger?.Log(Level.Error, fetcher);

        /// <summary>This will write an error log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="msg">The message to log.</param>
        static public void Error(this Logger logger, Message msg) =>
            logger?.Log(Level.Error, msg);

        /// <summary>This will write an error log.</summary>
        /// <param name="logger">The logger to write the log to.</param>
        /// <param name="format">The log to log.</param>
        /// <param name="args">Any arguments to pass into the log too.</param>
        static public void Error(this Logger logger, string format, params object[] args) =>
            logger?.Log(Level.Error, format, args);

        #endregion
    }
}
