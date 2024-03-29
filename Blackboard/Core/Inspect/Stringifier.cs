﻿using Blackboard.Core.Extensions;
using Blackboard.Core.Formula.Actions;
using Blackboard.Core.Innate;
using Blackboard.Core.Inspect.Loggers;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Inspect;

/// <summary>This is a tool for making human readable strings from he nodes.</summary>
sealed internal class Stringifier {
    #region Simple...

    /// <summary>Creates a stringifier configured for simple strings.</summary>
    /// <returns>The simple stringifier.</returns>
    static public Stringifier Simple() => new(
        showAllDataValues:  false,
        showLastDataValues: false,
        showParents:        false,
        showTailingNodes:   false);

    /// <summary>Gets a simple string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The simple string for the given nodes.</returns>
    static public string Simple(params INode[] nodes) => Simple().Stringify(nodes);

    /// <summary>Gets a simple string for the given nodes even any node is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The simple string for the given nodes.</returns>
    static public string Simple(IEnumerable<INode> nodes) => Simple().Stringify(nodes);

    /// <summary>Gets a simple string for the given formula.</summary>
    /// <param name="formula">The formula to stringify.</param>
    /// <returns>The simple string for the given formula.</returns>
    static public string Simple(Formula.Formula formula) => Simple().Stringify(formula);

    /// <summary>Gets a simple string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The simple string for the given actions.</returns>
    static public string Simple(params IAction[] actions) => Simple().Stringify(actions);

    /// <summary>Gets a simple string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The simple string for the given actions.</returns>
    static public string Simple(IEnumerable<IAction> actions) => Simple().Stringify(actions);

    #endregion
    #region Basic...

    /// <summary>Creates a stringifier configured for basic strings.</summary>
    /// <returns>The basic stringifier.</returns>
    static public Stringifier Basic() => new(
        showAllDataValues:   false,
        showLastDataValues:  false,
        showFirstDataValues: false,
        showParents:         false,
        showFuncs:           false,
        showTailingNodes:    false);

    /// <summary>Gets a basic string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The basic string for the given nodes.</returns>
    static public string Basic(params INode[] nodes) => Basic().Stringify(nodes);

    /// <summary>Gets a basic string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The basic string for the given nodes.</returns>
    static public string Basic(IEnumerable<INode> nodes) => Basic().Stringify(nodes);

    /// <summary>Gets a basic string for the given formula.</summary>
    /// <param name="formula">The formula to stringify.</param>
    /// <returns>The basic string for the given formula.</returns>
    static public string Basic(Formula.Formula formula) => Simple().Stringify(formula);

    /// <summary>Gets a basic string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The basic string for the given actions.</returns>
    static public string Basic(params IAction[] actions) => Basic().Stringify(actions);

    /// <summary>Gets a basic string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The basic string for the given actions.</returns>
    static public string Basic(IEnumerable<IAction> actions) => Basic().Stringify(actions);

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

    /// <summary>Gets a shallow string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The shallow string for the given nodes.</returns>
    static public string Shallow(params INode?[] nodes) => Shallow().Stringify(nodes);

    /// <summary>Gets a shallow string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The shallow string for the given nodes.</returns>
    static public string Shallow(IEnumerable<INode?> nodes) => Shallow().Stringify(nodes);

    /// <summary>Gets a shallow string for the given formula.</summary>
    /// <param name="formula">The formula to stringify.</param>
    /// <returns>The shallow string for the given formula.</returns>
    static public string Shallow(Formula.Formula formula) => Simple().Stringify(formula);

    /// <summary>Gets a shallow string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The shallow string for the given actions.</returns>
    static public string Shallow(params IAction[] actions) => Shallow().Stringify(actions);

    /// <summary>Gets a shallow string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The shallow string for the given actions.</returns>
    static public string Shallow(IEnumerable<IAction> actions) => Shallow().Stringify(actions);

    #endregion
    #region Deep...

    /// <summary>Creates a stringifier configured for deep strings.</summary>
    /// <returns>The deep stringifier.</returns>
    static public Stringifier Deep() => new();

    /// <summary>Gets a deep string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The deep string for the given nodes.</returns>
    static public string Deep(params INode[] nodes) => Deep().Stringify(nodes);

    /// <summary>Gets a deep string for the given nodes even any node which is null.</summary>
    /// <param name="nodes">The set of nodes which may contain nulls.</param>
    /// <returns>The deep string for the given nodes.</returns>
    static public string Deep(IEnumerable<INode> nodes) => Deep().Stringify(nodes);

