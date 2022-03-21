using System.Collections.Generic;
using System.Text.RegularExpressions;
using S = System;

namespace BlackboardTools {

    /// <summary>The collection of commands and methods to run those commands.</summary>
    internal class CommandHandler {
        private Dictionary<string, ICommand> handlers;

        /// <summary>Creates a new command handler.</summary>
        /// <param name="commands">All the commands to handle.</param>
        public CommandHandler(params ICommand[] commands) {
            this.handlers = new Dictionary<string, ICommand>();
            Regex regex = new("\\S+", RegexOptions.Compiled);
            foreach (ICommand command in commands) {
                string name = command.Name;
                if (!regex.IsMatch(name))
                    throw new S.Exception("Command has an invalid name: "+name);
                this.handlers.Add(name, command);
            }
        }

        /// <summary>Gets the set of commands listed by the command name.</summary>
        public IReadOnlyDictionary<string, ICommand> Commands => this.handlers;

        /// <summary>Runs a the given command.</summary>
        /// <param name="command">The command to run.</param>
        public void Run(string command) {
            if (string.IsNullOrWhiteSpace(command)) return;
            try {
                CommandArgs args = new(this, command);
                if (this.handlers.TryGetValue(args.Name, out ICommand handler))
                    handler.Run(args);
                S.Console.WriteLine("No command found with the name "+args.Name+".");
            } catch (CommandException e) {
                S.Console.WriteLine(e.Message);
            } catch (S.Exception e) {
                S.Console.WriteLine(e);
            }
        }
    }
}
