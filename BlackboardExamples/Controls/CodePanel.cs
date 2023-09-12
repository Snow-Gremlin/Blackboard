using System.Text;

namespace BlackboardExamples.Controls;

// TODO: Comment
sealed public class CodePanel : Panel {
    private const int tabCount = 3;
    private static readonly string tabSpaces = new(' ', tabCount);
    
    private int lineIndex;
    private RichTextBox editBox;

    public CodePanel() {
        this.lineIndex = 0;

        this.SuspendLayout();
        this.editBox             = new();
        this.editBox.Anchor      = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.editBox.Font        = new Font(FontFamily.GenericMonospace, 9.0f);
        this.editBox.Location    = new Point(6, 0);
        this.editBox.Size        = new Size(184, 200);
        this.editBox.Multiline   = true;
        this.editBox.WordWrap    = false;
        this.editBox.AcceptsTab  = true;
        this.editBox.BorderStyle = BorderStyle.None;

        this.editBox.KeyDown          += this.onKeyDown;
        this.editBox.SelectionChanged += this.onSelectionChanged;
        this.editBox.VScroll          += this.onVScroll;
        this.editBox.TextChanged      += this.onTextChanged;

        this.Controls.Add(this.editBox);
        this.BorderStyle = BorderStyle.Fixed3D;
        this.Font        = new Font(FontFamily.GenericMonospace, 9.0f);
        this.Size        = new Size(200, 200);
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.ResumeLayout(false);
    }

    public string[] Lines {
        get => this.editBox.Lines;
        set => this.editBox.Lines = value;
    }

    public void Select(int start, int length) => this.editBox.Select(start, length);

    public void ScrollToCaret() => this.editBox.ScrollToCaret();

    private void onKeyDown(object? sender, KeyEventArgs e) {
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
        int length = this.editBox.SelectionLength;
        if (length <= 0) {
            // Backwards tab with no selection (no op)
            return;
        }

        // Backwards tab with selection
        StringBuilder buf = new(this.Text);
        int start = this.editBox.SelectionStart;
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
        this.editBox.Select(start, length);
    }

    private void handleForwardTab() {
        StringBuilder buf = new(this.Text);
        int start  = this.editBox.SelectionStart;
        int length = this.editBox.SelectionLength;

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
        this.editBox.Select(start, length);
    }

    private void onSelectionChanged(object? sender, EventArgs args) {
        this.findLine();
        this.Invalidate();
    }
    
    private void onVScroll(object? sender, EventArgs e) {
        this.findLine();
        this.Invalidate();
    }

    private void findLine() {
        int charIndex  = this.editBox.GetCharIndexFromPosition(new Point(0, 0));
        this.lineIndex = this.editBox.GetLineFromCharIndex(charIndex);
    }

    private void drawLines(Graphics g) {
        g.Clear(this.BackColor);
        int counter = this.lineIndex + 1;
        int y = 2;
        int max = 0;
        while (y < this.ClientRectangle.Height - 15) {
            SizeF size = g.MeasureString(counter.ToString(), this.Font);
            g.DrawString(counter.ToString(), this.Font, new SolidBrush(this.ForeColor), new Point(3, y));
            counter++;
            y += (int)size.Height;
            if (max < size.Width) max = (int)size.Width;
        }
        max += 6;
        this.editBox.Location = new Point(max, 0);
        this.editBox.Size     = new Size(this.ClientRectangle.Width - max, this.ClientRectangle.Height);
    }

    protected override void OnPaint(PaintEventArgs e) {
        this.drawLines(e.Graphics);
        e.Graphics.TranslateTransform(50, 0);
        this.editBox.Invalidate();
        base.OnPaint(e);
    }

    private void onTextChanged(object? sender, EventArgs e) =>
        this.Text = this.editBox.Text;

    protected override void OnTextChanged(EventArgs e) {
        if (this.editBox.Text != this.Text)
            this.editBox.Text = this.Text;
        base.OnTextChanged(e);
    }




}