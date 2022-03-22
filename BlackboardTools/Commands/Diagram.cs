using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace BlackboardTools.Commands {
    internal class Diagram: ICommand {

        public string Name => "diagram";

        public string ShortDescription => "Creates a UML diagram for the classes in a given namespace.";

        public string FullDescription => "Creates a UML (mermaid) diagram for classes in a given namespace. "+
            "Example \"diagram Blackboard.Core.Nodes\". The resulting diagram commands is printed to the console. "+
            "The internal namespaces to the one which is given are also added to the diagram. "+
            "This is useful to update the diagrams in the README.md files.";

        public void Run(CommandArgs args) {
            args.RequiresExactly(1);

            string nspace = args.Arguments[0];
            List<S.Type> types = S.AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).
                NotNull().Where(t => t.IsPublic).
                Where(t => t.IsClass || t.IsInterface || t.IsAbstract).
                Where(t => t.Namespace is not null && (t.Namespace == nspace || t.Namespace.StartsWith(nspace+"."))).
                ToList();

            foreach (S.Type type in types) {
                // TODO: Finish
                S.Console.WriteLine(type);
            }


        }
    }
}
