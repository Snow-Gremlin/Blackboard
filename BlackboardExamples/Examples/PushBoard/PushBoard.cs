using Blackboard.Core.Extensions;
using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

/// <summary>A control for testing black board controls.</summary>
public partial class PushBoard : UserControl {

    /// <summary>The collection of all blackboard control children.</summary>
    private readonly IBlackBoardControl[] ctrls;

    /// <summary>Creates a new black board test control.</summary>
    public PushBoard() {
        this.InitializeComponent();
        this.ctrls = new IBlackBoardControl[] {
            this.trigger1,      this.trigger2,   this.trigger3,  this.trigger4,     this.trigger5,
            this.bool1,         this.bool2,      this.bool3,     this.bool4,        this.bool5,
            this.int1,          this.int2,       this.int3,      this.int4,         this.int5,
            this.string1,       this.string2,    this.string3,   this.string4,      this.string5,
            this.output1,       this.output2,    this.output3,   this.output4,      this.output5, this.output6,
            this.inputTriggers, this.inputBools, this.inputInts, this.inputStrings, this.output
        };
        this.errorBox.Visible = false;
    }

    private void presets_SelectedIndexChanged(object sender, EventArgs e) {

    }

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
            this.errorBox.Text = ex.Message;
            this.errorBox.Visible = true;
        }
    }

    private void codeInput_TextChanged(object sender, EventArgs e) {
        this.presets.ResetText();
        this.rebuildButton.Enabled = true;
    }
}
