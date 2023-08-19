using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;
using S = System;

namespace BlackboardTests.Tools;

/// <summary>These are extensions to general objects for testing.</summary>
static class GeneralExt {

    /// <summary>This measures the amount of time it takes to run the action several times.</summary>
    /// <param name="action">The action to run several times.</param>
    /// <param name="title">
    /// The title to show when printing measurement information to the console.
    /// If null or empty then no information is printed to the console.
    /// </param>
    /// <param name="divisor">
    /// The scalar to convert a call to an op.
    /// For example if the call makes 10 calls into some operation then the divisor is 10.
    /// </param>
    /// <param name="minSecs">The minimum amount of time to keep repeating the action.</param>
    /// <returns>The average number of milliseconds per action operation.</returns>
    static public double Measure(this S.Action action, string title = null, double divisor = 1.0, double minSecs = 0.5) {
        S.TimeSpan minimum = S.TimeSpan.FromSeconds(minSecs);
        int count = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();
        do {
            count++;
            action();
        } while (stopwatch.Elapsed < minimum);
        stopwatch.Stop();
        S.TimeSpan elapsed = stopwatch.Elapsed;
        S.TimeSpan perCall = elapsed.Divide(count);
        S.TimeSpan perOp   = perCall.Divide(divisor);

        if (!string.IsNullOrEmpty(title)) {
            S.Console.WriteLine(title+":");
            S.Console.WriteLine("   Total Time: "+elapsed.TotalSeconds+"s");
            S.Console.WriteLine("   Call Count: "+count);
            S.Console.WriteLine("   Per Call:   "+perCall.TotalMilliseconds+"ms");
            S.Console.WriteLine("   Per Op:     "+perOp.TotalMilliseconds+"ms");
        }
        return perOp.TotalMilliseconds;
    }

    /// <summary>Checks if the given buffer has the given expected value and clears the buffer.</summary>
    /// <param name="buffer">The buffer to check and clear.</param>
    /// <param name="expStr">The expected text in the buffer.</param>
    static public void Check(this StringBuilder buffer, string expStr) {
        Assert.AreEqual(expStr, buffer.ToString());
        buffer.Clear();
    }
}
