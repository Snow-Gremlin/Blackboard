using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

partial class PushBoard {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        inputTriggers=new BlackBoardGroupBox();
        inputTriggersFlow=new FlowLayoutPanel();
        trigger1=new BlackBoardButton();
        trigger2=new BlackBoardButton();
        trigger3=new BlackBoardButton();
        trigger4=new BlackBoardButton();
        trigger5=new BlackBoardButton();
        inputBools=new BlackBoardGroupBox();
        inputBoolsFlow=new FlowLayoutPanel();
        bool1=new BlackBoardCheckBox();
        bool2=new BlackBoardCheckBox();
        bool3=new BlackBoardCheckBox();
        bool4=new BlackBoardCheckBox();
        bool5=new BlackBoardCheckBox();
        inputInts=new BlackBoardGroupBox();
        inputIntsFlow=new FlowLayoutPanel();
        int1=new BlackBoardIntUpDown();
        int2=new BlackBoardIntUpDown();
        int3=new BlackBoardIntUpDown();
        int4=new BlackBoardIntUpDown();
        int5=new BlackBoardIntUpDown();
        inputStrings=new BlackBoardGroupBox();
        inputStringsFlow=new FlowLayoutPanel();
        string1=new BlackBoardTextBox();
        string2=new BlackBoardTextBox();
        string3=new BlackBoardTextBox();
        string4=new BlackBoardTextBox();
        string5=new BlackBoardTextBox();
        outputs=new BlackBoardGroupBox();
        outputsFlow=new FlowLayoutPanel();
        output1=new BlackBoardTextBox();
        output2=new BlackBoardTextBox();
        output3=new BlackBoardTextBox();
        output4=new BlackBoardTextBox();
        output5=new BlackBoardTextBox();
        splitContainer=new SplitContainer();
        mainFlow=new FlowLayoutPanel();
        setupGroup=new GroupBox();
        presets=new ComboBox();
        rebuildButton=new Button();
        codePanel=new Panel();
        codeInput=new TextBox();
        errorBox=new TextBox();
        runGroup=new GroupBox();
        quickCommand=new TextBox();
        runButton=new Button();
        inputTriggers.SuspendLayout();
        inputTriggersFlow.SuspendLayout();
        inputBools.SuspendLayout();
        inputBoolsFlow.SuspendLayout();
        inputInts.SuspendLayout();
        inputIntsFlow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)int1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int5).BeginInit();
        inputStrings.SuspendLayout();
        inputStringsFlow.SuspendLayout();
        outputs.SuspendLayout();
        outputsFlow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        mainFlow.SuspendLayout();
        setupGroup.SuspendLayout();
        codePanel.SuspendLayout();
        runGroup.SuspendLayout();
        this.SuspendLayout();
        // 
        // inputTriggers
        // 
        inputTriggers.AutoSize=true;
        inputTriggers.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputTriggers.Controls.Add(inputTriggersFlow);
        inputTriggers.Identifier="inputTriggers";
        inputTriggers.Location=new Point(3, 3);
        inputTriggers.Name="inputTriggers";
        inputTriggers.Size=new Size(521, 53);
        inputTriggers.TabIndex=1;
        inputTriggers.TabStop=false;
        inputTriggers.Text="Input Triggers";
        // 
        // inputTriggersFlow
        // 
        inputTriggersFlow.AutoSize=true;
        inputTriggersFlow.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputTriggersFlow.Controls.Add(trigger1);
        inputTriggersFlow.Controls.Add(trigger2);
        inputTriggersFlow.Controls.Add(trigger3);
        inputTriggersFlow.Controls.Add(trigger4);
        inputTriggersFlow.Controls.Add(trigger5);
        inputTriggersFlow.Dock=DockStyle.Fill;
        inputTriggersFlow.Location=new Point(3, 19);
        inputTriggersFlow.Name="inputTriggersFlow";
        inputTriggersFlow.Size=new Size(515, 31);
        inputTriggersFlow.TabIndex=5;
        // 
        // trigger1
        // 
        trigger1.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger1.Identifier="trigger1";
        trigger1.Location=new Point(3, 3);
        trigger1.MaximumSize=new Size(175, 25);
        trigger1.MinimumSize=new Size(75, 25);
        trigger1.Name="trigger1";
        trigger1.Size=new Size(97, 25);
        trigger1.TabIndex=0;
        trigger1.Text="trigger 1";
        trigger1.UseVisualStyleBackColor=true;
        // 
        // trigger2
        // 
        trigger2.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger2.Identifier="trigger2";
        trigger2.Location=new Point(106, 3);
        trigger2.MaximumSize=new Size(175, 25);
        trigger2.MinimumSize=new Size(75, 25);
        trigger2.Name="trigger2";
        trigger2.Size=new Size(97, 25);
        trigger2.TabIndex=1;
        trigger2.Text="trigger 2";
        trigger2.UseVisualStyleBackColor=true;
        // 
        // trigger3
        // 
        trigger3.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger3.Identifier="trigger3";
        trigger3.Location=new Point(209, 3);
        trigger3.MaximumSize=new Size(175, 25);
        trigger3.MinimumSize=new Size(75, 25);
        trigger3.Name="trigger3";
        trigger3.Size=new Size(97, 25);
        trigger3.TabIndex=2;
        trigger3.Text="trigger 3";
        trigger3.UseVisualStyleBackColor=true;
        // 
        // trigger4
        // 
        trigger4.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger4.Identifier="trigger4";
        trigger4.Location=new Point(312, 3);
        trigger4.MaximumSize=new Size(175, 25);
        trigger4.MinimumSize=new Size(75, 25);
        trigger4.Name="trigger4";
        trigger4.Size=new Size(97, 25);
        trigger4.TabIndex=3;
        trigger4.Text="trigger 4";
        trigger4.UseVisualStyleBackColor=true;
        // 
        // trigger5
        // 
        trigger5.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger5.Identifier="trigger5";
        trigger5.Location=new Point(415, 3);
        trigger5.MaximumSize=new Size(175, 25);
        trigger5.MinimumSize=new Size(75, 25);
        trigger5.Name="trigger5";
        trigger5.Size=new Size(97, 25);
        trigger5.TabIndex=4;
        trigger5.Text="trigger 5";
        trigger5.UseVisualStyleBackColor=true;
        // 
        // inputBools
        // 
        inputBools.AutoSize=true;
        inputBools.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputBools.Controls.Add(inputBoolsFlow);
        inputBools.Identifier="inputBools";
        inputBools.Location=new Point(3, 178);
        inputBools.Name="inputBools";
        inputBools.Size=new Size(411, 53);
        inputBools.TabIndex=2;
        inputBools.TabStop=false;
        inputBools.Text="Input Booleans";
        // 
        // inputBoolsFlow
        // 
        inputBoolsFlow.AutoSize=true;
        inputBoolsFlow.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputBoolsFlow.Controls.Add(bool1);
        inputBoolsFlow.Controls.Add(bool2);
        inputBoolsFlow.Controls.Add(bool3);
        inputBoolsFlow.Controls.Add(bool4);
        inputBoolsFlow.Controls.Add(bool5);
        inputBoolsFlow.Dock=DockStyle.Fill;
        inputBoolsFlow.Location=new Point(3, 19);
        inputBoolsFlow.Name="inputBoolsFlow";
        inputBoolsFlow.Size=new Size(405, 31);
        inputBoolsFlow.TabIndex=5;
        // 
        // bool1
        // 
        bool1.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool1.Identifier="bool1";
        bool1.Location=new Point(3, 3);
        bool1.MaximumSize=new Size(175, 25);
        bool1.MinimumSize=new Size(75, 25);
        bool1.Name="bool1";
        bool1.Size=new Size(75, 25);
        bool1.TabIndex=0;
        bool1.Text="bool 1";
        bool1.UseVisualStyleBackColor=true;
        // 
        // bool2
        // 
        bool2.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool2.Identifier="bool2";
        bool2.Location=new Point(84, 3);
        bool2.MaximumSize=new Size(175, 25);
        bool2.MinimumSize=new Size(75, 25);
        bool2.Name="bool2";
        bool2.Size=new Size(75, 25);
        bool2.TabIndex=1;
        bool2.Text="bool 2";
        bool2.UseVisualStyleBackColor=true;
        // 
        // bool3
        // 
        bool3.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool3.Identifier="bool3";
        bool3.Location=new Point(165, 3);
        bool3.MaximumSize=new Size(175, 25);
        bool3.MinimumSize=new Size(75, 25);
        bool3.Name="bool3";
        bool3.Size=new Size(75, 25);
        bool3.TabIndex=2;
        bool3.Text="bool 3";
        bool3.UseVisualStyleBackColor=true;
        // 
        // bool4
        // 
        bool4.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool4.Identifier="bool4";
        bool4.Location=new Point(246, 3);
        bool4.MaximumSize=new Size(175, 25);
        bool4.MinimumSize=new Size(75, 25);
        bool4.Name="bool4";
        bool4.Size=new Size(75, 25);
        bool4.TabIndex=3;
        bool4.Text="bool 4";
        bool4.UseVisualStyleBackColor=true;
        // 
        // bool5
        // 
        bool5.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool5.Identifier="bool5";
        bool5.Location=new Point(327, 3);
        bool5.MaximumSize=new Size(175, 25);
        bool5.MinimumSize=new Size(75, 25);
        bool5.Name="bool5";
        bool5.Size=new Size(75, 25);
        bool5.TabIndex=4;
        bool5.Text="bool 5";
        bool5.UseVisualStyleBackColor=true;
        // 
        // inputInts
        // 
        inputInts.AutoSize=true;
        inputInts.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputInts.Controls.Add(inputIntsFlow);
        inputInts.Identifier="inputInts";
        inputInts.Location=new Point(3, 62);
        inputInts.Name="inputInts";
        inputInts.Size=new Size(521, 51);
        inputInts.TabIndex=3;
        inputInts.TabStop=false;
        inputInts.Text="Input Integers";
        // 
        // inputIntsFlow
        // 
        inputIntsFlow.AutoSize=true;
        inputIntsFlow.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputIntsFlow.Controls.Add(int1);
        inputIntsFlow.Controls.Add(int2);
        inputIntsFlow.Controls.Add(int3);
        inputIntsFlow.Controls.Add(int4);
        inputIntsFlow.Controls.Add(int5);
        inputIntsFlow.Dock=DockStyle.Fill;
        inputIntsFlow.Location=new Point(3, 19);
        inputIntsFlow.Name="inputIntsFlow";
        inputIntsFlow.Size=new Size(515, 29);
        inputIntsFlow.TabIndex=5;
        // 
        // int1
        // 
        int1.Identifier="int1";
        int1.Location=new Point(3, 3);
        int1.MaximumSize=new Size(175, 0);
        int1.MinimumSize=new Size(75, 0);
        int1.Name="int1";
        int1.Size=new Size(97, 23);
        int1.TabIndex=0;
        // 
        // int2
        // 
        int2.Identifier="int2";
        int2.Location=new Point(106, 3);
        int2.MaximumSize=new Size(175, 0);
        int2.MinimumSize=new Size(75, 0);
        int2.Name="int2";
        int2.Size=new Size(97, 23);
        int2.TabIndex=1;
        // 
        // int3
        // 
        int3.Identifier="int3";
        int3.Location=new Point(209, 3);
        int3.MaximumSize=new Size(175, 0);
        int3.MinimumSize=new Size(75, 0);
        int3.Name="int3";
        int3.Size=new Size(97, 23);
        int3.TabIndex=2;
        // 
        // int4
        // 
        int4.Identifier="int4";
        int4.Location=new Point(312, 3);
        int4.MaximumSize=new Size(175, 0);
        int4.MinimumSize=new Size(75, 0);
        int4.Name="int4";
        int4.Size=new Size(97, 23);
        int4.TabIndex=3;
        // 
        // int5
        // 
        int5.Identifier="int5";
        int5.Location=new Point(415, 3);
        int5.MaximumSize=new Size(175, 0);
        int5.MinimumSize=new Size(75, 0);
        int5.Name="int5";
        int5.Size=new Size(97, 23);
        int5.TabIndex=4;
        // 
        // inputStrings
        // 
        inputStrings.AutoSize=true;
        inputStrings.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputStrings.Controls.Add(inputStringsFlow);
        inputStrings.Identifier="inputStrings";
        inputStrings.Location=new Point(3, 119);
        inputStrings.Name="inputStrings";
        inputStrings.Size=new Size(521, 53);
        inputStrings.TabIndex=4;
        inputStrings.TabStop=false;
        inputStrings.Text="Input Strings";
        // 
        // inputStringsFlow
        // 
        inputStringsFlow.AutoSize=true;
        inputStringsFlow.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        inputStringsFlow.Controls.Add(string1);
        inputStringsFlow.Controls.Add(string2);
        inputStringsFlow.Controls.Add(string3);
        inputStringsFlow.Controls.Add(string4);
        inputStringsFlow.Controls.Add(string5);
        inputStringsFlow.Dock=DockStyle.Fill;
        inputStringsFlow.Location=new Point(3, 19);
        inputStringsFlow.Name="inputStringsFlow";
        inputStringsFlow.Size=new Size(515, 31);
        inputStringsFlow.TabIndex=10;
        // 
        // string1
        // 
        string1.Identifier="string1";
        string1.Location=new Point(3, 3);
        string1.MaximumSize=new Size(175, 25);
        string1.MinimumSize=new Size(75, 25);
        string1.Name="string1";
        string1.Size=new Size(97, 25);
        string1.TabIndex=5;
        // 
        // string2
        // 
        string2.Identifier="string2";
        string2.Location=new Point(106, 3);
        string2.MaximumSize=new Size(175, 25);
        string2.MinimumSize=new Size(75, 25);
        string2.Name="string2";
        string2.Size=new Size(97, 25);
        string2.TabIndex=6;
        // 
        // string3
        // 
        string3.Identifier="string3";
        string3.Location=new Point(209, 3);
        string3.MaximumSize=new Size(175, 25);
        string3.MinimumSize=new Size(75, 25);
        string3.Name="string3";
        string3.Size=new Size(97, 25);
        string3.TabIndex=7;
        // 
        // string4
        // 
        string4.Identifier="string4";
        string4.Location=new Point(312, 3);
        string4.MaximumSize=new Size(175, 25);
        string4.MinimumSize=new Size(75, 25);
        string4.Name="string4";
        string4.Size=new Size(97, 25);
        string4.TabIndex=8;
        // 
        // string5
        // 
        string5.Identifier="string5";
        string5.Location=new Point(415, 3);
        string5.MaximumSize=new Size(175, 25);
        string5.MinimumSize=new Size(75, 25);
        string5.Name="string5";
        string5.Size=new Size(97, 25);
        string5.TabIndex=9;
        // 
        // outputs
        // 
        outputs.AutoSize=true;
        outputs.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        outputs.Controls.Add(outputsFlow);
        outputs.Identifier="outputs";
        outputs.Location=new Point(3, 237);
        outputs.Name="outputs";
        outputs.Size=new Size(521, 53);
        outputs.TabIndex=5;
        outputs.TabStop=false;
        outputs.Text="Output";
        // 
        // outputsFlow
        // 
        outputsFlow.AutoSize=true;
        outputsFlow.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        outputsFlow.Controls.Add(output1);
        outputsFlow.Controls.Add(output2);
        outputsFlow.Controls.Add(output3);
        outputsFlow.Controls.Add(output4);
        outputsFlow.Controls.Add(output5);
        outputsFlow.Dock=DockStyle.Fill;
        outputsFlow.Location=new Point(3, 19);
        outputsFlow.Name="outputsFlow";
        outputsFlow.Size=new Size(515, 31);
        outputsFlow.TabIndex=15;
        // 
        // output1
        // 
        output1.Identifier="output1";
        output1.Location=new Point(3, 3);
        output1.MaximumSize=new Size(175, 25);
        output1.MinimumSize=new Size(75, 25);
        output1.Name="output1";
        output1.ReadOnly=true;
        output1.Size=new Size(97, 25);
        output1.TabIndex=10;
        // 
        // output2
        // 
        output2.Identifier="output2";
        output2.Location=new Point(106, 3);
        output2.MaximumSize=new Size(175, 25);
        output2.MinimumSize=new Size(75, 25);
        output2.Name="output2";
        output2.ReadOnly=true;
        output2.Size=new Size(97, 25);
        output2.TabIndex=11;
        // 
        // output3
        // 
        output3.Identifier="output3";
        output3.Location=new Point(209, 3);
        output3.MaximumSize=new Size(175, 25);
        output3.MinimumSize=new Size(75, 25);
        output3.Name="output3";
        output3.ReadOnly=true;
        output3.Size=new Size(97, 25);
        output3.TabIndex=12;
        // 
        // output4
        // 
        output4.Identifier="output4";
        output4.Location=new Point(312, 3);
        output4.MaximumSize=new Size(175, 25);
        output4.MinimumSize=new Size(75, 25);
        output4.Name="output4";
        output4.ReadOnly=true;
        output4.Size=new Size(97, 25);
        output4.TabIndex=13;
        // 
        // output5
        // 
        output5.Identifier="output5";
        output5.Location=new Point(415, 3);
        output5.MaximumSize=new Size(175, 25);
        output5.MinimumSize=new Size(75, 25);
        output5.Name="output5";
        output5.ReadOnly=true;
        output5.Size=new Size(97, 25);
        output5.TabIndex=14;
        // 
        // splitContainer
        // 
        splitContainer.Dock=DockStyle.Fill;
        splitContainer.Location=new Point(0, 0);
        splitContainer.Name="splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(mainFlow);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(setupGroup);
        splitContainer.Panel2.Controls.Add(runGroup);
        splitContainer.Size=new Size(1000, 700);
        splitContainer.SplitterDistance=486;
        splitContainer.TabIndex=6;
        // 
        // mainFlow
        // 
        mainFlow.Controls.Add(inputTriggers);
        mainFlow.Controls.Add(inputInts);
        mainFlow.Controls.Add(inputStrings);
        mainFlow.Controls.Add(inputBools);
        mainFlow.Controls.Add(outputs);
        mainFlow.Dock=DockStyle.Fill;
        mainFlow.FlowDirection=FlowDirection.TopDown;
        mainFlow.Location=new Point(0, 0);
        mainFlow.Name="mainFlow";
        mainFlow.Size=new Size(486, 700);
        mainFlow.TabIndex=6;
        // 
        // setupGroup
        // 
        setupGroup.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        setupGroup.Controls.Add(presets);
        setupGroup.Controls.Add(rebuildButton);
        setupGroup.Controls.Add(codePanel);
        setupGroup.Location=new Point(3, 59);
        setupGroup.Name="setupGroup";
        setupGroup.Size=new Size(504, 641);
        setupGroup.TabIndex=5;
        setupGroup.TabStop=false;
        setupGroup.Text="Setup Script";
        // 
        // presets
        // 
        presets.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        presets.DropDownStyle=ComboBoxStyle.DropDownList;
        presets.FormattingEnabled=true;
        presets.Location=new Point(6, 22);
        presets.Name="presets";
        presets.Size=new Size(411, 23);
        presets.TabIndex=1;
        presets.SelectedIndexChanged+=this.presets_SelectedIndexChanged;
        // 
        // rebuildButton
        // 
        rebuildButton.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        rebuildButton.Location=new Point(423, 22);
        rebuildButton.Name="rebuildButton";
        rebuildButton.Size=new Size(75, 23);
        rebuildButton.TabIndex=0;
        rebuildButton.Text="Rebuild";
        rebuildButton.UseVisualStyleBackColor=true;
        rebuildButton.Click+=this.rebuildButton_Click;
        // 
        // codePanel
        // 
        codePanel.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        codePanel.Controls.Add(codeInput);
        codePanel.Controls.Add(errorBox);
        codePanel.Location=new Point(6, 51);
        codePanel.Name="codePanel";
        codePanel.Size=new Size(492, 584);
        codePanel.TabIndex=3;
        // 
        // codeInput
        // 
        codeInput.Dock=DockStyle.Fill;
        codeInput.Font=new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point);
        codeInput.Location=new Point(0, 0);
        codeInput.Multiline=true;
        codeInput.Name="codeInput";
        codeInput.ScrollBars=ScrollBars.Both;
        codeInput.Size=new Size(492, 394);
        codeInput.TabIndex=2;
        codeInput.WordWrap=false;
        codeInput.TextChanged+=this.codeInput_TextChanged;
        // 
        // errorBox
        // 
        errorBox.BackColor=Color.FromArgb(255, 192, 192);
        errorBox.Dock=DockStyle.Bottom;
        errorBox.Font=new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point);
        errorBox.ForeColor=Color.Maroon;
        errorBox.Location=new Point(0, 394);
        errorBox.Multiline=true;
        errorBox.Name="errorBox";
        errorBox.ReadOnly=true;
        errorBox.ScrollBars=ScrollBars.Both;
        errorBox.Size=new Size(492, 190);
        errorBox.TabIndex=3;
        // 
        // runGroup
        // 
        runGroup.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        runGroup.Controls.Add(quickCommand);
        runGroup.Controls.Add(runButton);
        runGroup.Location=new Point(3, 3);
        runGroup.Name="runGroup";
        runGroup.Size=new Size(504, 53);
        runGroup.TabIndex=4;
        runGroup.TabStop=false;
        runGroup.Text="Quick Command";
        // 
        // quickCommand
        // 
        quickCommand.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        quickCommand.Location=new Point(6, 22);
        quickCommand.Name="quickCommand";
        quickCommand.Size=new Size(411, 23);
        quickCommand.TabIndex=1;
        // 
        // runButton
        // 
        runButton.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        runButton.Location=new Point(423, 22);
        runButton.Name="runButton";
        runButton.Size=new Size(75, 23);
        runButton.TabIndex=0;
        runButton.Text="Run";
        runButton.UseVisualStyleBackColor=true;
        runButton.Click+=this.runButton_Click;
        // 
        // PushBoard
        // 
        this.AutoScaleDimensions=new SizeF(7F, 15F);
        this.AutoScaleMode=AutoScaleMode.Font;
        this.Controls.Add(splitContainer);
        this.Name="PushBoard";
        this.Size=new Size(1000, 700);
        this.Tag="Push Board";
        inputTriggers.ResumeLayout(false);
        inputTriggers.PerformLayout();
        inputTriggersFlow.ResumeLayout(false);
        inputBools.ResumeLayout(false);
        inputBools.PerformLayout();
        inputBoolsFlow.ResumeLayout(false);
        inputInts.ResumeLayout(false);
        inputInts.PerformLayout();
        inputIntsFlow.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)int1).EndInit();
        ((System.ComponentModel.ISupportInitialize)int2).EndInit();
        ((System.ComponentModel.ISupportInitialize)int3).EndInit();
        ((System.ComponentModel.ISupportInitialize)int4).EndInit();
        ((System.ComponentModel.ISupportInitialize)int5).EndInit();
        inputStrings.ResumeLayout(false);
        inputStrings.PerformLayout();
        inputStringsFlow.ResumeLayout(false);
        inputStringsFlow.PerformLayout();
        outputs.ResumeLayout(false);
        outputs.PerformLayout();
        outputsFlow.ResumeLayout(false);
        outputsFlow.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        mainFlow.ResumeLayout(false);
        mainFlow.PerformLayout();
        setupGroup.ResumeLayout(false);
        codePanel.ResumeLayout(false);
        codePanel.PerformLayout();
        runGroup.ResumeLayout(false);
        runGroup.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private BlackBoardGroupBox inputTriggers;
    private BlackBoardButton trigger1;
    private BlackBoardButton trigger2;
    private BlackBoardButton trigger3;
    private BlackBoardButton trigger4;
    private BlackBoardButton trigger5;
    private BlackBoardGroupBox inputBools;
    private BlackBoardCheckBox bool1;
    private BlackBoardCheckBox bool3;
    private BlackBoardCheckBox bool2;
    private BlackBoardCheckBox bool5;
    private BlackBoardCheckBox bool4;
    private BlackBoardGroupBox inputInts;
    private BlackBoardIntUpDown int1;
    private BlackBoardIntUpDown int2;
    private BlackBoardIntUpDown int3;
    private BlackBoardIntUpDown int4;
    private BlackBoardIntUpDown int5;
    private BlackBoardGroupBox inputStrings;
    private BlackBoardTextBox string5;
    private BlackBoardTextBox string4;
    private BlackBoardTextBox string3;
    private BlackBoardTextBox string2;
    private BlackBoardTextBox string1;
    private BlackBoardGroupBox outputs;
    private BlackBoardTextBox output5;
    private BlackBoardTextBox output4;
    private BlackBoardTextBox output3;
    private BlackBoardTextBox output2;
    private BlackBoardTextBox output1;
    private SplitContainer splitContainer;
    private ComboBox presets;
    private Button rebuildButton;
    private TextBox codeInput;
    private Panel codePanel;
    private TextBox errorBox;
    private FlowLayoutPanel inputTriggersFlow;
    private FlowLayoutPanel inputBoolsFlow;
    private FlowLayoutPanel inputIntsFlow;
    private FlowLayoutPanel inputStringsFlow;
    private FlowLayoutPanel outputsFlow;
    private FlowLayoutPanel mainFlow;
    private GroupBox runGroup;
    private Button runButton;
    private TextBox quickCommand;
    private GroupBox setupGroup;
}
