using Blackboard.Core.Extensions;
using Blackboard.Core.Record;
using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

/// <summary>A control for testing black board controls.</summary>
public partial class PushBoard : UserControl {
    private readonly IBlackBoardControl[] ctrls;
    private bool suspendCodeChange;
    private Blackboard.Blackboard? blackboard;

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
        this.suspendCodeChange = false;
        this.blackboard        = null;
        this.runButton.Enabled = false;
        this.presets.Items.Add(PresetItem.CustomInstance);
        this.setPresets();
        this.presets.SelectedIndex = 0;
    }

    /// <summary>Handles a quick command being run.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void runButton_Click(object sender, EventArgs e) {
        try {
            if (this.quickCommand.Text == "print")
                Console.WriteLine(this.blackboard?.ToString());
            else {
                Result? result = this.blackboard?.Perform(this.quickCommand.Lines);
                if (result is not null && result.HasOutput) Console.WriteLine(result);
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>Handles the enter key being pressed on the quick command.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void quickCommand_KeyDown(object sender, KeyEventArgs e) {
        if (e.KeyCode == Keys.Enter) this.runButton.PerformClick();
    }

    /// <summary>Handles a preset is selected.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void presets_SelectedIndexChanged(object sender, EventArgs e) {
        PresetItem item = (PresetItem)this.presets.SelectedItem;
        if (item.Custom) return;

        this.suspendCodeChange     = true;
        this.codeInput.Lines       = item.Code;
        this.rebuildButton.Enabled = true;
        this.suspendCodeChange     = false;
    }

    /// <summary>Handles rebuilding the code and a blackboard instance.</summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void rebuildButton_Click(object sender, EventArgs e) {
        try {
            this.rebuildButton.Enabled = false;
            this.ctrls.Foreach(ctrl => ctrl.Disconnect());

            Blackboard.Blackboard b = new();
            this.ctrls.Foreach(ctrl => ctrl.Connect(b));
            Result result = b.Perform(this.codeInput.Lines);
            if (result.HasOutput) Console.WriteLine(result);

            this.runButton.Enabled = true;
            this.blackboard = b;

        } catch (Exception ex) {
            this.ctrls.Foreach(ctrl => ctrl.Disconnect());
            this.runButton.Enabled = false;
            Console.WriteLine(ex.Message);
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
    /// <param name="code">The code to set for the preset.</param>
    private void addPreset(string name, params string[] code) =>
        this.presets.Items.Add(new PresetItem(name, code));

    /// <summary>Adds all the presets.</summary>
    private void setPresets() {
        this.addPreset("Min/Max",
            "// This preset changes the push board example into five input values.",
            "// The minimum value of the input values is printed to the output.",
            "// The check box changes the output from minimum to maximum.",
            "",
            "// Setup the push board",
            "namespace inputTriggers { visible := false; }",
            "namespace inputStrings  { visible := false; }",
            "namespace inputBools { text := \"options\"; }",
            "namespace bool1 { text := \"max\"; }",
            "namespace bool2 { visible := false; }",
            "namespace bool3 { visible := false; }",
            "namespace bool4 { visible := false; }",
            "namespace bool5 { visible := false; }",
            "namespace output2 { visible := false; }",
            "namespace output3 { visible := false; }",
            "namespace output4 { visible := false; }",
            "namespace output5 { visible := false; }",
            "",
            "// Setup the example",
            "namespace output1 {",
            "   string text := bool1.checked ?",
            "      max(int1.value, int2.value, int3.value, int4.value, int5.value) :",
            "      min(int1.value, int2.value, int3.value, int4.value, int5.value);",
            "}");

        this.addPreset("X",
            "X");
    }
}
