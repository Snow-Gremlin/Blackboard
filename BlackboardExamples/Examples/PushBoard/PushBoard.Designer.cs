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
        trigger5=new BlackBoardButton();
        trigger4=new BlackBoardButton();
        trigger3=new BlackBoardButton();
        trigger2=new BlackBoardButton();
        trigger1=new BlackBoardButton();
        inputBools=new BlackBoardGroupBox();
        bool5=new BlackBoardCheckBox();
        bool4=new BlackBoardCheckBox();
        bool3=new BlackBoardCheckBox();
        bool2=new BlackBoardCheckBox();
        bool1=new BlackBoardCheckBox();
        inputInts=new BlackBoardGroupBox();
        int5=new BlackBoardIntUpDown();
        int4=new BlackBoardIntUpDown();
        int3=new BlackBoardIntUpDown();
        int2=new BlackBoardIntUpDown();
        int1=new BlackBoardIntUpDown();
        inputStrings=new BlackBoardGroupBox();
        string5=new BlackBoardTextBox();
        string4=new BlackBoardTextBox();
        string3=new BlackBoardTextBox();
        string2=new BlackBoardTextBox();
        string1=new BlackBoardTextBox();
        output=new BlackBoardGroupBox();
        output6=new BlackBoardTextBox();
        output5=new BlackBoardTextBox();
        output4=new BlackBoardTextBox();
        output3=new BlackBoardTextBox();
        output2=new BlackBoardTextBox();
        output1=new BlackBoardTextBox();
        splitContainer=new SplitContainer();
        panel1=new Panel();
        codeInput=new TextBox();
        errorBox=new TextBox();
        presets=new ComboBox();
        rebuildButton=new Button();
        inputTriggers.SuspendLayout();
        inputBools.SuspendLayout();
        inputInts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)int5).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)int1).BeginInit();
        inputStrings.SuspendLayout();
        output.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        panel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // inputTriggers
        // 
        inputTriggers.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        inputTriggers.Controls.Add(trigger5);
        inputTriggers.Controls.Add(trigger4);
        inputTriggers.Controls.Add(trigger3);
        inputTriggers.Controls.Add(trigger2);
        inputTriggers.Controls.Add(trigger1);
        inputTriggers.Location=new Point(3, 3);
        inputTriggers.Name="inputTriggers";
        inputTriggers.Size=new Size(527, 65);
        inputTriggers.TabIndex=1;
        inputTriggers.TabStop=false;
        inputTriggers.Text="Input Triggers";
        // 
        // trigger5
        // 
        trigger5.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger5.Identifier="trigger5";
        trigger5.Location=new Point(418, 22);
        trigger5.MaximumSize=new Size(125, 25);
        trigger5.MinimumSize=new Size(75, 25);
        trigger5.Name="trigger5";
        trigger5.Size=new Size(97, 25);
        trigger5.TabIndex=4;
        trigger5.Text="trigger 5";
        trigger5.UseVisualStyleBackColor=true;
        // 
        // trigger4
        // 
        trigger4.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger4.Identifier="trigger4";
        trigger4.Location=new Point(315, 22);
        trigger4.MaximumSize=new Size(125, 25);
        trigger4.MinimumSize=new Size(75, 25);
        trigger4.Name="trigger4";
        trigger4.Size=new Size(97, 25);
        trigger4.TabIndex=3;
        trigger4.Text="trigger 4";
        trigger4.UseVisualStyleBackColor=true;
        // 
        // trigger3
        // 
        trigger3.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger3.Identifier="trigger3";
        trigger3.Location=new Point(212, 22);
        trigger3.MaximumSize=new Size(125, 25);
        trigger3.MinimumSize=new Size(75, 25);
        trigger3.Name="trigger3";
        trigger3.Size=new Size(97, 25);
        trigger3.TabIndex=2;
        trigger3.Text="trigger 3";
        trigger3.UseVisualStyleBackColor=true;
        // 
        // trigger2
        // 
        trigger2.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger2.Identifier="trigger2";
        trigger2.Location=new Point(109, 22);
        trigger2.MaximumSize=new Size(125, 25);
        trigger2.MinimumSize=new Size(75, 25);
        trigger2.Name="trigger2";
        trigger2.Size=new Size(97, 25);
        trigger2.TabIndex=1;
        trigger2.Text="trigger 2";
        trigger2.UseVisualStyleBackColor=true;
        // 
        // trigger1
        // 
        trigger1.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        trigger1.Identifier="trigger1";
        trigger1.Location=new Point(6, 22);
        trigger1.MaximumSize=new Size(125, 25);
        trigger1.MinimumSize=new Size(75, 25);
        trigger1.Name="trigger1";
        trigger1.Size=new Size(97, 25);
        trigger1.TabIndex=0;
        trigger1.Text="trigger 1";
        trigger1.UseVisualStyleBackColor=true;
        // 
        // inputBools
        // 
        inputBools.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        inputBools.Controls.Add(bool5);
        inputBools.Controls.Add(bool4);
        inputBools.Controls.Add(bool3);
        inputBools.Controls.Add(bool2);
        inputBools.Controls.Add(bool1);
        inputBools.Location=new Point(3, 74);
        inputBools.Name="inputBools";
        inputBools.Size=new Size(527, 65);
        inputBools.TabIndex=2;
        inputBools.TabStop=false;
        inputBools.Text="Input Booleans";
        // 
        // bool5
        // 
        bool5.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool5.Identifier="bool5";
        bool5.Location=new Point(418, 22);
        bool5.Name="bool5";
        bool5.Size=new Size(70, 35);
        bool5.TabIndex=4;
        bool5.Text="bool 5";
        bool5.UseVisualStyleBackColor=true;
        // 
        // bool4
        // 
        bool4.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool4.Identifier="bool4";
        bool4.Location=new Point(315, 22);
        bool4.Name="bool4";
        bool4.Size=new Size(70, 35);
        bool4.TabIndex=3;
        bool4.Text="bool 4";
        bool4.UseVisualStyleBackColor=true;
        // 
        // bool3
        // 
        bool3.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool3.Identifier="bool3";
        bool3.Location=new Point(212, 22);
        bool3.Name="bool3";
        bool3.Size=new Size(70, 35);
        bool3.TabIndex=2;
        bool3.Text="bool 3";
        bool3.UseVisualStyleBackColor=true;
        // 
        // bool2
        // 
        bool2.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool2.Identifier="bool2";
        bool2.Location=new Point(109, 22);
        bool2.Name="bool2";
        bool2.Size=new Size(70, 35);
        bool2.TabIndex=1;
        bool2.Text="bool 2";
        bool2.UseVisualStyleBackColor=true;
        // 
        // bool1
        // 
        bool1.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;
        bool1.Identifier="bool1";
        bool1.Location=new Point(21, 22);
        bool1.Name="bool1";
        bool1.Size=new Size(70, 35);
        bool1.TabIndex=0;
        bool1.Text="bool 1";
        bool1.UseVisualStyleBackColor=true;
        // 
        // inputInts
        // 
        inputInts.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        inputInts.Controls.Add(int5);
        inputInts.Controls.Add(int4);
        inputInts.Controls.Add(int3);
        inputInts.Controls.Add(int2);
        inputInts.Controls.Add(int1);
        inputInts.Location=new Point(5, 145);
        inputInts.Name="inputInts";
        inputInts.Size=new Size(527, 65);
        inputInts.TabIndex=3;
        inputInts.TabStop=false;
        inputInts.Text="Input Integers";
        // 
        // int5
        // 
        int5.Identifier="int5";
        int5.Location=new Point(416, 25);
        int5.Name="int5";
        int5.Size=new Size(97, 23);
        int5.TabIndex=4;
        // 
        // int4
        // 
        int4.Identifier="int4";
        int4.Location=new Point(313, 25);
        int4.Name="int4";
        int4.Size=new Size(97, 23);
        int4.TabIndex=3;
        // 
        // int3
        // 
        int3.Identifier="int3";
        int3.Location=new Point(210, 25);
        int3.Name="int3";
        int3.Size=new Size(97, 23);
        int3.TabIndex=2;
        // 
        // int2
        // 
        int2.Identifier="int2";
        int2.Location=new Point(107, 25);
        int2.Name="int2";
        int2.Size=new Size(97, 23);
        int2.TabIndex=1;
        // 
        // int1
        // 
        int1.Identifier="int1";
        int1.Location=new Point(4, 25);
        int1.Name="int1";
        int1.Size=new Size(97, 23);
        int1.TabIndex=0;
        // 
        // inputStrings
        // 
        inputStrings.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        inputStrings.Controls.Add(string5);
        inputStrings.Controls.Add(string4);
        inputStrings.Controls.Add(string3);
        inputStrings.Controls.Add(string2);
        inputStrings.Controls.Add(string1);
        inputStrings.Location=new Point(5, 216);
        inputStrings.Name="inputStrings";
        inputStrings.Size=new Size(527, 65);
        inputStrings.TabIndex=4;
        inputStrings.TabStop=false;
        inputStrings.Text="Input Strings";
        // 
        // string5
        // 
        string5.Identifier="string5";
        string5.Location=new Point(416, 25);
        string5.Name="string5";
        string5.Size=new Size(97, 23);
        string5.TabIndex=9;
        // 
        // string4
        // 
        string4.Identifier="string4";
        string4.Location=new Point(313, 25);
        string4.Name="string4";
        string4.Size=new Size(97, 23);
        string4.TabIndex=8;
        // 
        // string3
        // 
        string3.Identifier="string3";
        string3.Location=new Point(210, 25);
        string3.Name="string3";
        string3.Size=new Size(97, 23);
        string3.TabIndex=7;
        // 
        // string2
        // 
        string2.Identifier="string2";
        string2.Location=new Point(107, 25);
        string2.Name="string2";
        string2.Size=new Size(97, 23);
        string2.TabIndex=6;
        // 
        // string1
        // 
        string1.Identifier="string1";
        string1.Location=new Point(4, 25);
        string1.Name="string1";
        string1.Size=new Size(97, 23);
        string1.TabIndex=5;
        // 
        // output
        // 
        output.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        output.Controls.Add(output6);
        output.Controls.Add(output5);
        output.Controls.Add(output4);
        output.Controls.Add(output3);
        output.Controls.Add(output2);
        output.Controls.Add(output1);
        output.Location=new Point(3, 287);
        output.Name="output";
        output.Size=new Size(527, 410);
        output.TabIndex=5;
        output.TabStop=false;
        output.Text="Output";
        // 
        // output6
        // 
        output6.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        output6.Identifier="output6";
        output6.Location=new Point(6, 54);
        output6.Multiline=true;
        output6.Name="output6";
        output6.ReadOnly=true;
        output6.ScrollBars=ScrollBars.Both;
        output6.Size=new Size(513, 350);
        output6.TabIndex=15;
        // 
        // output5
        // 
        output5.Identifier="output5";
        output5.Location=new Point(418, 25);
        output5.Name="output5";
        output5.ReadOnly=true;
        output5.Size=new Size(97, 23);
        output5.TabIndex=14;
        // 
        // output4
        // 
        output4.Identifier="output4";
        output4.Location=new Point(315, 25);
        output4.Name="output4";
        output4.ReadOnly=true;
        output4.Size=new Size(97, 23);
        output4.TabIndex=13;
        // 
        // output3
        // 
        output3.Identifier="output3";
        output3.Location=new Point(212, 25);
        output3.Name="output3";
        output3.ReadOnly=true;
        output3.Size=new Size(97, 23);
        output3.TabIndex=12;
        // 
        // output2
        // 
        output2.Identifier="output2";
        output2.Location=new Point(109, 25);
        output2.Name="output2";
        output2.ReadOnly=true;
        output2.Size=new Size(97, 23);
        output2.TabIndex=11;
        // 
        // output1
        // 
        output1.Identifier="output1";
        output1.Location=new Point(6, 25);
        output1.Name="output1";
        output1.ReadOnly=true;
        output1.Size=new Size(97, 23);
        output1.TabIndex=10;
        // 
        // splitContainer
        // 
        splitContainer.Dock=DockStyle.Fill;
        splitContainer.Location=new Point(0, 0);
        splitContainer.Name="splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(inputTriggers);
        splitContainer.Panel1.Controls.Add(output);
        splitContainer.Panel1.Controls.Add(inputBools);
        splitContainer.Panel1.Controls.Add(inputStrings);
        splitContainer.Panel1.Controls.Add(inputInts);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(panel1);
        splitContainer.Panel2.Controls.Add(presets);
        splitContainer.Panel2.Controls.Add(rebuildButton);
        splitContainer.Size=new Size(1000, 700);
        splitContainer.SplitterDistance=535;
        splitContainer.TabIndex=6;
        // 
        // panel1
        // 
        panel1.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;
        panel1.Controls.Add(codeInput);
        panel1.Controls.Add(errorBox);
        panel1.Location=new Point(3, 39);
        panel1.Name="panel1";
        panel1.Size=new Size(455, 652);
        panel1.TabIndex=3;
        // 
        // codeInput
        // 
        codeInput.Dock=DockStyle.Fill;
        codeInput.Location=new Point(0, 0);
        codeInput.Multiline=true;
        codeInput.Name="codeInput";
        codeInput.ScrollBars=ScrollBars.Both;
        codeInput.Size=new Size(455, 462);
        codeInput.TabIndex=2;
        codeInput.WordWrap=false;
        codeInput.TextChanged+=this.codeInput_TextChanged;
        // 
        // errorBox
        // 
        errorBox.BackColor=Color.FromArgb(255, 192, 192);
        errorBox.Dock=DockStyle.Bottom;
        errorBox.ForeColor=Color.Maroon;
        errorBox.Location=new Point(0, 462);
        errorBox.Multiline=true;
        errorBox.Name="errorBox";
        errorBox.ReadOnly=true;
        errorBox.ScrollBars=ScrollBars.Both;
        errorBox.Size=new Size(455, 190);
        errorBox.TabIndex=3;
        errorBox.WordWrap=false;
        // 
        // presets
        // 
        presets.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        presets.DropDownStyle=ComboBoxStyle.DropDownList;
        presets.FormattingEnabled=true;
        presets.Location=new Point(3, 10);
        presets.Name="presets";
        presets.Size=new Size(352, 23);
        presets.TabIndex=1;
        presets.SelectedIndexChanged+=this.presets_SelectedIndexChanged;
        // 
        // rebuildButton
        // 
        rebuildButton.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        rebuildButton.Location=new Point(361, 3);
        rebuildButton.Name="rebuildButton";
        rebuildButton.Size=new Size(97, 35);
        rebuildButton.TabIndex=0;
        rebuildButton.Text="Rebuild";
        rebuildButton.UseVisualStyleBackColor=true;
        rebuildButton.Click+=this.rebuildButton_Click;
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
        inputBools.ResumeLayout(false);
        inputInts.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)int5).EndInit();
        ((System.ComponentModel.ISupportInitialize)int4).EndInit();
        ((System.ComponentModel.ISupportInitialize)int3).EndInit();
        ((System.ComponentModel.ISupportInitialize)int2).EndInit();
        ((System.ComponentModel.ISupportInitialize)int1).EndInit();
        inputStrings.ResumeLayout(false);
        inputStrings.PerformLayout();
        output.ResumeLayout(false);
        output.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
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
    private BlackBoardGroupBox output;
    private BlackBoardTextBox output5;
    private BlackBoardTextBox output4;
    private BlackBoardTextBox output3;
    private BlackBoardTextBox output2;
    private BlackBoardTextBox output1;
    private BlackBoardTextBox output6;
    private SplitContainer splitContainer;
    private ComboBox presets;
    private Button rebuildButton;
    private TextBox codeInput;
    private Panel panel1;
    private TextBox errorBox;
}
