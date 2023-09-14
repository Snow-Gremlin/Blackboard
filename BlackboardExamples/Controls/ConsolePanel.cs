using Blackboard.Core.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace BlackboardExamples;

/// <summary>A customized text box that is populated with text from the console.</summary>
public partial class ConsolePanel : TextBox {

    /// <summary>A hook that injects into the console out stream to return lines of text.</summary>
    private class ConsoleHook : TextWriter {
        private readonly TextWriter oldConsole;
        private readonly StringBuilder buffer;
        private readonly Action<string> writeText;
        private readonly System.Windows.Forms.Timer timer;

        /// <summary>Creates a new console hook.</summary>
        /// <param name="writeText">The action to write console text to.</param>
        public ConsoleHook(Action<string> writeText) {
            this.oldConsole = Console.Out;
            this.buffer     = new();
            this.writeText  = writeText;
            this.timer      = new() { Interval = 250 };
            this.timer.Tick += this.timerTick;
            Console.SetOut(this);
        }

        /// <summary>Handles a delayed timer for flushing the buffer.</summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        private void timerTick(object? sender, EventArgs e) {
            this.timer.Stop();
            this.Flush();
        }

        /// <summary>Unhooks disconnects the hook from the console.</summary>
        public void Unhook() => Console.SetOut(this.oldConsole);

        /// <summary>Gets the text encoding used by the console.</summary>
        public override Encoding Encoding => this.oldConsole.Encoding;

        /// <summary>Flushes the console and any pending buffered text.</summary>
        public override void Flush() {
            this.oldConsole.Flush();
            this.writeText(this.buffer.ToString());
            this.buffer.Clear();
        }

        /// <summary>Writes a single character out to the console and buffer.</summary>
        /// <remarks>If the character is a new line, a timer is started for a flush.</remarks>
        /// <param name="value">The character to write.</param>
        public override void Write(char value) {
            this.oldConsole.Write(value);
            this.buffer.Append(value);
            if (value == '\n' && !this.timer.Enabled)
                this.timer.Start();
        }
    }
    
    /// <summary>A regular expression for splitting a new line with optional carriage returns.</summary>
    /// <returns>The line splitter regular expression.</returns>
    [GeneratedRegex("\\r*\\n\\r*", RegexOptions.Compiled)]
    private static partial Regex lineSplitter();

    /// <summary>The maximum number of lines to keep in the console before throwing away old lines.</summary>
    private const int lineLimit = 1_000_000;

    /// <summary>The hook to inject </summary>
    private readonly ConsoleHook consoleHook;

    /// <summary>Creates a new console panel and hooks into the console stream.</summary>
    public ConsolePanel() {
        this.Dock        = DockStyle.Fill;
        this.ScrollBars  = ScrollBars.Both;
        this.BackColor   = Color.FromArgb(10, 10, 10);
        this.ForeColor   = Color.White;
        this.Multiline   = true;
        this.ReadOnly    = true;
        this.WordWrap    = false;
        this.Font        = new Font(FontFamily.GenericMonospace, 9.0f);
        this.consoleHook = new(this.appendText);
        this.scrollToBottom();
    }

    /// <summary>Disposes this panel and unhooks from the console stream.</summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources;
    /// false to only release unmanaged.
    /// </param>
    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        this.consoleHook.Unhook();
        this.consoleHook.Dispose();
    }

    /// <summary>Appends text from the console into this panel.</summary>
    /// <param name="text">The text to append.</param>
    private void appendText(string text) {
        bool autoScroll = this.shouldAutoScroll();
        List<string> lines = new(this.Lines);
        lineSplitter().Split(text.TrimEnd()).
            Select(l => l.TrimEnd()).Foreach(lines.Add);
        if (lines.Count > lineLimit)
            lines.RemoveRange(0, lines.Count - lineLimit);
        this.Lines = lines.ToArray();
        if (autoScroll) this.scrollToBottom();
    }

    /// <summary>Determines if the scroll bar is at the bottom of the panel.</summary>
    /// <returns>True if the panel should auto-scroll on new text.</returns>
    private bool shouldAutoScroll() =>
        this.SelectionStart + this.SelectionLength >= this.Text.Length - 10;

    /// <summary>Scrolls to the bottom of the panel.</summary>
    private void scrollToBottom() {
        this.Select(this.Text.Length, 0);
        this.ScrollToCaret();
    }
}
