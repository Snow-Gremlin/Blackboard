using Blackboard.Core.Extensions;
using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

/// <summary>A control for testing black board controls.</summary>
public partial class PushBoard : UserControl {

    /// <summary>The collection of all blackboard control children.</summary>
    private readonly IBlackBoardControl[] ctrls;
    private bool suspendCodeChange;

    /// <summary>Creates a new black board test control.</summary>
    public PushBoard() {
        this.InitializeComponent();
        this.ctrls = new IBlackBoardControl[] {
            this.trigger1,      this.trigger2,   this.trigger3,  this.trigger4,     this.trigger5,
            this.bool1,         this.bool2,      this.bool3,     this.bool4,        this.bool5,
            this.int1,          this.int2,       this.int3,      this.int4,         this.int5,
            this.string1,       this.string2,    this.string3,   this.string4,      this.string5,
            this.output1,       this.output2,    this.output3,   this.output4,      this.output5,
            this.inputTriggers, this.inputBools, this.inputInts, this.inputStrings, this.outputs
        };
        this.suspendCodeChange  = false;
        this.presetDesc.Visible = false;
        this.errorBox.Visible   = false;
        this.presets.Items.Add(PresetItem.CustomInstance);
        this.setPresets();
        this.presets.SelectedIndex = 0;
    }

    /// <summary>Handles a preset is selected.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void presets_SelectedIndexChanged(object sender, EventArgs e) {
        PresetItem item = this.presets.SelectedItem as PresetItem ??
            throw new Exception("Unexpected preset item type.");

        if (item.Custom) {
            this.presetDesc.Visible = false;
            return;
        }

        this.suspendCodeChange  = true;
        this.errorBox.Visible   = false;
        this.codeInput.Lines    = item.Code;
        this.presetDesc.Text    = item.Description;
        this.presetDesc.Visible = true;
        this.suspendCodeChange  = false;
    }

    /// <summary>Handles rebuilding the code and a blackboard instance.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void rebuildButton_Click(object sender, EventArgs e) {
        try {
            this.rebuildButton.Enabled = false;
            this.errorBox.Visible = false;
            this.ctrls.Foreach(ctrl => ctrl.Disconnect());

            Blackboard.Blackboard b = new();
            this.ctrls.Foreach(ctrl => ctrl.Connect(b));
            b.Perform(this.codeInput.Lines);

        } catch (Exception ex) {
            this.ctrls.Foreach(ctrl => ctrl.Disconnect());
            this.presetDesc.Visible = false;
            this.errorBox.Text = ex.Message;
            this.errorBox.Visible = true;
        }
    }

    /// <summary>Handles the code text changing.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void codeInput_TextChanged(object sender, EventArgs e) {
        if (this.suspendCodeChange) return;
        this.presets.SelectedIndex = 0;
        this.rebuildButton.Enabled = true;
    }

    /// <summary>Adds a new preset item.</summary>
    /// <param name="name">The display name of the preset.</param>
    /// <param name="description">The description to display for this preset.</param>
    /// <param name="code">The code to set for the preset.</param>
    private void addPreset(string name, string description, params string[] code) =>
        this.presets.Items.Add(new PresetItem(name, description, code));

    /// <summary>Adds all the presets.</summary>
    private void setPresets() {
        this.addPreset(
            "Min/Max",
            "",
            "// Setup the push board",
            "namespace inputTriggers { visible := false; }",
            "namespace inputStrings  { visible := false; }",
            "namespace inputBools { text := \"options\"; }",
            "namespace bool1 { text := \"maximum\"; }",
            "namespace bool2 { visible := false; }",
            "namespace bool3 { visible := false; }",
            "namespace bool4 { visible := false; }",
            "namespace bool5 { visible := false; }");

        this.addPreset(
            "X",
            "",
            "X");
    }
}
