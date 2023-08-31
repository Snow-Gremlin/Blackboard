using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A numeric integer up/down which can connect to a blackboard instance.</summary>
internal class BlackBoardIntUpDown: NumericUpDown, IBlackBoardControl {
    private bool priorEnabled;
    private bool priorVisible;
    private int  priorBackColor;
    private int  priorForeColor;

    private IValueWatcher<bool>? enabledWatcher;
    private IValueWatcher<bool>? visibleWatcher;
    private IValueWatcher<int>?  backColorWatcher;
    private IValueWatcher<int>?  foreColorWatcher;
    private IValueWatcher<int>?  maxValueWatcher;
    private IValueWatcher<int>?  minValueWatcher;
    private IValueWatcher<int>?  stepValueWatcher;

    private InputValue<int>? onValueChanged;

    /// <summary>Creates a new numeric integer up/down.</summary>
    public BlackBoardIntUpDown() {
        this.priorEnabled     = false;
        this.priorBackColor   = 0;
        this.priorForeColor   = 0;
        this.Identifier       = "numericUpDown";
        this.enabledWatcher   = null;
        this.visibleWatcher   = null;
        this.backColorWatcher = null;
        this.foreColorWatcher = null;
        this.maxValueWatcher  = null;
        this.minValueWatcher  = null;
        this.stepValueWatcher = null;
        this.onValueChanged   = null;
    }

    /// <summary>The identifier and optional namespace to write this numeric up/down to.</summary>
    [Category("Design")]
    [DefaultValue("numericUpDown")]
    [Description("The identifier and optional namespace to write this numeric up/down to.")]
    public string Identifier { get; set; }

    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) {
        if (this.enabledWatcher is not null) this.Disconnect();

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

        this.onValueChanged = b.ValueInput<int>(this.Identifier+".value");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
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

        this.onValueChanged = null;
    }

    private void onTextChanged     (object? sender, ValueEventArgs<string> e) => this.Text      = e.Current;
    private void onEnabledChanged  (object? sender, ValueEventArgs<bool>   e) => this.Enabled   = e.Current;
    private void onVisibleChanged  (object? sender, ValueEventArgs<bool>   e) => this.Visible   = e.Current;
    private void onBackColorChanged(object? sender, ValueEventArgs<int>    e) => this.BackColor = Color.FromArgb(e.Current);
    private void onForeColorChanged(object? sender, ValueEventArgs<int>    e) => this.ForeColor = Color.FromArgb(e.Current);
    protected override void OnValueChanged(EventArgs e) { base.OnValueChanged(e); this.onValueChanged?.SetValue((int)this.Value); }
}
