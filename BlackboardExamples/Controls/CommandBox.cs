namespace BlackboardExamples.Controls;

/// <summary>A text box with history for command input.</summary>
public class CommandBox : TextBox {
    private readonly List<string> history;
    private int historyIndex;
    private string tempCommand;

    /// <summary>Creates a new command box.</summary>
    public CommandBox() {
        this.Font          = new(FontFamily.GenericMonospace, 9.0f);
        this.WordWrap      = false;
        this.AcceptsReturn = true;
        this.history       = new();
        this.historyIndex  = -1;
        this.tempCommand   = "";
    }

    /// <summary>Handles a key being pressed down on the command box.</summary>
    /// <param name="e">The argument with key information.</param>
    protected override void OnKeyDown(KeyEventArgs e) {
        switch (e.KeyData) {
            case Keys.Enter:
                this.OnReturnPressed(e);
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;

            case Keys.Up:
                this.backwardHistory();
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;

            case Keys.Down:
                this.forwardHistory();
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;

            default:
                this.historyIndex = -1;
                break;
        }
        base.OnKeyDown(e);
    }

    /// <summary>Handles return being pressed.</summary>
    /// <param name="args">The arguments for this action.</param>
    protected virtual void OnReturnPressed(EventArgs args) =>
        this.ReturnPressed?.Invoke(this, args);

    /// <summary>
    /// Indicates return has been pressed on this text box.
    /// This usually indicates that the command should be run.
    /// </summary>
    public event EventHandler? ReturnPressed;

    /// <summary>Steps backward in history of commands.</summary>
    private void backwardHistory() {
        if (this.historyIndex < -1) {
            // On the "empty command" past the "current input" so reload the stored current input.
            this.historyIndex = -1;
            this.Text = this.tempCommand;
            this.Select(this.Text.Length, 0);
            return;
        }
        
        if (this.historyIndex == -1) {
            // On the "current input" so step forward in history.
            // If there is no history, just leave on "current input".
            if (this.history.Count <= 0) return;

            // If the "current input" is the same of the most recent historical command
            // then skip over it, if there is another historical command before the first one.
            // Before switching store off the current input.
            if (this.Text == this.history[0]) {
                if (this.history.Count <= 1) return;
                this.historyIndex = 1;
                this.tempCommand = this.Text;
                this.Text = this.history[1];
                this.Select(this.Text.Length, 0);
                return;
            }

            // Step to the first historical command after storing off the current input.
            this.historyIndex = 0;
            this.tempCommand = this.Text;
            this.Text = this.history[0];
            this.Select(this.Text.Length, 0);
            return;
        }
        
        // On a historical command so step to the older command.
        if (this.historyIndex + 1 < this.history.Count) {
            this.historyIndex++;
            this.Text = this.history[this.historyIndex];
            this.Select(this.Text.Length, 0);
        }
    }

    /// <summary>Steps forward in the history of commands.</summary>
    private void forwardHistory() {
        // Ensure the loaded history index is within the history list.
        if (this.historyIndex >= this.history.Count)
            this.historyIndex = this.history.Count-1;

        // If already on the "empty command", just leave.
        if (this.historyIndex < -1) return;

        if (this.historyIndex == -1) {
            // On the "current input". If the current input is not empty,
            // then store off the current input and move to the "empty command".
            if (this.Text.Length > 0) {
                this.historyIndex = -2;
                this.tempCommand = this.Text;
                this.Text = "";
            }
            return;
        }

        if (this.historyIndex == 0) {
            // On the first historical command. If the historical command is the same
            // as the stored off current input then skip to the "empty command".
            if (this.Text == this.tempCommand) {
                this.historyIndex = -2;
                this.Text = "";
                return;
            }

            // Otherwise step to the "current input".
            this.historyIndex = -1;
            this.Text = this.tempCommand;
            this.Select(this.Text.Length, 0);
            return;
        }

        // On a historical command older than the first so step to the newer command.
        this.historyIndex--;
        this.Text = this.history[this.historyIndex];
        this.Select(this.Text.Length, 0);
    }

    /// <summary>Pushes the given command to the history.</summary>
    /// <param name="command">The command to push.</param>
    public void PushToHistory(string command) {
        if (command.Length <= 0) return;

        for (int i = 0; i < this.history.Count; ++i) {
            if (this.history[i] == command) {
                this.history.RemoveAt(i);
                break;
            }
        }

        this.history.Insert(0, command);
        this.historyIndex = -1;
    }

    /// <summary>Pushes the current input into the history.</summary>
    public void AcceptCurrent() => this.PushToHistory(this.Text);
}
