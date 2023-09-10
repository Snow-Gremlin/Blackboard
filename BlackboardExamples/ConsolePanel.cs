﻿using Blackboard.Core.Extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace BlackboardExamples;

// TODO: Comment
sealed public partial class ConsolePanel : UserControl {
    
    [GeneratedRegex("\\r*\\n\\r*")]
    private static partial Regex lineSplitter();

    /// <summary>A hook that injects into the console out to return lines of text.</summary>
    private class ConsoleHook : TextWriter {
        private readonly TextWriter oldConsole;
        private readonly StringBuilder buffer;
        private readonly Action<string> writeText;
        private readonly System.Windows.Forms.Timer timer;

        public ConsoleHook(Action<string> writeText) {
            this.oldConsole = Console.Out;
            this.buffer     = new();
            this.writeText  = writeText;
            this.timer      = new() { Interval = 250 };
            this.timer.Tick += this.timer_Tick;
            Console.SetOut(this);
        }

        private void timer_Tick(object? sender, EventArgs e) {
            this.timer.Stop();
            this.Flush();
        }

        public void Unhook() => Console.SetOut(this.oldConsole);

        public override Encoding Encoding => this.oldConsole.Encoding;

        public override void Flush() {
            this.oldConsole.Flush();
            this.writeText(this.buffer.ToString());
            this.buffer.Clear();
        }

        public override void Write(char value) {
            this.oldConsole.Write(value);
            this.buffer.Append(value);
            if (value == '\n' && !this.timer.Enabled)
                this.timer.Start();
        }
    }

    const int lineLimit = 100000;

    private readonly TextBox textBox;
    private readonly ConsoleHook consoleHook;

    public ConsolePanel() {
        this.textBox   = new() {
            Dock       = DockStyle.Fill,
            ScrollBars = ScrollBars.Both,
            BackColor  = Color.FromArgb(10, 10, 10),
            ForeColor  = Color.White,
            Multiline  = true,
            ReadOnly   = true,
            WordWrap   = false,
            Font       = new Font(FontFamily.GenericMonospace, 9.0f)
        };
        this.Controls.Add(this.textBox);

        this.consoleHook = new(this.appendText);
        this.scrollToBottom();
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        this.consoleHook.Unhook();
        this.consoleHook.Dispose();
    }

    private void appendText(string text) {
        bool autoScroll = this.scrollAtBottom();
        List<string> lines = new(this.textBox.Lines);
        lineSplitter().Split(text.TrimEnd()).
            Select(l => l.TrimEnd()).Foreach(lines.Add);
        if (lines.Count > lineLimit)
            lines.RemoveRange(0, lines.Count - lineLimit);
        this.textBox.Lines = lines.ToArray();
        if (autoScroll) this.scrollToBottom();
    }

    private bool scrollAtBottom() =>
        this.textBox.SelectionStart >= this.textBox.TextLength;

    private void scrollToBottom() {
        this.textBox.Select(this.textBox.Text.Length, 0);
        this.textBox.ScrollToCaret();
    }
}