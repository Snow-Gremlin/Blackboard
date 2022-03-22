using BlackboardTools.Commands;
using S = System;

namespace BlackboardTools {

    /// <summary>The entry point for the Blackboard tools.</summary>
    static public class EntryPoint {

        /// <summary>Tuns the Blackboard tools.</summary>
        static public void Main() {
            CommandHandler handler = new(
                new Exit(),
                new Help(),
                new Diagram()
            );

            S.Console.WriteLine("Enter command for the tool to run (type \"help\" to show help, type \"exit\" to leave):");
            Input input = new();
            while (true) {
                string command = input.Read();
                handler.Run(command);
            }
        }
    }
}
