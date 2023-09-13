using System.Text;

namespace BlackboardExamples.Controls;

/// <summary>A text box which handles tabs for code formatting.</summary>
public partial class CodePanel : TextBox {
    private const int tabCount = 3;
    private static readonly string tabSpaces = new(' ', tabCount);

    /// <summary>Creates a new code panel.</summary>
    public CodePanel() {
        this.Font       = new(FontFamily.GenericMonospace, 9.0f);
        this.Multiline  = true;
        this.WordWrap   = false;
        this.AcceptsTab = true;
        this.ScrollBars = ScrollBars.Both;
    }

    /// <summary>Handles a key being pressed down on the code panel.</summary>
    /// <param name="e">The argument with key information.</param>
    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.KeyCode == Keys.Tab) {
            if (e.Modifiers == Keys.Shift)
                this.handleBackwardTab();
            else this.handleForwardTab();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        base.OnKeyDown(e);
    }

    /// <summary>Finds the new line character prior to the given start index.</summary>
    /// <param name="buf">The buffer to check in.</param>
    /// <param name="start">The start index to search at and before.</param>
    /// <returns>The index for the prior new line or -1 if no new line was found.</returns>
    static private int findPriorNewLine(StringBuilder buf, int start) {
        for (int i = start; i >= 0; i--) {
            if (buf[i] == '\n') return i;
        }
        return -1;
    }

    /// <summary>Counts the number of spaces starting from the start up to the max count.</summary>
    /// <param name="buf">The buffer to check in.</param>
    /// <param name="start">The start index to start counting spaces from.</param>
    /// <param name="max">The maximum number of spaces to check for.</param>
    /// <returns>The number of spaces in the string from the given start.</returns>
    static private int countSpaces(StringBuilder buf, int start, int max = tabCount) {
        for (int i = 0; i < max; i++) {
            if (buf[start + i] != ' ') return i;
        }
        return max;
    }

    /// <summary>Handles tab being pressed with shift to move text to the left.</summary>
    private void handleBackwardTab() {
        int length = this.SelectionLength;
        if (length <= 0) {
            // Backwards tab with no selection (no op)
            return;
        }

        // Backwards tab with selection
        StringBuilder buf = new(this.Text);
        int start = this.SelectionStart;
        int index = start + length;
        int count;
        do {
            index = findPriorNewLine(buf, index-1);
            count = countSpaces(buf, index+1);
            if (count > 0) {
                buf.Remove(index+1, count);
                length -= count;
            }
        } while (index > start);
        start -= count;
        length += count;
        this.Text = buf.ToString();
        this.Select(start, length);
    }

    /// <summary>Handles tab being pressed without shift to move text to the right.</summary>
    private void handleForwardTab() {
        StringBuilder buf = new(this.Text);
        int start  = this.SelectionStart;
        int length = this.SelectionLength;

        if (length <= 0) {
            // Forward tab with no selection
            buf.Insert(start, tabSpaces);
            start += tabCount;
        } else {
            // Forward tab with selection
            int index = start + length;
            do {
                index = findPriorNewLine(buf, index-1);
                buf.Insert(index+1, tabSpaces);
                length += tabCount;
            } while (index > start);
            start += tabCount;
            length -= tabCount;
        }

        this.Text = buf.ToString();
        this.Select(start, length);
    }
}