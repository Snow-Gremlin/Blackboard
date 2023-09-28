using System.Collections.Generic;

namespace Blackboard.Core.Inspect;

/// <summary>
/// This is a message counter logger used for keeping
/// a count of messages by different levels.
/// </summary>
sealed public class LogCounter : Logger {
    private readonly Dictionary<Level, int> counts;

    /// <summary>Creates a new message counter logger.</summary>
    /// <param name="next">The next logger the entry is passed to.</param>
    public LogCounter(Logger? next = null) : base(next) =>
        this.counts = new();

    /// <summary>Resets the number of counts.</summary>
    public void Reset() => this.counts.Clear();

    /// <summary>Gets the count for the given level.</summary>
    /// <param name="level">The level to get the count from.</param>
    /// <returns>The number of messages with the given level.</returns>
    public int Count(Level level) => this.counts.GetValueOrDefault(level);

    /// <summary>This counters the given entry.</summary>
    /// <param name="entry">The entry to count.</param>
    /// <returns>Always returns true to continue.</returns>
    protected override bool ProcessEntry(Entry entry) {
        this.counts[entry.Level] = this.Count(entry.Level) + 1;
        return true;
    }

    /// <summary>This will get the name of the logger.</summary>
    /// <returns>The name of the logger.</returns>
    public override string ToString() => nameof(LogCounter);
}
