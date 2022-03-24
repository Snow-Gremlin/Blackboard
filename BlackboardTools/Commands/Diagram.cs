using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = System;

namespace BlackboardTools.Commands {
    internal class Diagram: ICommand {

        public string Name => "diagram";

        public string ShortDescription => "Creates a UML diagram for the classes in a given namespace.";

        public string FullDescription => "Creates a UML diagram for classes in a given namespace. "+
            "Example \"diagram Blackboard.Core.Nodes\". The resulting diagram commands is printed to the console. "+
            "The internal namespaces to the one which is given are also added to the diagram. "+
            "This is useful to update the diagrams in the README.md files. "+
            "NOTE: This doesn't work correctly yet.";

        private class Node {
            readonly public S.Type Type;
            readonly public string Label;
            public Node(S.Type type, int index) {
                this.Type = type;
                this.Label = "Node"+index;
            }
        }

        public void Run(CommandArgs args) {
            args.RequiresExactly(1);
            string nspace = args.Arguments[0];
            Dictionary<string, Node> allTypes = getAllTypes(nspace);

            StringBuilder buf = new();
            foreach (Node node in allTypes.Values)
                addClassDef(buf, node);
            foreach (Node node in allTypes.Values)
                addInheritance(buf, node, allTypes);
            S.Console.WriteLine(buf);
        }

        static private string getTypeKey(S.Type type) =>
            type.Namespace+"::"+type.Name;

        static private Dictionary<string, Node> getAllTypes(string nspace) =>
            S.AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).NotNull().
            Where(t => t.IsPublic).
            Where(t => t.IsClass || t.IsInterface || t.IsAbstract).
            Where(t => t.Namespace is not null && (t.Namespace == nspace || t.Namespace.StartsWith(nspace+"."))).
            WithIndex().Select(pair => new Node(pair.item, pair.index)).
            ToDictionary(n => getTypeKey(n.Type));

        static private string getSimpleTypeName(S.Type type) {
            string name = type.Name;
            int index = name.IndexOf('`');
            if (index > 0) name = name[..index];
            return name;
        }

        static private string getTypeName(S.Type type) {
            string name = getSimpleTypeName(type);

            if (type.ContainsGenericParameters) {
                List<string> gens = new();

                S.Type[] genTypes = type.GetGenericArguments();
                foreach (S.Type genType in genTypes) {
                    string gen = getSimpleTypeName(genType);
                    // Need to add Constraints?
                    gens.Add(gen);
                }

                name += "<"+gens.Join(", ")+">";
            }
            return name;
        }

        static private void addClassDef(StringBuilder buf, Node node) {
            string name = getTypeName(node.Type);
            string ctype =
                node.Type.IsInterface ? "interface" :
                node.Type.IsAbstract  ? "abstract"  :
                "class";

            buf.AppendLine(ctype+" \""+name+"\" as "+node.Label);
        }

        static private void addInheritance(StringBuilder buf, Node node, Dictionary<string, Node> allTypes) {
            foreach (S.Type inter in node.Type.GetInterfaces()) {
                if (allTypes.TryGetValue(getTypeKey(inter), out Node other))
                    buf.AppendLine(node.Label+" --> "+other.Label);
            }
        }
    }
}