    /// <summary>Gets a deep string for the given formula.</summary>
    /// <param name="formula">The formula to stringify.</param>
    /// <returns>The deep string for the given formula.</returns>
    static public string Deep(Formula.Formula formula) => Deep().Stringify(formula);

    /// <summary>Gets a deep string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The deep string for the given actions.</returns>
    static public string Deep(params IAction[] actions) => Deep().Stringify(actions);

    /// <summary>Gets a deep string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The deep string for the given actions.</returns>
    static public string Deep(IEnumerable<IAction> actions) => Deep().Stringify(actions);

    #endregion

    /// <summary>Gets a string showing the whole slate from the global.</summary>
    /// <param name="slate">The slate to get the graph from.</param>
    /// <param name="showFuncs">Indicates that functions should be outputted.</param>
    /// <returns>The full debug string for this slate.</returns>
    static public string GraphString(Slate slate, bool showFuncs = false) {
        Stringifier stringifier = Deep();
        stringifier.ShowFuncs = showFuncs;
        stringifier.PreLoadNames(slate);
        return stringifier.Stringify(slate.Global);
    }

    /// <summary>Creates a new node stringifier instance.</summary>
    /// <param name="showDataType">Indicates that data types for the nodes should be outputted.</param>
    /// <param name="showAllDataValues">Indicates that values should be outputted at each node.</param>
    /// <param name="showLastDataValues">Indicates that values should only be outputted at the deepest node.</param>
    /// <param name="showFirstDataValues">Indicates that values should only be outputted at the shallowest node.</param>
    /// <param name="showParents">Indicates that parents/arguments should be outputted.</param>
    /// <param name="showTailingNodes">Indicates that fields and function definitions should be outputted.</param>
    /// <param name="showFuncs">Indicates that functions should be outputted.</param>
    /// <param name="showSimpleConstants">Indicates that constants should be shown just as their value.</param>
    /// <param name="depth">The number of parent nodes to descend into to output.</param>
    /// <param name="indent">The string to indent with.</param>
    public Stringifier(
        bool showDataType        = true,
        bool showAllDataValues   = false,
        bool showLastDataValues  = true,
        bool showFirstDataValues = true,
        bool showParents         = true,
        bool showTailingNodes    = true,
        bool showFuncs           = true,
        bool showSimpleConstants = true,
        int  depth               = int.MaxValue,
        string indent            = "  ") {
        this.ShowDataType        = showDataType;
        this.ShowAllDataValues   = showAllDataValues;
        this.ShowLastDataValues  = showLastDataValues;
        this.ShowFirstDataValues = showFirstDataValues;
        this.ShowParents         = showParents;
        this.ShowTailingNodes    = showTailingNodes;
        this.ShowFuncs           = showFuncs;
        this.ShowSimpleConstants = showSimpleConstants;
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

    /// <summary>Indicates that constants should be shown just as their value.</summary>
    public bool ShowSimpleConstants;

    /// <summary>The number of parent nodes to descend into to output.</summary>
    public int Depth;

    /// <summary>The amount to indent newlines.</summary>
    public string Indent;

    /// <summary>The list of fields which have been added to node names.</summary>
    private readonly HashSet<IFieldReader> readFieldNodes;

    /// <summary>A cache of named nodes used for replacing parent nodes with named ones.</summary>
    /// <remarks>This will be automatically populated by Namespaces.</remarks>
    private readonly Dictionary<INode, string> nodeNames;

    #region Naming...

    /// <summary>Sets the name to show for a node.</summary>
    /// <remarks>If the node is already named, the name is overwritten with this new name.</remarks>
    /// <param name="name">The name to show for the node.</param>
    /// <param name="node">The node to give a name to.</param>
    /// <returns>The stringifier so that calls can be chained.</returns>
    public Stringifier SetNodeName(string name, INode node) {
        this.nodeNames[node] = name;
        return this;
    }

    /// <summary>Sets the names to show for nodes.</summary>
    /// <param name="other">The other stringifier to get the preloaded names from.</param>
    /// <returns>The stringifier so that calls can be chained.</returns>
    public Stringifier PreLoadNames(Stringifier other) {
        other.nodeNames.Foreach(this.nodeNames.Add);
        return this;
    }

    /// <summary>Preloads the node name to use when outputting using the namespaces reachable from global.</summary>
    /// <param name="slate">The slate containing the global namespace to load.</param>
    /// <returns>The stringifier so that calls can be chained.</returns>
    public Stringifier PreLoadNames(Slate slate) {
        this.SetNodeName("Global", slate.Global);
        return this.PreLoadNames(slate.Global);
    }

    /// <summary>Preloads the node names to use when outputting the parents of nodes.</summary>
    /// <param name="node">The node with the readers to start preloading names with.</param>
    /// <returns>The stringifier so that calls can be chained.</returns>
    public Stringifier PreLoadNames(IFieldReader node) =>
        this.preLoadNames(node, "");
    
    /// <summary>Preloads the node names to use when outputting the parents of nodes.</summary>
    /// <param name="node">The node with the readers to start preloading names with.</param>
    /// <param name="prefix">The part of the name to add at the from of the names for the namespace.</param>
    /// <returns>The stringifier so that calls can be chained.</returns>
    private Stringifier preLoadNames(IFieldReader node, string prefix) {
        if (this.readFieldNodes.Contains(node)) return this;
        this.readFieldNodes.Add(node);
        foreach (KeyValuePair<string, INode> pair in node.Fields) {
            string name = pair.Key;
            if (prefix.Length > 0) name = prefix + "." + name;
            if (pair.Value is IFieldReader fieldReader) this.preLoadNames(fieldReader, name);
            this.SetNodeName(name, pair.Value);
        }
        return this;
    }

    #endregion
    #region Nodes...

    /// <summary>Gets the string for the given nodes with the given configuration.</summary>
    /// <param name="nodes">The nodes to stringify.</param>
    /// <returns>The string of all the given nodes.</returns>
    public string Stringify(IEnumerable<INode?> nodes) =>
        this.stringNode(nodes, this.Depth, false, true);

    /// <summary>Gets the string for the given nodes with the given configuration.</summary>
    /// <param name="nodes">The nodes to stringify.</param>
    /// <returns>The string of all the given nodes.</returns>
    public string Stringify(params INode?[] nodes) =>
        this.stringNode(nodes, this.Depth, false, true);

    /// <summary>Creates a string for a collection of nodes.</summary>
    /// <param name="nodes">The nodes to stringify.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <param name="useOnlyName">Indicates that if there is a name for a node, use that name for the output.</param>
    /// <param name="first">Indicates this is a first node and if it has a name, it should show it.</param>
    /// <returns>The string for these nodes.</returns>
    private string stringNode(IEnumerable<INode?> nodes, int depth, bool useOnlyName, bool first) =>
        nodes.Select(node => this.stringNode(node, depth, useOnlyName, first)).Join(", ");

    /// <summary>Creates a string for a single node.</summary>
    /// <param name="node">The node to stringify.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <param name="useOnlyName">Indicates that if there is a name for a node, use that name for the output.</param>
    /// <param name="first">Indicates this is a first node and if it has a name, it should show it.</param>
    /// <returns>The string for this node.</returns>
    private string stringNode(INode? node, int depth, bool useOnlyName, bool first) {
        if (node is null) return "null";
        if (node is IFieldReader fieldReader) this.PreLoadNames(fieldReader);

        // Check if a named parent which we can stop at with the name.
        if (useOnlyName && this.nodeNames.TryGetValue(node, out string? name1))
            return name1 + this.nodeDataValue(node, depth, first);

        // Check if a constant which can be outputted as just the value.
        if (this.ShowSimpleConstants && node is IConstant)
            return this.nodeDataType(node) + getDataValue(node);

        // Construct the node and any of its children, if it is a first node and it has a name, show it.
        return (first && this.nodeNames.TryGetValue(node, out string? name2) ? name2+": " : "") +
            this.stringNodeWithoutName(node, depth, first);
    }

    /// <summary>Creates a string for a single node without the a name.</summary>
    /// <param name="node">The node to stringify.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <param name="first">Indicates this is a first node and if it has a name, it should show it.</param>
    /// <returns>The string for this node.</returns>
    private string stringNodeWithoutName(INode? node, int depth, bool first) =>
        node is null ? "null" : node.TypeName +
        this.nodeDataType(node)   + this.nodeDataValue(node, depth, first) +
        this.parents(node, depth) + this.tailingNodes(node, depth);

    /// <summary>Gets the data type of the node.</summary>
    /// <param name="node">The node to get the data type of.</param>
    /// <returns>The string for the data type.</returns>
    private string nodeDataType(INode? node) =>
        !this.ShowDataType ? "" :
        node switch {
            null          => "null",
            IFuncDef  def => nodeDataType(def),
            IDataNode dat => "<" + dat.Data.Type.Name + ">",
            ITrigger      => "<trigger>",
            _             => "",
        };

    /// <summary>Gets the data type of the function definition.</summary>
    /// <param name="node">The function definition to get the data type of.</param>
    /// <returns>The string for the data type.</returns>
    static private string nodeDataType(IFuncDef node) {
        string inputTypes = "";
        if (node.ArgumentTypes.Count > 0) {
            inputTypes = node.ArgumentTypes.Join(", ");
            if (node.LastArgVariable) inputTypes += "...";
            inputTypes += "|";
        }
        return "<"+inputTypes+Type.FromType(node.ReturnType) + ">";
    }

    /// <summary>Gets the string for the data value of the given node.</summary>
    /// <param name="node">The node to get the data value for.</param>
    /// <returns>The string for the data value.</returns>
    private static string getDataValue(INode? node) =>
        node switch {
            null          => "null",
            IDataNode dat => "[" + dat.Data.ValueAsString + "]",
            ITrigger trig => (trig.Provoked ? "[provoked]" : "[]"),
            _             => "",
        };

    /// <summary>Gets the data value of the node without the node type.</summary>
    /// <param name="node">The node to get the data value of.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <param name="first">Indicates this is a first node.</param>
    /// <returns>The string for the data value.</returns>
    private string nodeDataValue(INode? node, int depth, bool first) =>
        this.ShowAllDataValues ? getDataValue(node) :
        this.ShowFirstDataValues && first ? getDataValue(node) :
        this.ShowLastDataValues && (depth <= 1 || node is not IChild child || !child.Parents.HasParents) ? getDataValue(node) : "";

    /// <summary>Gets a string for the parents of the given node</summary>
    /// <param name="node">The node to get the parents from.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <returns>The string for the parents of the given node.</returns>
    private string parents(INode? node, int depth) =>
        !this.ShowParents || depth <= 1 || node is not IChild child || !child.Parents.HasParents ? "" :
        node switch {
            IFuncDef => "",
            _        => "(" + this.stringNode(child.Parents, depth-1, true, false) + ")",
        };

    /// <summary>Gets a string for the tailing nodes.</summary>
    /// <param name="node">The node to get the tailing nodes from.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <returns>The string containing the tailing nodes or is empty if there is no tail.</returns>
    private string tailingNodes(INode? node, int depth) =>
        !this.ShowTailingNodes ? "" :
        node switch {
            null                => "null",
            IFuncGroup   group  => this.tailingNodes(group,  depth),
            IFieldReader reader => this.tailingNodes(reader, depth),
            _                   => "",
        };

    /// <summary>Gets the function definitions for the function group.</summary>
    /// <param name="node">The function group to get the tailing nodes from.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <returns>The string containing the tailing nodes.</returns>
    private string tailingNodes(IFuncGroup node, int depth) {
        if (!node.Definitions.Any()) return "";
        if (!this.ShowFuncs || depth <= 1) return "{...}";
        string nl = "\n";

        string tail = node.Definitions.Select(def =>
            this.stringNode(def, depth-1, false, false).Trim().Indent(this.Indent)).
            Join("," + nl);

        return "{" + nl + tail + nl + "}";
    }

    /// <summary>Gets the all the field nodes for the field reader.</summary>
    /// <param name="node">The field reader to get the tailing nodes from.</param>
    /// <param name="depth">The depth to output theses nodes to.</param>
    /// <returns>The string containing the tailing nodes.</returns>
    private string tailingNodes(IFieldReader node, int depth) {
        if (!node.Fields.Any()) return "";
        if (depth <= 1) return "{...}";
        string nl = "\n";

        string tail = node.Fields.Select(pair =>
            (pair.Value is IFuncGroup or IFuncDef) && !this.ShowFuncs ? null :
            pair.Key == Operators.Namespace ? null :
            pair.Key + ": " + this.stringNodeWithoutName(pair.Value, depth-1, true)
        ).NotNull().Indent(this.Indent).Join("," + nl);

        return string.IsNullOrEmpty(tail) ? "" : "{" + nl + tail + nl +"}";
    }

    /// <summary>Gets a short string for the field reader. Typically used to stringify receivers.</summary>
    /// <param name="node">The node to stringify.</param>
    /// <returns>The string for the given field reader.</returns>
    private string shortFieldReader(IFieldReader node) =>
        node is null ? "null" :
        this.nodeNames.TryGetValue(node, out string? name) ? name :
        node.TypeName;

    #endregion
    #region Actions...

    /// <summary>Get the string for the given formula.</summary>
    /// <param name="formula">The formula to stringify.</param>
    /// <returns>The string for the given formula.</returns>
    public string Stringify(Formula.Formula formula) {
        this.PreLoadNames(formula.slate);
        const string nl = "\n";
        return formula is null ? "null" : formula.Actions.Count <= 0 ? "[]" :
            "[" + nl + this.Stringify(formula.Actions).Indent(this.Indent) + nl + "]";
    }

    /// <summary>Gets a string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The string for the given actions.</returns>
    public string Stringify(params IAction[] actions) =>
        this.Stringify(actions as IEnumerable<IAction>);

    /// <summary>Gets a string for the given actions even any action which is null.</summary>
    /// <param name="actions">The actions to stringify.</param>
    /// <returns>The string for the given actions.</returns>
    public string Stringify(IEnumerable<IAction> actions) =>
        actions.Select(this.stringAction).Join("\n");

    /// <summary>Gets the string for a given action.</summary>
    /// <param name="action">The action to stringify.</param>
    /// <returns>The string for the given action.</returns>
    private string stringAction(IAction action) =>
        action switch {
            null            => "null",
            IGetter getter  => this.stringGetter(getter),
            IAssign assign  => this.stringAssign(assign),
            Define  define  => this.stringDefine(define),
            Extern  ext     => this.stringAddExtern(ext),
            Temp    temp    => this.stringTemp(temp),
            Provoke provoke => this.stringProvoke(provoke),
            Finish          => "Finish",
            _               => "Unknown Action",
        };

    /// <summary>Get the string for the given getter action.</summary>
    /// <param name="getter">The getter action to stringify.</param>
    /// <returns>The string for the given output action.</returns>
    private string stringGetter(IGetter getter) =>
        getter.Names.Join(".") + " <= " + this.Stringify(getter.Node) +
            " {" + Simple().PreLoadNames(this).Stringify(getter.NeedPending.Nodes) + "};";

    /// <summary>Get the string for the given assign action.</summary>
    /// <param name="assign">The assign action to stringify.</param>
    /// <returns>The string for the given assign action.</returns>
    private string stringAssign(IAssign assign) =>
        this.Stringify(assign.Target) + " = " + this.Stringify(assign.Value) +
            " {" + Simple().PreLoadNames(this).Stringify(assign.NeedPending.Nodes) + "};";

    /// <summary>Get the string for the given define action.</summary>
    /// <param name="define">The define action to stringify.</param>
    /// <returns>The string for the given define action.</returns>
    private string stringDefine(Define define) =>
        this.shortFieldReader(define.Receiver) + "." + define.Name + " := " + this.Stringify(define.Node) +
            " {" + Simple().PreLoadNames(this).Stringify(define.NeedParents) + "};";
    
    /// <summary>Get the string for the given add extern action.</summary>
    /// <param name="ext">The add extern action to stringify.</param>
    /// <returns>The string for the given define action.</returns>
    private string stringAddExtern(Extern ext) =>
        this.shortFieldReader(ext.Receiver) + "." + ext.Name + " := " + this.Stringify(ext.Node) + ";";

    /// <summary>Get the string for the given temp action.</summary>
    /// <param name="temp">The temp action to stringify.</param>
    /// <returns>The string for the given temp action.</returns>
    private string stringTemp(Temp temp) =>
        temp.Name + " := " + this.Stringify(temp.Node) +
            " {" + Simple().PreLoadNames(this).Stringify(temp.NeedParents) + "};";

    /// <summary>Get the string for the given provoke action.</summary>
    /// <param name="provoke">The provoke action to stringify.</param>
    /// <returns>The string for the given provoke action.</returns>
    private string stringProvoke(Provoke provoke) =>
        this.Stringify(provoke.Trigger) + " -> " + this.Stringify(provoke.Target) +
            " {" + Simple().PreLoadNames(this).Stringify(provoke.NeedPending.Nodes) + "};";

    #endregion
    #region Other...

    /// <summary>Converts the given object with this stringifier.</summary>
    /// <param name="value">The value to stringify.</param>
    /// <returns>
    /// The string of the given action or node,
    /// or the original value if not an action or node.
    /// </returns>
    public object StringifyObject(object? value) =>
        value switch {
            null                       => "null",
            INode               node   => this.Stringify(node),
            IAction             action => this.Stringify(action),
            Message             msg    => msg.Stringify(this),
            IEnumerable<object> list   => list.Select(this.StringifyObject).Join(", "),
            S.Type              type   => type.FormattedTypeName(),
            _                          => value
        };

    /// <summary>Converts the given collection object with this stringifier.</summary>
    /// <param name="value">The values to stringify.</param>
    /// <returns>
    /// For each object this returns the string of a given action or node,
    /// or the original value if not an action or node.
    /// </returns>
    public IEnumerable<object> StringifyObject(IEnumerable<object?> values) =>
        values.Select(this.StringifyObject);

    #endregion
}
