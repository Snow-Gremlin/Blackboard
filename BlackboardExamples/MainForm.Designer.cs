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
        this.SuspendLayout();
        // 
        // tabControl
        // 
        tabControl.Dock=DockStyle.Fill;
        tabControl.Location=new Point(0, 0);
        tabControl.Name="tabControl";
        tabControl.SelectedIndex=0;
        tabControl.Size=new Size(1184, 961);
        tabControl.TabIndex=0;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions=new SizeF(7F, 15F);
        this.AutoScaleMode=AutoScaleMode.Font;
        this.ClientSize=new Size(1184, 961);
        this.Controls.Add(tabControl);
        this.Name="MainForm";
        this.Text="Blackboard Examples";
        this.ResumeLayout(false);
    }

    #endregion

    private TabControl tabControl;
}
