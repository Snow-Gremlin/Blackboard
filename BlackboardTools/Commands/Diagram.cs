using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S = System;

namespace BlackboardTools.Commands {
    internal class Diagram: ICommand {

        public string Name => "diagram";

        public string ShortDescription => "Creates a UML diagram for the classes in a given namespace.";

        public string FullDescription => "Creates a UML (mermaid) diagram for classes in a given namespace. "+
            "Example \"diagram Blackboard.Core.Nodes\". The resulting diagram commands is printed to the console. "+
            "The internal namespaces to the one which is given are also added to the diagram. "+
            "This is useful to update the diagrams in the README.md files.";

        private class Node {
            readonly public S.Type Type;
            readonly public string Name;
            public Node(S.Type type, int index) {
                this.Type = type;
                this.Name = "Node"+index;
            }
        }

        public void Run(CommandArgs args) {
            args.RequiresExactly(1);
            string nspace = args.Arguments[0];
            Dictionary<string, Node> allTypes = getAllTypes(nspace);
            Dictionary<string, List<Node>> grouped = groupTypes(allTypes);

            StringBuilder buf = new();
            buf.AppendLine("flowchart TB");
            int nspaceIndex = 0;
            foreach ((string group, List<Node> nodes) in grouped) {
                addGroup(buf, nspaceIndex, group, nodes, allTypes);
                nspaceIndex++;
            }
            S.Console.WriteLine(buf);
        }

        static private Dictionary<string, Node> getAllTypes(string nspace) =>
            S.AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).
            NotNull().Where(t => t.IsPublic).
            Where(t => t.IsClass || t.IsInterface || t.IsAbstract).
            Where(t => t.Namespace is not null && (t.Namespace == nspace || t.Namespace.StartsWith(nspace+"."))).
            WithIndex().Select(pair => new Node(pair.item, pair.index)).ToDictionary(n => n.Type.FullName);

        static private Dictionary<string, List<Node>> groupTypes(Dictionary<string, Node> allTypes) =>
            allTypes.Values.GroupBy(n => n.Type.Namespace).
            ToDictionary(g => g.Key, g => g.ToList());

        static private void addGroup(StringBuilder buf, int nspaceIndex, string group, List<Node> nodes, Dictionary<string, Node> allTypes) {
            buf.AppendLine("subgraph NSpace"+nspaceIndex+" ["+group+"]");
            buf.AppendLine();
            foreach (Node node in nodes) {
                addNode(buf, node, allTypes);
                buf.AppendLine();
            }
            buf.AppendLine("end");
        }

        static private void addNode(StringBuilder buf, Node node, Dictionary<string, Node> allTypes) {
            string name = node.Type.Name;
            int index = name.IndexOf('`');
            if (index > 0) name = name[..index];

            if (node.Type.ContainsGenericParameters) {
                buf.AppendLine(">"+node.Type.GetGenericArguments().Select(t => t.Name).Join(", "));
                //buf.AppendLine(">"+node.Type.GetGenericParameterConstraints().Select(t => t.Name).Join(", "));
            }

            if (node.Type.IsInterface)
                buf.AppendLine(node.Name+"[/"+name+"/]");
            else if (node.Type.IsAbstract)
                buf.AppendLine(node.Name+"[/"+name+"]");
            else buf.AppendLine(node.Name+"["+name+"]");

            // node.Type.GetInterfaces
        }
    }
}
