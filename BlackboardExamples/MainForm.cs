namespace BlackboardExamples;

public partial class MainForm : Form {

    public MainForm() {
        this.InitializeComponent();

        this.addExample(new Examples.PushBoard.PushBoard());
    }

    private void addExample(UserControl ctrl) {
        TabPage page = new();
        page.Controls.Add(ctrl);
        page.Text = ctrl.Tag?.ToString() ?? ctrl.Name;
        ctrl.Dock = DockStyle.Fill;
        this.tabControl.TabPages.Add(page);
    }
}
