using S = System;

namespace BlackboardTools.Commands {

    /// <summary>A command for exiting the tools.</summary>
    internal class Exit: ICommand {
        public string Name => "exit";
        public string ShortDescription => "This will quit the Blackboard tools.";
        public string FullDescription => this.ShortDescription;
        public void Run(CommandArgs args) {
            args.RequiresExactly(0);
            S.Environment.Exit(0);
        }
    }
}
