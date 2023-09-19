using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A numeric integer up/down which can connect to a blackboard instance.</summary>
internal class BlackBoardIntUpDown: NumericUpDown, IBlackBoardControl {
    private readonly IBlackBoardComponent[] components;
    private InputValue<int>?  value;
    private InputValue<bool>? focusState;

    /// <summary>Creates a new numeric integer up/down.</summary>
    public BlackBoardIntUpDown() {
        this.Identifier = "numericUpDown";
        ValueWatcher<int>  valueWatcher     = new("value",     v => this.Value     = v,                 () => (int)this.Value);
        ValueWatcher<bool> enabledWatcher   = new("enabled",   v => this.Enabled   = v,                 () => this.Enabled);
        ValueWatcher<bool> visibleWatcher   = new("visible",   v => this.Visible   = v,                 () => this.Visible);
        ValueWatcher<bool> readOnlyWatcher  = new("readOnly",  v => this.ReadOnly  = v,                 () => this.ReadOnly);
        ValueWatcher<int>  backColorWatcher = new("backColor", v => this.BackColor = Color.FromArgb(v), this.BackColor.ToArgb);
        ValueWatcher<int>  foreColorWatcher = new("foreColor", v => this.ForeColor = Color.FromArgb(v), this.ForeColor.ToArgb);
        ValueWatcher<int>  maxValueWatcher  = new("max",       v => this.Maximum   = v,                 () => (int)this.Maximum);
        ValueWatcher<int>  minValueWatcher  = new("min",       v => this.Minimum   = v,                 () => (int)this.Minimum);
        ValueWatcher<int>  stepValueWatcher = new("step",      v => this.Increment = v,                 () => (int)this.Increment);
        this.components = new IBlackBoardComponent[] { valueWatcher, enabledWatcher, visibleWatcher, readOnlyWatcher,
            backColorWatcher, foreColorWatcher, maxValueWatcher, minValueWatcher, stepValueWatcher };
        this.value      = null;
        this.focusState = null;
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
        this.components.Foreach(c => c.Connect(b, this.Identifier));
        this.value      = b.ValueInput<int> (this.Identifier+".value");
        this.focusState = b.ValueInput<bool>(this.Identifier+".focus");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
        this.components.Foreach(c => c.Disconnect());
        this.value      = null;
        this.focusState = null;
    }

    protected override void OnValueChanged(EventArgs e) { base.OnValueChanged(e); this.value?.SetValue((int)this.Value); }
    protected override void OnGotFocus    (EventArgs e) { base.OnGotFocus(e);     this.focusState?.SetValue(true);  }
    protected override void OnLostFocus   (EventArgs e) { base.OnLostFocus(e);    this.focusState?.SetValue(false); }
}
