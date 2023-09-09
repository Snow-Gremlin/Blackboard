using System.Text;

namespace BlackboardExamples;

public partial class MainForm : Form {

    public MainForm() {
        this.InitializeComponent();

        this.addExample(new Examples.PushBoard.PushBoard());

        for (int i = 0; i < 1000; i++)
            Console.WriteLine(i+". Hello World");
    }

    private void addExample(UserControl ctrl) {
        TabPage page = new();
        page.Controls.Add(ctrl);
        page.Text = ctrl.Tag?.ToString() ?? ctrl.Name;
        ctrl.Dock = DockStyle.Fill;
        this.tabControl.TabPages.Add(page);
    }
}
