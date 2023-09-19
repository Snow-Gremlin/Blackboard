using Blackboard.Core.Extensions;
using System.ComponentModel;

namespace BlackboardExamples.Controls;

/// <summary>A check box which can connect to a blackboard instance.</summary>
sealed public class BlackBoardGroupBox : GroupBox, IBlackBoardControl {
    private readonly IBlackBoardComponent[] components;

    /// <summary>Creates a new group box.</summary>
    public BlackBoardGroupBox() {
        this.Identifier = "groupBox";
        ValueWatcher<string> textWatcher      = new("text",      v => this.Text      = v,                 () => this.Text);
        ValueWatcher<bool>   enabledWatcher   = new("enabled",   v => this.Enabled   = v,                 () => this.Enabled);
        ValueWatcher<bool>   visibleWatcher   = new("visible",   v => this.Visible   = v,                 () => this.Visible);
        ValueWatcher<int>    backColorWatcher = new("backColor", v => this.BackColor = Color.FromArgb(v), this.BackColor.ToArgb);
        ValueWatcher<int>    foreColorWatcher = new("foreColor", v => this.ForeColor = Color.FromArgb(v), this.ForeColor.ToArgb);
        this.components = new IBlackBoardComponent[] { textWatcher, enabledWatcher, visibleWatcher, backColorWatcher, foreColorWatcher };
    }
    
    /// <summary>The identifier and optional namespace to write this group box to.</summary>
    [Category("Design")]
    [DefaultValue("groupBox")]
    [Description("The identifier and optional namespace to write this group box to.")]
    public string Identifier { get; set; }

    /// <summary>Connects this control to the given blackboard.</summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) => this.components.Foreach(c => c.Connect(b, this.Identifier));

    /// <summary>Disconnects this control from a blackboard.</summary>
    /// <remarks>This has no effect if not connected.</remarks>
    public void Disconnect() => this.components.Foreach(c => c.Disconnect());
}
