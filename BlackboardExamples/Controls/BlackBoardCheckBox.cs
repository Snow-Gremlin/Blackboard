﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A check box which can connect to a blackboard instance.</summary>
sealed public class BlackBoardCheckBox : CheckBox, IBlackBoardControl {
    private readonly IBlackBoardComponent[] components;
    private InputValue<bool>? onCheckTrigger;
    private InputValue<bool>? onFocusChanged;

    /// <summary>Creates a new check box.</summary>
    public BlackBoardCheckBox() {
        this.Identifier = "checkBox";
        ValueWatcher<string> textWatcher      = new("text",      v => this.Text      = v,                 () => this.Text);
        ValueWatcher<bool>   enabledWatcher   = new("enabled",   v => this.Enabled   = v,                 () => this.Enabled);
        ValueWatcher<bool>   visibleWatcher   = new("visible",   v => this.Visible   = v,                 () => this.Visible);
        ValueWatcher<int>    backColorWatcher = new("backColor", v => this.BackColor = Color.FromArgb(v), this.BackColor.ToArgb);
        ValueWatcher<int>    foreColorWatcher = new("foreColor", v => this.ForeColor = Color.FromArgb(v), this.ForeColor.ToArgb);
        this.components = new IBlackBoardComponent[] { textWatcher, enabledWatcher, visibleWatcher, backColorWatcher, foreColorWatcher };
        this.onCheckTrigger = null;
        this.onFocusChanged = null;
    }

    /// <summary>The identifier and optional namespace to write this check box to.</summary>
    [Category("Design")]
    [DefaultValue("checkBox")]
    [Description("The identifier and optional namespace to write this check box to.")]
    public string Identifier { get; set; }

    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) {
        this.components.Foreach(c => c.Connect(b, this.Identifier));
        this.onCheckTrigger = b.ValueInput<bool>(this.Identifier+".checked");
        this.onFocusChanged = b.ValueInput<bool>(this.Identifier+".focus");
    }

    /// <summary>Disconnects this control from a blackboard.</summary>
    public void Disconnect() {
        this.components.Foreach(c => c.Disconnect());
        this.onCheckTrigger = null;
        this.onFocusChanged = null;
    }

    protected override void OnCheckedChanged(EventArgs e) { base.OnCheckedChanged(e); this.onCheckTrigger?.SetValue(this.Checked); }
    protected override void OnGotFocus      (EventArgs e) { base.OnGotFocus(e);       this.onFocusChanged?.SetValue(true);         }
    protected override void OnLostFocus     (EventArgs e) { base.OnLostFocus(e);      this.onFocusChanged?.SetValue(false);        }
}
