using Blackboard.Core.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core {
    static public class Stringifier {


        /// <summary>Gets a string for the given node even if the node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The string for the given node.</returns>
        static public string Simple(params INode[] nodes) =>
            Simple(nodes as IEnumerable<INode>);

        /// <summary>Gets a string got the given set of nodes comma separated.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The string for the given nodes.</returns>
        static public string Simple(IEnumerable<INode> nodes) =>
            nodes.Select(simpleNode).Join(", ");

        static private string simpleNode(INode node) =>
            node is null ? "null" : node.TypeName + head(node);

        /// <summary>Gets the data type and data value of the node without the node type.</summary>
        /// <param name="node">The node to get the head of.</param>
        /// <returns>The head string.</returns>
        static private string head(INode node) =>
            node switch {
                IFuncDef  => funcDefHead (node as IFuncDef ),
                IDataNode => iDataHead   (node as IDataNode),
                ITrigger  => iTriggerHead(node as ITrigger ),
                _         => "",
            };

        static private string funcDefHead(IFuncDef node) {
            string inputTypes = "";
            if (node.ArgumentTypes.Count > 0) {
                inputTypes = node.ArgumentTypes.Strings().Join(", ");
                if (node.LastArgVariable) inputTypes += "...";
                inputTypes += "|";
            }
            return "<"+inputTypes+Type.FromType(node.ReturnType) + ">";
        }

        static private string iDataHead(IDataNode node) =>
            "<" + node.Data.TypeName + ">[" + node.Data.ValueString + "]";

        static private string iTriggerHead(ITrigger node) =>
            "<trigger>" + (node.Provoked ? "[provoked]" : "");

        static public string Deep(INode node, bool showFuncs = true, int depth = int.MaxValue) =>
            Deep(new INode[] { node }, showFuncs, depth);

        static public string Deep(IEnumerable<INode> nodes, bool showFuncs = true, int depth = int.MaxValue) =>
            nodes.Select((INode node) => deepNode(node, showFuncs, depth)).Join(", ");

        static private string deepNode(INode node, bool showFuncs = true, int nodeDepth = int.MaxValue) {
            if (node is null) return "null";
            string tail = nodeDepth <= 0 ? "" :
                "(" + Deep(node.Parents, showFuncs, nodeDepth-1) + ")";
            return node.TypeName + head(node) + tail;
        }

        /*
        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => (this.Parent is not null ? this.Parent.ToString()+"." : "")+this.Name;

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) {
            string tail = "";
            if (nodeDepth <= 0) tail = "...";
            else {
                const string indent = "  ";
                string fieldStr = this.fields.SelectFromPairs((string name, INode node) => {
                    return node is IFuncGroup or IFuncDef && !showFuncs ? null :
                        name + ": " + INode.NodePrettyString(showFuncs, nodeDepth-1, node);
                }).NotNull().Indent(indent).Join(",\n" + indent);
                
                if (!string.IsNullOrEmpty(fieldStr)) tail = "\n" + indent + fieldStr + "\n";
            }
            return this.TypeName + "[" + tail + "]";
        }
         
        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) {
            string tail = "";
            if (this.defs.Count > 0) {
                if (showFuncs && nodeDepth > 0) {
                    const string indent = "  ";
                    List<string> parts = new();
                    foreach (IFuncDef def in this.defs)
                        parts.Add(INode.NodeString(def).Trim().Replace("\n", "\n"+indent));
                    tail = "\n" + indent + parts.Join(",\n" + indent) + "\n";
                } else tail = "...";
            }
            return this.TypeName + "[" + tail + "]";
        }
        */
    }
}
