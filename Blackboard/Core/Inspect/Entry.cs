using Blackboard.Core.Extensions;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Inspect;

/// <summary>An entry contains a message in the process of being logged.</summary>
sealed public class Entry {

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

    private S.Func<Message?> fetcher;
    private readonly List<string> groups;

    /// <summary>Creates a new entry for a deferred message.</summary>
    /// <param name="level">The level of this deferred message.</param>
    /// <param name="fetcher">
    /// This is the function used for constructing the message if
    /// and when this entry is being logged and not filtered out.
    /// </param>
    internal Entry(Level level, S.Func<Message?> fetcher) {
        if (fetcher is null)
            throw new S.ArgumentNullException(nameof(fetcher),
                "A message entry must have a non-null function for creating or getting a message.");
        this.fetcher = fetcher;
        this.Level = level;
        this.groups = new List<string>();
    }

    /// <summary>Creates a copy of this entry and message.</summary>
    /// <remarks>This will actually create the message not just copy the fetcher.</remarks>
    /// <returns>The cloned entry.</returns>
    public Entry Clone() {
        Message? msg = this.Message?.Clone();
        Entry copy = new(this.Level, () => msg);
        copy.groups.AddRange(this.groups);
        return copy;
    }

    /// <summary>Gets the message in this entry.</summary>
    /// <remarks>
    /// Messages are deferred to allow more lower level messages like info to be logged
    /// even if the message creation is slow. If the lower level messages are being
    /// filtered out, then the effort of the message creation is avoided.
    /// </remarks>
    public Message? Message {
        get {
            Message? msg = this.fetcher();
            this.fetcher = () => msg;
            return msg;
        }
    }

    /// <summary>The level of this message.</summary>
    public readonly Level Level;

    /// <summary>The group labels for where this entry exists.</summary>
    public IReadOnlyList<string> Groups => this.groups;

    /// <summary>Indicates that this entry is part of a larger group.</summary>
    /// <remarks>To prevent groups being added in loops, this will cut off at 100 groups.</remarks>
    /// <param name="label">The label to add to the group's path.</param>
    internal void AddToGroup(string label) {
        if (this.groups.Count < 100) this.groups.Insert(0, label);
    }

    /// <summary>This determines if the given labels match the groups.</summary>
    /// <param name="full">True indicates the length must match, false to only match the front.</param>
    /// <param name="labels">The labels to match against the groups.</param>
    /// <returns>True if they match, false otherwise.</returns>
    internal bool MatchGroups(bool full, params string[] labels) {
        int count = labels.Length;
        if ((count > this.groups.Count) ||
            (full && count < this.groups.Count)) return false;
        for (int i = 0; i < count; ++i) {
            if (labels[i] != this.groups[i]) return false;
        }
        return true;
    }

    /// <summary>Adds a message processor for changing the message while it is being gotten or created.</summary>
    /// <remarks>The processor should not be null but will have no effect if it is.</remarks>
    /// <param name="processor">The processor function to modify a message with.</param>
    internal void AddProcessing(S.Func<Message, Message> processor) {
        if (processor is null) return;
        S.Func<Message?> oldFetcher = this.fetcher;
        this.fetcher = () => {
            Message? msg = oldFetcher();
            return msg is null ? null : processor(msg);
        };
    }

    /// <summary>Gets the message text.</summary>
    /// <returns>The string for this entry.</returns>
    public override string ToString() => this.ToString(true, false, false);

    /// <summary>Gets the message text.</summary>
    /// <param name="indent">Indicates the message should be indented by group depth.</param>
    /// <param name="showLevel">Indicates the message should shows the level string for the message.</param>
    /// <param name="showGroup">Indicates the message should show the group of the message.</param>
    /// <returns>The string for this entry.</returns>
    public string ToString(bool indent = true, bool showLevel = false, bool showGroup = false) {
        string str = this.Message?.ToString() ?? "null";
        if (showLevel) str = LevelToString(this.Level) + " " + str;
        if (showGroup) str = this.groups.Join("/") + " " + str;
        if (indent) str = str.Indent(new string(' ', this.groups.Count*2));
        return str;
    }
}
