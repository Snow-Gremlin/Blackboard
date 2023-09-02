using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A text box which can connect to a blackboard instance.</summary>
internal class BlackBoardTextBox: TextBox, IBlackBoardControl {
    private readonly IBlackBoardComponent[] components;
    private InputValue<string>? onTextChanged;
    private InputValue<bool>?   onFocusChanged;

    /// <summary>Creates a new text box.</summary>
    public BlackBoardTextBox() {
        this.Identifier = "textBox";
        ValueWatcher<string> textWatcher      = new("text",      v => this.Text      = v,                 () => this.Text);
        ValueWatcher<bool>   enabledWatcher   = new("enabled",   v => this.Enabled   = v,                 () => this.Enabled);
        ValueWatcher<bool>   visibleWatcher   = new("visible",   v => this.Visible   = v,                 () => this.Visible);
        ValueWatcher<bool>   readOnlyWatcher  = new("readOnly",  v => this.ReadOnly  = v,                 () => this.ReadOnly);
        ValueWatcher<int>    backColorWatcher = new("backColor", v => this.BackColor = Color.FromArgb(v), this.BackColor.ToArgb);
        ValueWatcher<int>    foreColorWatcher = new("foreColor", v => this.ForeColor = Color.FromArgb(v), this.ForeColor.ToArgb);
        this.components = new IBlackBoardComponent[] { textWatcher, enabledWatcher,
            visibleWatcher, readOnlyWatcher, backColorWatcher, foreColorWatcher };
        this.onTextChanged  = null;
        this.onFocusChanged = null;
    }

    /// <summary>The identifier and optional namespace to write this text box to.</summary>
    [Category("Design")]
    [DefaultValue("textBox")]
    [Description("The identifier and optional namespace to write this text box to.")]
    public string Identifier { get; set; }

    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) {
        this.components.Foreach(c => c.Connect(b, this.Identifier));
        this.onTextChanged  = b.ValueInput<string>(this.Identifier+".text");
        this.onFocusChanged = b.ValueInput<bool>  (this.Identifier+".focus");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
        this.components.Foreach(c => c.Disconnect());
        this.onTextChanged  = null;
        this.onFocusChanged = null;
    }

    protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); this.onTextChanged?.SetValue(this.Text); }
    protected override void OnGotFocus   (EventArgs e) { base.OnGotFocus(e);    this.onFocusChanged?.SetValue(true);            }
    protected override void OnLostFocus  (EventArgs e) { base.OnLostFocus(e);   this.onFocusChanged?.SetValue(false);           }
}
