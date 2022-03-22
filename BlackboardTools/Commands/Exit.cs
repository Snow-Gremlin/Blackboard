using S = System;

namespace BlackboardTools.Commands {
    internal class Exit: ICommand {
        
        public string Name => "exit";
        
        public string ShortDescription => "Quits the Blackboard tools.";
        
        public string FullDescription => this.ShortDescription;

        public void Run(CommandArgs args) {
            args.RequiresExactly(0);
            S.Environment.Exit(0);
        }
    }
}
