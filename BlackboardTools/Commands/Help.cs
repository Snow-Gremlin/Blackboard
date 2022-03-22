using System.Collections.Generic;
using System.Linq;
using S = System;

namespace BlackboardTools.Commands {
    internal class Help: ICommand {
        
        public string Name => "help";
        
        public string ShortDescription => "Displays this help text. Use \"help <command>\" for more information about the given command.";
        
        public string FullDescription => "You found me!";

        public void Run(CommandArgs args) {
            args.RequiresMaximum(1);
            if (args.Arguments.Length == 1)
                showOne(args.Handler.Commands, args.Arguments[0]);
            else showAll(args.Handler.Commands);
        }

        static private void showOne(IReadOnlyDictionary<string, ICommand> commands, string name) {
            if (commands.TryGetValue(name, out ICommand command))
                S.Console.WriteLine(command.FullDescription);
            else S.Console.WriteLine("No command found with the name "+name+".");
        }

        static private void showAll(IReadOnlyDictionary<string, ICommand> commands) {
            const string separator = "  ";
            List<string> names = new(commands.Keys);
            names.Sort();
            int maxWidth = names.Select(name => name.Length).Max();
            int descWidth = S.Console.WindowWidth - maxWidth - separator.Length - 1;
            foreach (string name in names) {
                string desc = commands[name].ShortDescription;
                string[] lines = wrapText(desc, descWidth).ToArray();
                S.Console.WriteLine(name.PadRight(maxWidth)+separator+lines[0]);
                string indent = "".PadRight(maxWidth);
                foreach (string line in lines[1..])
                    S.Console.WriteLine(indent+separator+line);
            }
        }

        static private List<string> wrapText(string text, int width) {
            string[] words = text.Split(' ');
            if (words.Length < 0) return new() { "" };
            List<string> lines = new() { words[0] };
            foreach (string word in words[1..]) {
                string current = lines[^1];
                if (current.Length + 1 + word.Length <= width)
                    lines[^1] = current+" "+word;
                else lines.Add(word);
            }
            return lines;
        }
    }
}
