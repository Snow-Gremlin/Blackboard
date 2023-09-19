namespace BlackboardExamples;

internal static class Program {

    /// <summary>The main entry point for the Blackboard example application.</summary>
    [STAThread]
    static void Main() {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}