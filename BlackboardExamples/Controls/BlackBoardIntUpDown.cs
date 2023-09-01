using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A numeric integer up/down which can connect to a blackboard instance.</summary>
internal class BlackBoardIntUpDown: NumericUpDown, IBlackBoardControl {
    private bool priorEnabled;
    private bool priorVisible;
    private int  priorBackColor;
    private int  priorForeColor;
    private int  priorMax;
    private int  priorMin;
    private int  priorStep;

    private IValueWatcher<bool>? enabledWatcher;
    private IValueWatcher<bool>? visibleWatcher;
    private IValueWatcher<int>?  backColorWatcher;
    private IValueWatcher<int>?  foreColorWatcher;
    private IValueWatcher<int>?  maxValueWatcher;
    private IValueWatcher<int>?  minValueWatcher;
    private IValueWatcher<int>?  stepValueWatcher;

    private InputValue<int>?  onValueChanged;
    private InputValue<bool>? onFocusChanged;

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
        this.onFocusChanged   = null;
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

        this.priorMax        = (int)this.Maximum;
        this.maxValueWatcher = b.OnChange<int>(this.Identifier+".max");
        this.maxValueWatcher.OnChanged += this.onMaximumChanged;

        this.priorMin        = (int)this.Minimum;
        this.minValueWatcher = b.OnChange<int>(this.Identifier+".min");
        this.minValueWatcher.OnChanged += this.onMinimumChanged;

        this.priorStep        = (int)this.Increment;
        this.stepValueWatcher = b.OnChange<int>(this.Identifier+".step");
        this.stepValueWatcher.OnChanged += this.onStepChanged;

        this.onValueChanged = b.ValueInput<int>(this.Identifier+".value");
        this.onFocusChanged = b.ValueInput<bool>(this.Identifier+".focus");
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

        if (this.maxValueWatcher is not null) {
            this.maxValueWatcher.OnChanged -= this.onMaximumChanged;
            this.maxValueWatcher = null;
            this.Maximum = this.priorMax;
        }

        if (this.minValueWatcher is not null) {
            this.minValueWatcher.OnChanged -= this.onMinimumChanged;
            this.minValueWatcher = null;
            this.Minimum = this.priorMin;
        }

        if (this.stepValueWatcher is not null) {
            this.stepValueWatcher.OnChanged -= this.onStepChanged;
            this.stepValueWatcher = null;
            this.Increment = this.priorStep;
        }

        this.onValueChanged = null;
        this.onFocusChanged = null;
    }

    private void onTextChanged     (object? sender, ValueEventArgs<string> e) => this.Text      = e.Current;
    private void onEnabledChanged  (object? sender, ValueEventArgs<bool>   e) => this.Enabled   = e.Current;
    private void onVisibleChanged  (object? sender, ValueEventArgs<bool>   e) => this.Visible   = e.Current;
    private void onBackColorChanged(object? sender, ValueEventArgs<int>    e) => this.BackColor = Color.FromArgb(e.Current);
    private void onForeColorChanged(object? sender, ValueEventArgs<int>    e) => this.ForeColor = Color.FromArgb(e.Current);
    private void onMaximumChanged  (object? sender, ValueEventArgs<int>    e) => this.Maximum   = e.Current;
    private void onMinimumChanged  (object? sender, ValueEventArgs<int>    e) => this.Minimum   = e.Current;
    private void onStepChanged     (object? sender, ValueEventArgs<int>    e) => this.Increment = e.Current;

    protected override void OnValueChanged(EventArgs e) { base.OnValueChanged(e); this.onValueChanged?.SetValue((int)this.Value); }
    protected override void OnGotFocus    (EventArgs e) { base.OnGotFocus(e);     this.onFocusChanged?.SetValue(true);            }
    protected override void OnLostFocus   (EventArgs e) { base.OnLostFocus(e);    this.onFocusChanged?.SetValue(false);           }
}
