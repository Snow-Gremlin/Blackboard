using System.Collections.Generic;
using System.Text.RegularExpressions;
using S = System;

namespace BlackboardTools {

    internal class CommandArgs {

        static private Regex regex = new("^\\s* (\\w+) (?:\\s+ (\\S+))* \\s*$",
            RegexOptions.Compiled|RegexOptions.IgnorePatternWhitespace);

        public readonly CommandHandler Handler;
        public readonly string Name;
        public readonly string Command;
        public readonly string[] Arguments;

        public CommandArgs(CommandHandler handler, string command) {
            this.Handler = handler;
            this.Command = command;
            Match match = regex.Match(command);

            if (!match.Success || match.Groups.Count != 3)
                throw new CommandException("The given command is invalid: "+command);

            this.Name = match.Groups[1].Value;
            List<string> args = new();
            foreach (Capture capture in match.Groups[2].Captures)
                args.Add(capture.Value);
            this.Arguments = args.ToArray();
        }

        public void RequiresExactly(int count) {
            if (this.Arguments.Length != count) {
                if (count == 0) throw new CommandException(this.Name+" may not have arguments.");
                if (count == 1) throw new CommandException(this.Name+" may only have 1 argument.");
                throw new CommandException(this.Name+"may only have "+count+" arguments.");
            }
        }

        public void RequiresMin(int min) {
            if (this.Arguments.Length < min) {
                if (min == 1) throw new CommandException(this.Name+" must have at least 1 argument.");
                throw new CommandException(this.Name+" must have at least "+min+" arguments.");
            }
        }

        public void RequiresMaximum(int max) {
            if (this.Arguments.Length > max) {
                if (max == 1) throw new CommandException(this.Name+" must have at most 1 argument.");
                throw new CommandException(this.Name+" must have at most "+max+" arguments.");
            }
        }

        public override string ToString() =>
            this.Name + "(" + string.Join(", ", this.Arguments) + ")";
    }
}
