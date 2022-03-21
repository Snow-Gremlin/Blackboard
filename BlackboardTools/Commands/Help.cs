namespace BlackboardTools.Commands {
    internal class Help: ICommand {
        public string Name => "help";
        public string ShortDescription => "Displays this help text. Use \"help <command>\" for more information about the given command.";
        public string FullDescription => "You found me!";
        public void Run(CommandArgs args) {
            args.RequiresMaximum(1);
            if (args.Arguments.Length == 1) {




            }

            // TODO: Output help

        }
    }
}
