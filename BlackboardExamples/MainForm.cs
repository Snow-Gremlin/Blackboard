namespace BlackboardExamples;

/// <summary>The main form for displaying Blackboard examples.</summary>
public partial class MainForm : Form {

    /// <summary>Creates a new Blackboard example.</summary>
    public MainForm() {
        this.InitializeComponent();

        this.addExample(new Examples.PushBoard.PushBoard());
    }

    /// <summary>Adds a new example to the tabs of examples.</summary>
    /// <param name="ctrl">The control for the example to add.</param>
    private void addExample(Control ctrl) {
        TabPage page = new();
        page.Controls.Add(ctrl);
        page.Text = ctrl.Tag?.ToString() ?? ctrl.Name;
        ctrl.Dock = DockStyle.Fill;
        this.tabControl.TabPages.Add(page);
    }
}
