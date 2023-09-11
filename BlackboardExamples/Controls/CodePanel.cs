using System;
using System.Text;

namespace BlackboardExamples.Controls;

// TODO: Comment
sealed public class CodePanel : TextBox {
     private static readonly string tabSpaces = "   ";
     private static readonly int tabCount = tabSpaces.Length;
    
    public CodePanel() {
        this.Dock       = DockStyle.Fill;
        this.Font       = new Font(FontFamily.GenericMonospace, 9.0f);
        this.ScrollBars = ScrollBars.Both;
        this.Multiline  = true;
        this.WordWrap   = false;
        this.AcceptsTab = true;
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.KeyCode == Keys.Tab) {
            if (e.Modifiers == Keys.Shift) 
                this.handleBackwardTab();
            else this.handleForwardTab();
            e.Handled = true;
            e.SuppressKeyPress = true;
        } else base.OnKeyDown(e);
    }

    static private int findPriorNewLine(StringBuilder buf, int start) {
        for (int i = start; i >= 0; i--) {
            if (buf[i] == '\n') return i;
        }
        return -1;
    }

    static private int countSpaces(StringBuilder buf, int start) {
        for (int i = 0; i < tabCount; i++) {
            if (buf[start + i] != ' ') return i;
        }
        return tabCount;
    }

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

    private void handleForwardTab() {
        StringBuilder buf = new(this.Text);
        int start  = this.SelectionStart;
        int length = this.SelectionLength;
        
        if (length <= 0) {
            // Forward tab with no selection
            buf.Insert(start, tabSpaces);
            start += tabCount;
        }  else {
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
