using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A button which can connect to a blackboard instance.</summary>
internal class BlackBoardButton : Button, IBlackBoardControl {
    private readonly IBlackBoardComponent[] components;
    private InputTrigger?     onClickTrigger;
    private InputValue<bool>? onFocusChanged;

    /// <summary>Creates a new button.</summary>
    public BlackBoardButton() {
        this.Identifier = "button";
        ValueWatcher<string> textWatcher      = new("text",      v => this.Text      = v,                 () => this.Text);
        ValueWatcher<bool>   enabledWatcher   = new("enabled",   v => this.Enabled   = v,                 () => this.Enabled);
        ValueWatcher<bool>   visibleWatcher   = new("visible",   v => this.Visible   = v,                 () => this.Visible);
        ValueWatcher<int>    backColorWatcher = new("backColor", v => this.BackColor = Color.FromArgb(v), this.BackColor.ToArgb);
        ValueWatcher<int>    foreColorWatcher = new("foreColor", v => this.ForeColor = Color.FromArgb(v), this.ForeColor.ToArgb);
        this.components = new IBlackBoardComponent[] { textWatcher, enabledWatcher, visibleWatcher, backColorWatcher, foreColorWatcher };
        this.onClickTrigger = null;
        this.onFocusChanged = null;
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
        this.components.Foreach(c => c.Connect(b, this.Identifier));
        this.onClickTrigger = b.Provoker(this.Identifier+".onClick");
        this.onFocusChanged = b.ValueInput<bool>(this.Identifier+".focus");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
        this.components.Foreach(c => c.Disconnect());
        this.onClickTrigger = null;
        this.onFocusChanged = null;
    }

    protected override void OnClick    (EventArgs e) { base.OnClick(e);     this.onClickTrigger?.Provoke();       }
    protected override void OnGotFocus (EventArgs e) { base.OnGotFocus(e);  this.onFocusChanged?.SetValue(true);  }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); this.onFocusChanged?.SetValue(false); }
}
