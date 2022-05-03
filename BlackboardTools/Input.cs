using System.Collections.Generic;
using System.Text;
using S = System;
using IO = System.IO;
using System.Linq;

namespace BlackboardTools {

    /// <summary>The console input handler for reading user commands.</summary>
    internal class Input {
        private const string historyFile = "./history.txt";
        private const int historyLimit = 100;

        private int index;
        private int priorWidth;
        private readonly StringBuilder command;
        private readonly List<string> history;
        private int historyIndex;

        /// <summary>Creates a new input handler.</summary>
        public Input() {
            this.index = 0;
            this.priorWidth = 0;
            this.command = new StringBuilder();
            this.history = new List<string>();
            this.historyIndex = -1;
            this.loadHistory();
        }

        /// <summary>Reads the user command from the console input.</summary>
        /// <remarks>Blocks until the user has finished typing in a command.</remarks>
        /// <returns>The command that the user has inputted.</returns>
        public string Read() {
            this.reset();
            while (true) {
                S.ConsoleKeyInfo info = S.Console.ReadKey(true);
                if (info.Key == S.ConsoleKey.Enter) break;
                this.handleKey(info);
            }
            return this.finish();
        }

        /// <summary>Resets the current command to prepare for a new input.</summary>
        private void reset() {
            this.index = 0;
            this.priorWidth = 0;
            this.command.Clear();
            this.historyIndex = -1;
            this.update();
        }

        /// <summary>Adds the given text to the history if the text isn't the currently selected or prior command.</summary>
        /// <param name="text">The text to add to the history.</param>
        private void addToHistory(string text) {
            if (text.Length <= 0) return;
            else if (this.historyIndex >= 0) {
                // If this is the old command, move it to the front.
                if (text == this.history[this.historyIndex])
                    this.history.RemoveAt(this.historyIndex);
            } else if (this.history.Count > 0) {
                if (text == this.history[0]) return;
            }
            this.history.Insert(0, text);
            this.saveHistory();
        }

        /// <summary>Finishes a user input, updates the history, and returns the command.</summary>
        /// <returns>The command which the user has inputted.</returns>
        private string finish() {
            S.Console.WriteLine();
            string text = this.command.ToString().Trim();
            this.addToHistory(text);
            return text;
        }

        /// <summary>Updates the cursor location in the console.</summary>
        private void updateCursor() =>
            S.Console.SetCursorPosition(this.index+1, S.Console.CursorTop);

        /// <summary>Updates the current command to the console.</summary>
        private void update() {
            string text = this.command.ToString();
            string line = (">"+text).PadRight(this.priorWidth);
            this.priorWidth = text.Length+1;

            // TODO: Need to handle when the input is longer than the console width.
            S.Console.ForegroundColor = S.ConsoleColor.Cyan;
            S.Console.SetCursorPosition(0, S.Console.CursorTop);
            S.Console.Write(line);
            this.updateCursor();
            S.Console.ResetColor();
        }

        /// <summary>Handles the key input from the user.</summary>
        /// <param name="info">The input information from the user's input.</param>
        private void handleKey(S.ConsoleKeyInfo info) {
            switch (info.Key) {
                case S.ConsoleKey.Backspace:  this.backspace();           break;
                case S.ConsoleKey.Delete:     this.delete();              break;
                case S.ConsoleKey.LeftArrow:  this.move(-1);              break;
                case S.ConsoleKey.RightArrow: this.move(1);               break;
                case S.ConsoleKey.Home:       this.move(int.MinValue);    break;
                case S.ConsoleKey.End:        this.move(int.MaxValue);    break;
                case S.ConsoleKey.UpArrow:    this.seach(1);              break;
                case S.ConsoleKey.DownArrow:  this.seach(-1);             break;
                default:                      this.addChar(info.KeyChar); break;
            }
        }

        /// <summary>Handles the user pressing backspace.</summary>
        private void backspace() {
            if (this.command.Length <= 0 || this.index <= 0) return;
            this.command.Remove(this.index-1, 1);
            this.index--;
            this.update();
        }

        /// <summary>Handles the user pressing delete.</summary>
        private void delete() {
            if (this.index >= this.command.Length) return;
            this.command.Remove(this.index, 1);
            this.update();
        }

        /// <summary>Handles the user moving left or right with arrow keys or home/end keys.</summary>
        /// <param name="delta">The change in the location of the cursor.</param>
        private void move(int delta) {
            int newIndex = S.Math.Clamp(this.index+delta, 0, this.command.Length);
            if (this.index == newIndex) return;
            this.index = newIndex;
            this.updateCursor();
        }

        /// <summary>Handles the user pressing up or down to move through the history.</summary>
        /// <param name="delta">The change in the history's index.</param>
        private void seach(int delta) {
            this.historyIndex = S.Math.Clamp(this.historyIndex+delta, -1, this.history.Count-1);
            string text = this.historyIndex >= 0 ? this.history[this.historyIndex] : "";
            this.command.Clear();
            this.command.Append(text);
            this.index = text.Length;
            this.update();
        }

        /// <summary>Handles the user pressing a letter, number, symbol, punctuation, or anything else.</summary>
        /// <param name="value">The character value the user inputted.</param>
        private void addChar(char value) {
            if (char.IsControl(value)) return;
            this.command.Insert(this.index, value);
            this.index++;
            this.update();
        }

        /// <summary>Saves the history of commands to a file.</summary>
        private void saveHistory() =>
            IO.File.WriteAllLines(historyFile, this.history.Take(historyLimit));

        /// <summary>Loads the history of command from a file.</summary>
        private void loadHistory() {
            if (IO.File.Exists(historyFile))
                this.history.AddRange(IO.File.ReadAllLines(historyFile));
        }
    }
}
