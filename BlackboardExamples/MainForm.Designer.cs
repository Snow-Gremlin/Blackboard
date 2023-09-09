namespace BlackboardExamples;

partial class MainForm {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        tabControl=new TabControl();
        console=new ConsolePanel();
        mainSplit=new SplitContainer();
        ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
        mainSplit.Panel1.SuspendLayout();
        mainSplit.Panel2.SuspendLayout();
        mainSplit.SuspendLayout();
        this.SuspendLayout();
        // 
        // tabControl
        // 
        tabControl.Dock=DockStyle.Fill;
        tabControl.Location=new Point(0, 0);
        tabControl.Name="tabControl";
        tabControl.SelectedIndex=0;
        tabControl.Size=new Size(1184, 657);
        tabControl.TabIndex=0;
        // 
        // console
        // 
        console.Dock=DockStyle.Fill;
        console.Location=new Point(0, 0);
        console.Name="console";
        console.Size=new Size(1184, 300);
        console.TabIndex=1;
        // 
        // mainSplit
        // 
        mainSplit.Dock=DockStyle.Fill;
        mainSplit.Location=new Point(0, 0);
        mainSplit.Name="mainSplit";
        mainSplit.Orientation=Orientation.Horizontal;
        // 
        // mainSplit.Panel1
        // 
        mainSplit.Panel1.Controls.Add(tabControl);
        // 
        // mainSplit.Panel2
        // 
        mainSplit.Panel2.Controls.Add(console);
        mainSplit.Size=new Size(1184, 961);
        mainSplit.SplitterDistance=657;
        mainSplit.TabIndex=2;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions=new SizeF(7F, 15F);
        this.AutoScaleMode=AutoScaleMode.Font;
        this.ClientSize=new Size(1184, 961);
        this.Controls.Add(mainSplit);
        this.Name="MainForm";
        this.Text="Blackboard Examples";
        mainSplit.Panel1.ResumeLayout(false);
        mainSplit.Panel2.ResumeLayout(false);
        mainSplit.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
        mainSplit.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private TabControl tabControl;
    private ConsolePanel console;
    private SplitContainer mainSplit;
}
