using IO = System.IO;
using S = System;

namespace Blackboard.Core.Inspect.Loggers;

/// <summary>A logger that outputs to a console.</summary>
sealed public class ConsoleLogger : Logger {

    /// <summary>Creates a new logger which outputs to a console.</summary>
    /// <param name="next">The next logger the entry is passed to.</param>
    public ConsoleLogger(Logger? next = null) : base(next) { }

    /// <summary>Creates a new logger which outputs to a console.</summary>
    /// <param name="output">The output to send the console logs, or null to use the default console.</param>
    /// <param name="next">The next logger the entry is passed to.</param>
    public ConsoleLogger(IO.TextWriter? output, Logger? next = null) : base(next) =>
        this.Out = output;

    /// <summary>The output to send the logs too.</summary>
    public IO.TextWriter? Out;

    /// <summary>This writes the given entry to the buffer.</summary>
    /// <param name="entry">The entry to write.</param>
    /// <returns>Always returns true to continue.</returns>
    protected override bool ProcessEntry(Entry entry) {
        this.Out ??= S.Console.Out;
        this.Out.WriteLine(entry.ToString());
        return true;
    }

    /// <summary>This will get the name of the logger.</summary>
    /// <returns>The name of the logger.</returns>
    public override string ToString() => nameof(ConsoleLogger);
}
