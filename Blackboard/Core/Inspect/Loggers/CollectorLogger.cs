using System.Collections.Generic;

namespace Blackboard.Core.Inspect.Loggers;

/// <summary>
/// This is a message collector logger used for keeping a list of messages,
/// via entries, which have been logged still in the form of a message instance.
/// </summary>
sealed public class CollectorLogger : Logger {

    /// <summary>The string buffer to write to.</summary>
    private readonly List<Entry> entries;

    /// <summary>Creates a new message collector logger.</summary>
    /// <param name="next">The next logger the entry is passed to.</param>
    public CollectorLogger(Logger? next = null) : base(next) =>
        this.entries = new();

    /// <summary>Clears this collection.</summary>
    public void Clear() => this.entries.Clear();

    /// <summary>The messages that have been collected.</summary>
    public IReadOnlyList<Entry> Entries => this.entries;

    /// <summary>This writes the given entry to the buffer.</summary>
    /// <param name="entry">The entry to write.</param>
    /// <returns>Always returns true to continue.</returns>
    protected override bool ProcessEntry(Entry entry) {
        this.entries.Add(entry.Clone());
        return true;
    }

    /// <summary>This will get the name of the logger.</summary>
    /// <returns>The name of the logger.</returns>
    public override string ToString() => nameof(CollectorLogger);
}
