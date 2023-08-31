using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A button which can connect to a blackboard instance.</summary>
internal class BlackBoardButton : Button, IBlackBoardControl {
    private string priorText;
    private bool   priorEnabled;
    private bool   priorVisible;
    private int    priorBackColor;
    private int    priorForeColor;

    private IValueWatcher<string>? textWatcher;
    private IValueWatcher<bool>?   enabledWatcher;
    private IValueWatcher<bool>?   visibleWatcher;
    private IValueWatcher<int>?    backColorWatcher;
    private IValueWatcher<int>?    foreColorWatcher;

    private InputTrigger?     onClickTrigger;
    private InputValue<bool>? onFocusChanged;

    /// <summary>Creates a new button.</summary>
    public BlackBoardButton() {
        this.priorText        = string.Empty;
        this.priorEnabled     = false;
        this.priorVisible     = false;
        this.priorBackColor   = 0;
        this.priorForeColor   = 0;
        this.Identifier       = "button";
        this.textWatcher      = null;
        this.enabledWatcher   = null;
        this.visibleWatcher   = null;
        this.backColorWatcher = null;
        this.foreColorWatcher = null;
        this.onClickTrigger   = null;
        this.onFocusChanged   = null;
    }

    /// <summary>The identifier and optional namespace to write this button to.</summary>
    [Category("Design")]
    [DefaultValue("button")]
    [Description("The identifier and optional namespace to write this button to.")]
    public string Identifier { get; set; }

    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) {
        if (this.textWatcher is not null) this.Disconnect();

        this.priorText   = this.Text;
        this.textWatcher = b.OnChange<string>(this.Identifier+".text");
        this.textWatcher.OnChanged += this.onTextChanged;

        this.priorEnabled   = this.Enabled;
        this.enabledWatcher = b.OnChange<bool>(this.Identifier+".enabled");
        this.enabledWatcher.OnChanged += this.onEnabledChanged;
        
        this.priorVisible   = this.Visible;
        this.visibleWatcher = b.OnChange<bool>(this.Identifier+".visible");
        this.visibleWatcher.OnChanged += this.onVisibleChanged;

        this.priorBackColor   = this.BackColor.ToArgb();
        this.backColorWatcher = b.OnChange<int>(this.Identifier+".backColor");
        this.backColorWatcher.OnChanged += this.onBackColorChanged;

        this.priorForeColor   = this.ForeColor.ToArgb();
        this.foreColorWatcher = b.OnChange<int>(this.Identifier+".foreColor");
        this.foreColorWatcher.OnChanged += this.onForeColorChanged;

        this.onClickTrigger = b.Provoker(this.Identifier+".onClick");
        this.onFocusChanged = b.ValueInput<bool>(this.Identifier+".focus");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
        if (this.textWatcher is not null) {
            this.textWatcher.OnChanged -= this.onTextChanged;
            this.textWatcher = null;
            this.Text = this.priorText;
        }

        if (this.enabledWatcher is not null) {
            this.enabledWatcher.OnChanged -= this.onEnabledChanged;
            this.enabledWatcher = null;
            this.Enabled = this.priorEnabled;
        }
        
        if (this.visibleWatcher is not null) {
            this.visibleWatcher.OnChanged -= this.onVisibleChanged;
            this.visibleWatcher = null;
            this.Visible = this.priorVisible;
        }

        if (this.backColorWatcher is not null) {
            this.backColorWatcher.OnChanged -= this.onBackColorChanged;
            this.backColorWatcher = null;
            this.BackColor = Color.FromArgb(this.priorBackColor);
        }

        if (this.foreColorWatcher is not null) {
            this.foreColorWatcher.OnChanged -= this.onForeColorChanged;
            this.foreColorWatcher = null;
            this.ForeColor = Color.FromArgb(this.priorForeColor);
        }

        this.onClickTrigger = null;
        this.onFocusChanged = null;
    }

    private void onTextChanged     (object? sender, ValueEventArgs<string> e) => this.Text      = e.Current;
    private void onEnabledChanged  (object? sender, ValueEventArgs<bool>   e) => this.Enabled   = e.Current;
    private void onVisibleChanged  (object? sender, ValueEventArgs<bool>   e) => this.Visible   = e.Current;
    private void onBackColorChanged(object? sender, ValueEventArgs<int>    e) => this.BackColor = Color.FromArgb(e.Current);
    private void onForeColorChanged(object? sender, ValueEventArgs<int>    e) => this.ForeColor = Color.FromArgb(e.Current);

    protected override void OnClick    (EventArgs e) { base.OnClick(e);     this.onClickTrigger?.Provoke();       }
    protected override void OnGotFocus (EventArgs e) { base.OnGotFocus(e);  this.onFocusChanged?.SetValue(true);  }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); this.onFocusChanged?.SetValue(false); }
}
