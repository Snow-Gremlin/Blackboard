using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core {

    /// <summary>This is a tool for making human readable strings from he nodes.</summary>
    sealed public class Stringifier {

        #region Simple...

        /// <summary>Creates a stringifier configured for simple strings.</summary>
        /// <returns>The simple stringifier.</returns>
        static public Stringifier Simple() => new(
            showAllDataValues:  false,
            showLastDataValues: false,
            showParents:        false,
            showTailingNodes:   false);

        /// <summary>Gets a simple string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The simple string for the given nodes.</returns>
        static public string Simple(params INode[] nodes) => Simple().Stringify(nodes);

        /// <summary>Gets a simple string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The simple string for the given nodes.</returns>
        static public string Simple(IEnumerable<INode> nodes) => Simple().Stringify(nodes);

        #endregion
        #region Basic...

        /// <summary>Creates a stringifier configured for basic strings.</summary>
        /// <returns>The basic stringifier.</returns>
        static public Stringifier Basic() => new(
            showAllDataValues:  false,
            showLastDataValues: false,
            showParents:        false,
            showTailingNodes:   false);

        /// <summary>Gets a basic string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The basic string for the given nodes.</returns>
        static public string Basic(params INode[] nodes) => Basic().Stringify(nodes);

        /// <summary>Gets a basic string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The basic string for the given nodes.</returns>
        static public string Basic(IEnumerable<INode> nodes) => Basic().Stringify(nodes);

        #endregion
        #region Shallow...

        /// <summary>Creates a stringifier configured for shallow strings.</summary>
        /// <returns>The shallow stringifier.</returns>
        static public Stringifier Shallow() => new(
            showAllDataValues:  false,
            showLastDataValues: false,
            showTailingNodes:   false,
            showFuncs:          false,
            depth:              3);
          
        /// <summary>Gets a shallow string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The shallow string for the given nodes.</returns>
        static public string Shallow(params INode[] nodes) => Shallow().Stringify(nodes);

        /// <summary>Gets a shallow string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The shallow string for the given nodes.</returns>
        static public string Shallow(IEnumerable<INode> nodes) => Shallow().Stringify(nodes);

        #endregion
        #region Deep...

        /// <summary>Creates a stringifier configured for deep strings.</summary>
        /// <returns>The deep stringifier.</returns>
        static public Stringifier Deep() => new();

        /// <summary>Gets a deep string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The deep string for the given nodes.</returns>
        static public string Deep(params INode[] nodes) => Deep().Stringify(nodes);

        /// <summary>Gets a deep string for the given nodes even any node is null.</summary>
        /// <param name="nodes">The set of nodes which may contain nulls.</param>
        /// <returns>The deep string for the given nodes.</returns>
        static public string Deep(IEnumerable<INode> nodes) => Deep().Stringify(nodes);

        #endregion
 
        /// <summary>Gets a string showing the whole driver from the global.</summary>
        /// <param name="driver">The driver to get the graph from.</param>
        /// <param name="showFuncs">Indicates that functions should be outputted.</param>
        /// <returns>The full debug string for this driver.</returns>
        static public string GraphString(Driver driver, bool showFuncs = false) {
            Stringifier stringifier = Deep();
            stringifier.ShowFuncs = showFuncs;
            stringifier.PreloadNames(driver);
            return stringifier.Stringify(driver.Global);
        }

        /// <summary>Creates a new node stringifier instance.</summary>
        /// <param name="showDataType">Indicates that data types for the nodes should be outputted.</param>
        /// <param name="showAllDataValues">Indicates that values should be outputted at each node.</param>
        /// <param name="showLastDataValues">Indicates that values should only be outputted at the deepest node.</param>
        /// <param name="showParents">Indicates that parents/arguments should be outputted.</param>
        /// <param name="showTailingNodes">Indicates that fields and function definitions should be outputted.</param>
        /// <param name="showFuncs">Indicates that functions should be outputted.</param>
        /// <param name="depth">The number of parent nodes to decend into to output.</param>
        /// <param name="indent">The string to indent with.</param>
        public Stringifier(
            bool showDataType        = true,
            bool showAllDataValues   = true,
            bool showLastDataValues  = true,
            bool showFirstDataValues = true,
            bool showParents         = true,
            bool showTailingNodes    = true,
            bool showFuncs           = true,
            int  depth               = int.MaxValue,
            string indent            = "  ") {
            this.ShowDataType        = showDataType;
            this.ShowAllDataValues   = showAllDataValues;
            this.ShowLastDataValues  = showLastDataValues;
            this.ShowFirstDataValues = showFirstDataValues;
            this.ShowParents         = showParents;
            this.ShowTailingNodes    = showTailingNodes;
            this.ShowFuncs           = showFuncs;
            this.Depth               = depth;
            this.Indent              = indent;
            this.readFieldNodes      = new HashSet<IFieldReader>();
            this.nodeNames           = new Dictionary<INode, string>();
        }

        /// <summary>Indicates that data types for the nodes should be outputted.</summary>
        public bool ShowDataType;

        /// <summary>Indicates that values should be outputted at each node.</summary>
        public bool ShowAllDataValues;

        /// <summary>Indicates that values should be outputted at the deepest node.</summary>
        /// <remarks>This is ignored if ShowAllDataValues is true.</remarks>
        public bool ShowLastDataValues;

        /// <summary>Indicates that values should be outputted if the node is a first node.</summary>
        /// <remarks>This is ignored if ShowAllDataValues is true.</remarks>
        public bool ShowFirstDataValues;

        /// <summary>Indicates that parents/arguments should be outputted.</summary>
        public bool ShowParents;

        /// <summary>Indicates that fields and function definitions should be outputted.</summary>
        public bool ShowTailingNodes;

        /// <summary>Indicates that functions should be outputted.</summary>
        public bool ShowFuncs;
        
        /// <summary>The number of parent nodes to decend into to output.</summary>
        public int Depth;

        /// <summary>The amount to indent newlines.</summary>
        public string Indent;

        /// <summary>The list of fields which have been added to node names.</summary>
        private HashSet<IFieldReader> readFieldNodes;

        /// <summary>A cache of named nodes used for replacing parent nodes with named ones.</summary>
        /// <remarks>This will be automatically populated by namespaces.</remarks>
        private Dictionary<INode, string> nodeNames;

        /// <summary>Sets the name to show for a node.</summary>
        /// <remarks>If the node is already named, the name is overwritten with this new name.</remarks>
        /// <param name="name">The name to show for the node.</param>
        /// <param name="node">The node to give a name to.</param>
        public void SetNodeName(string name, INode node) =>
            this.nodeNames.Add(node, name);

        /// <summary>Preloads the node name to use when outputting using the namespaces reachable from global.</summary>
        /// <param name="driver">The driver containing the global namespace to load.</param>
        public void PreloadNames(Driver driver) {
            this.SetNodeName("Global", driver.Global);
            this.PreloadNames(driver.Global);
        }

        /// <summary>Preloads the node names to use when outputting the parents of nodes.</summary>
        /// <param name="node">The node with the readers to start preloading names with.</param>
        public void PreloadNames(IFieldReader node) {
            if (this.readFieldNodes.Contains(node)) return;
            this.readFieldNodes.Add(node);
            foreach (KeyValuePair<string, INode> pair in node.Fields) {
                if (pair.Value is IFieldReader fieldReader) this.PreloadNames(fieldReader);
                this.SetNodeName(pair.Key, pair.Value);
            }
        }

        /// <summary>Gets the string for the given nodes with the given configuration.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <returns>The string of all the given nodes.</returns>
        public string Stringify(IEnumerable<INode> nodes) =>
            this.stringNode(nodes, this.Depth, false, true);

        /// <summary>Gets the string for the given nodes with the given configuration.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <returns>The string of all the given nodes.</returns>
        public string Stringify(params INode[] nodes) =>
            this.stringNode(nodes, this.Depth, false, true);

        /// <summary>Creates a string for a collection of nodes.</summary>
        /// <param name="nodes">The nodes to stringify.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <param name="useOnlyName">Indicates that if there is a name for a node, use that name for the output.</param>
        /// <param name="first">Indicates this is a first node and if it has a name, it should show it.</param>
        /// <returns>The string for these nodes.</returns>
        private string stringNode(IEnumerable<INode> nodes, int depth, bool useOnlyName, bool first) =>
            nodes.Select((INode node) => this.stringNode(node, depth, useOnlyName, first)).Join(", ");

        /// <summary>Creates a string for a single node.</summary>
        /// <param name="node">The node to stringify.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <param name="useOnlyName">Indicates that if there is a name for a node, use that name for the output.</param>
        /// <param name="first">Indicates this is a first node and if it has a name, it should show it.</param>
        /// <returns>The string for this node.</returns>
        private string stringNode(INode node, int depth, bool useOnlyName, bool first) {
            if (node is null) return "null";
            if (node is IFieldReader fieldReader) this.PreloadNames(fieldReader);

            // Check if a named parent which we can stop at with the name.
            if (useOnlyName && this.nodeNames.ContainsKey(node))
                return this.nodeNames[node] + this.nodeDataValue(node, depth, first);

            // Construct the node and any of its children, if it is a first node and it has a name, show it.
            return (first && this.nodeNames.ContainsKey(node) ? this.nodeNames[node]+": " : "") +
                node.TypeName +
                this.nodeDataType(node)   + this.nodeDataValue(node, depth, first) +
                this.parents(node, depth) + this.tailingNodes(node, depth);
        }

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
                inputTypes = node.ArgumentTypes.Join(", ");
                if (node.LastArgVariable) inputTypes += "...";
                inputTypes += "|";
            }
            return "<"+inputTypes+Type.FromType(node.ReturnType) + ">";
        }

        /// <summary>Gets the data value of the node without the node type.</summary>
        /// <param name="node">The node to get the data value of.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <param name="first">Indicates this is a first node.</param>
        /// <returns>The stringfor the data value.</returns>
        private string nodeDataValue(INode node, int depth, bool first) =>
            !this.ShowAllDataValues &&
            (!this.ShowFirstDataValues || !first) &&
            (!this.ShowLastDataValues || (depth > 1 && node.Parents.Any())) ? "" :
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
            !this.ShowParents || depth <= 1 || !node.Parents.Any() ? "" :
            node switch {
                IFuncDef => "",
                _        => "(" + this.stringNode(node.Parents, depth-1, true, false) + ")",
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
            if (!this.ShowFuncs || depth <= 1) return "{...}";
            string nl = S.Environment.NewLine;

            string tail = node.Children.Select(def =>
                this.stringNode(def, depth-1, false, false).Trim().Replace(nl, nl + this.Indent)).
                Join("," + nl + this.Indent);

            return "{" + nl + this.Indent + tail + nl + "}";
        }

        /// <summary>Gets the all the feild nodes for the field reader.</summary>
        /// <param name="node">The field reader to get the tailing nodes from.</param>
        /// <param name="depth">The depth to output theses nodes to.</param>
        /// <returns>The string containing the tailing nodes.</returns>
        private string tailingNodes(IFieldReader node, int depth) {
            if (!node.Fields.Any()) return "";
            if (depth <= 1) return "{...}";
            string nl = S.Environment.NewLine;

            string tail = node.Fields.Select(pair =>
                (pair.Value is IFuncGroup or IFuncDef) && !this.ShowFuncs ? null :
                pair.Key == Driver.OperatorNamespace ? null :
                pair.Key + ": " + this.stringNode(pair.Value, depth-1, false, false)
            ).NotNull().Indent(this.Indent).Join("," + nl + this.Indent);

            return string.IsNullOrEmpty(tail) ? "" : "{" + nl + this.Indent + tail + nl +"}";
        }
    }
}
