using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core {

    /// <summary>This is a tool for making human readable strings from he nodes.</summary>
    sealed public class Stringifier {

        /// <summary>The amount to indent newlines.</summary>
        private const string indent = "  ";

        /// <summary>Gets a simple string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The simple string for the given nodes.</returns>
        static public string Simple(params INode[] nodes) =>
            Simple(nodes as IEnumerable<INode>);

        /// <summary>Gets a simple string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The simple string for the given nodes.</returns>
        static public string Simple(IEnumerable<INode> nodes) =>
            new Stringifier(showAllDataValues: false, showLastDataValues: false, showParents: false, showTailingNodes: false).Stringify(nodes);

        /// <summary>Gets a basic string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The basic string for the given nodes.</returns>
        static public string Basic(params INode[] nodes) =>
            Basic(nodes as IEnumerable<INode>);

        /// <summary>Gets a basic string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The basic string for the given nodes.</returns>
        static public string Basic(IEnumerable<INode> nodes) =>
            new Stringifier(showAllDataValues: false, showParents: false, showTailingNodes: false).Stringify(nodes);

        /// <summary>Gets a shallow string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The shallow string for the given nodes.</returns>
        static public string Shallow(params INode[] nodes) =>
            Shallow(nodes as IEnumerable<INode>);

        /// <summary>Gets a shallow string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The shallow string for the given nodes.</returns>
        static public string Shallow(IEnumerable<INode> nodes) =>
            new Stringifier(showAllDataValues: false, showFuncs: false, showTailingNodes: false, depth: 3).Stringify(nodes);

        /// <summary>Gets a deep string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The deep string for the given nodes.</returns>
        static public string Deep(params INode[] nodes) =>
            Deep(nodes as IEnumerable<INode>);

        /// <summary>Gets a deep string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The deep string for the given nodes.</returns>
        static public string Deep(IEnumerable<INode> nodes) =>
            new Stringifier().Stringify(nodes);

        /// <summary>Creates a new node stringifier instance.</summary>
        /// <param name="showDataType">Indicates that data types for the nodes should be outputted.</param>
        /// <param name="showAllDataValues">Indicates that values should be outputted at each node.</param>
        /// <param name="showLastDataValues">Indicates that values should only be outputted at the deepest node.</param>
        /// <param name="showParents">Indicates that parents/arguments should be outputted.</param>
        /// <param name="showTailingNodes">Indicates that fields and function definitions should be outputted.</param>
        /// <param name="showFuncs">Indicates that functions should be outputted.</param>
        /// <param name="depth">The number of parent nodes to decend into to output.</param>
        public Stringifier(
            bool showDataType       = true,
            bool showAllDataValues  = true,
            bool showLastDataValues = true,
            bool showParents        = true,
            bool showTailingNodes   = true,
            bool showFuncs          = true,
            int depth               = int.MaxValue) {
            this.ShowDataType       = showDataType;
            this.ShowAllDataValues  = showAllDataValues;
            this.ShowLastDataValues = showLastDataValues;
            this.ShowParents        = showParents;
            this.ShowTailingNodes   = showTailingNodes;
            this.ShowFuncs          = showFuncs;
            this.Depth              = depth;
        }

        /// <summary>Indicates that data types for the nodes should be outputted.</summary>
        public bool ShowDataType;

        /// <summary>Indicates that values should be outputted at each node.</summary>
        public bool ShowAllDataValues;

        /// <summary>Indicates that values should only be outputted at the deepest node.</summary>
        public bool ShowLastDataValues;

        /// <summary>Indicates that parents/arguments should be outputted.</summary>
        public bool ShowParents;

        /// <summary>Indicates that fields and function definitions should be outputted.</summary>
        public bool ShowTailingNodes;

        /// <summary>Indicates that functions should be outputted.</summary>
        public bool ShowFuncs;
        
        /// <summary>The number of parent nodes to decend into to output.</summary>
        public int Depth;

        /// <summary>Gets the string for the given nodes with the given configuration.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <returns>The string of all the given nodes.</returns>
        public string Stringify(IEnumerable<INode> nodes) =>
            this.stringNode(nodes, this.Depth);

        /// <summary>Gets the string for the given nodes with the given configuration.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <returns>The string of all the given nodes.</returns>
        public string Stringify(params INode[] nodes) =>
            this.stringNode(nodes, this.Depth);

        /// <summary>Creates a string for a collection of nodes.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string for these nodes.</returns>
        private string stringNode(IEnumerable<INode> nodes, int depth) =>
            nodes.Select((INode node) => this.stringNode(node, depth)).Join(", ");

        /// <summary>Creates a string for a single node.</summary>
        /// <param name="node">The node to stringify.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string for this node.</returns>
        private string stringNode(INode node, int depth) =>
            node is null ? "null" : node.TypeName +
            this.nodeDataType(node)   + this.nodeDataValue(node, depth) +
            this.parents(node, depth) + this.tailingNodes(node, depth);

        /// <summary>Gets the data type of the node.</summary>
        /// <param name="node">The node to get the data type of.</param>
        /// <returns>The stringfor the data type.</returns>
        private string nodeDataType(INode node) =>
            !this.ShowDataType ? "" :
            node switch {
                IFuncDef  => nodeDataType(node as IFuncDef),
                IDataNode => "<" + (node as IDataNode).Data.TypeName + ">",
                ITrigger  => "<trigger>",
                _         => "",
            };

        /// <summary>Gets the data type of the function definition.</summary>
        /// <param name="node">The function definition to get the data type of.</param>
        /// <returns>The stringfor the data type.</returns>
        static private string nodeDataType(IFuncDef node) {
            string inputTypes = "";
            if (node.ArgumentTypes.Count > 0) {
                inputTypes = node.ArgumentTypes.Strings().Join(", ");
                if (node.LastArgVariable) inputTypes += "...";
                inputTypes += "|";
            }
            return "<"+inputTypes+Type.FromType(node.ReturnType) + ">";
        }

        /// <summary>Gets the data value of the node without the node type.</summary>
        /// <param name="node">The node to get the data value of.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The stringfor the data value.</returns>
        private string nodeDataValue(INode node, int depth) =>
            !this.ShowAllDataValues && ((depth > 0 && node.Parents.Any()) || !this.ShowLastDataValues) ? "" :
            node switch {
                IDataNode => "[" + (node as IDataNode).Data.ValueString + "]",
                ITrigger  => ((node as ITrigger).Provoked ? "[provoked]" : ""),
                _         => "",
            };

        /// <summary>Gets a string for the parents of the given node</summary>
        /// <param name="node">The node to get the parents from.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string for the parents of the given node.</returns>
        private string parents(INode node, int depth) =>
            !this.ShowParents || depth <= 0 ? "" :
            node switch {
                IFuncDef => "",
                _        => "(" + this.stringNode(node.Parents, depth-1) + ")",
            };

        /// <summary>Gets a string for the tailing nodes.</summary>
        /// <param name="node">The node to get the tailing nodes from.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string containing the tailing nodes or is empty if there is no tail.</returns>
        private string tailingNodes(INode node, int depth) =>
            !this.ShowTailingNodes ? "" :
            node switch {
                IFuncGroup   => this.tailingNodes(node as IFuncGroup,   depth),
                IFieldReader => this.tailingNodes(node as IFieldReader, depth),
                _            => "",
            };

        /// <summary>Gets the function definitions for the function group.</summary>
        /// <param name="node">The function group to get the tailing nodes from.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string containing the tailing nodes.</returns>
        private string tailingNodes(IFuncGroup node, int depth) {
            if (!node.Children.Any()) return "";
            if (!this.ShowFuncs || depth <= 0) return "{...}";

            string tail = node.Children.Select(def =>
                this.stringNode(def, depth-1).Trim().Replace("\n", "\n"+indent)).
                Join(",\n" + indent);

            return "{\n" + indent + tail + "\n}";
        }

        /// <summary>Gets the all the feild nodes for the field reader.</summary>
        /// <param name="node">The field reader to get the tailing nodes from.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string containing the tailing nodes.</returns>
        private string tailingNodes(IFieldReader node, int depth) {
            if (!node.Fields.Any()) return "";
            if (depth <= 0) return "{...}";

            string tail = node.Fields.Select(pair =>
                pair.Value is IFuncGroup or IFuncDef && !this.ShowFuncs ? null :
                pair.Key + ": " + this.stringNode(pair.Value, depth-1)
            ).NotNull().Indent(indent).Join(",\n" + indent);

            return string.IsNullOrEmpty(tail) ? "" : "{\n" + indent + tail + "\n}";
        }
    }
}
