using Blackboard.Core.Actions;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using System.Collections.Generic;

namespace Blackboard.Core;

/// <summary>A collection of creation methods which switch based on type.</summary>
static public class Maker {

    /// <summary>This gets the function group to cast to the given type.</summary>
    /// <param name="slate">The slate to look up the casts with.</param>
    /// <param name="type">The type the cast returns.</param>
    /// <returns>The function group for casting a value to the given type or null if an unexpected type./returns>
    static public IFuncGroup? GetCastMethod(Slate slate, Type type) {
        if (slate.Global.Find(Slate.OperatorNamespace) is not Namespace ops) return null;
        INode? castGroup =
            type == Type.Object  ? ops.Find("castObject") :
            type == Type.Bool    ? ops.Find("castBool") :
            type == Type.Int     ? ops.Find("castInt") :
            type == Type.Uint    ? ops.Find("castUint") :
            type == Type.Double  ? ops.Find("castDouble") :
            type == Type.String  ? ops.Find("castString") :
            type == Type.Trigger ? ops.Find("castTrigger") :
            null;
        return castGroup as IFuncGroup;
    }

    /// <summary>This creates an assignment action for the given type.</summary>
    /// <param name="type">The type to create an assignment action for.</param>
    /// <param name="target">The target node to assign to.</param>
    /// <param name="root">The root node to assign to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    /// <returns>The newly created assignment action or null if an unexpected type.</returns>
    static public IAction? CreateAssignAction(Type type, INode target, INode root, IEnumerable<INode> allNewNodes) =>
        type == Type.Object  ? Assign<Object>.Create(target, root, allNewNodes) :
        type == Type.Bool    ? Assign<Bool>.  Create(target, root, allNewNodes) :
        type == Type.Int     ? Assign<Int>.   Create(target, root, allNewNodes) :
        type == Type.Uint    ? Assign<Uint>.  Create(target, root, allNewNodes) :
        type == Type.Double  ? Assign<Double>.Create(target, root, allNewNodes) :
        type == Type.String  ? Assign<String>.Create(target, root, allNewNodes) :
        type == Type.Trigger ? Provoke.       Create(target, root, allNewNodes) :
        null;

    /// <summary>This creates an getter action for the given type.</summary>
    /// <param name="type">The type to create an getter action for.</param>
    /// <param name="names">The names in the path to write the value to.</param>
    /// <param name="root">The root node to get to the given target.</param>
    /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
    /// <returns>The newly created getter action or null if an unexpected type.</returns>
    static public IAction? CreateGetterAction(Type type, string[] names, INode root, IEnumerable<INode> allNewNodes) =>
        type == Type.Object  ? ValueGetter<Object>.Create(names, root, allNewNodes) :
        type == Type.Bool    ? ValueGetter<Bool>.  Create(names, root, allNewNodes) :
        type == Type.Int     ? ValueGetter<Int>.   Create(names, root, allNewNodes) :
        type == Type.Uint    ? ValueGetter<Uint>.  Create(names, root, allNewNodes) :
        type == Type.Double  ? ValueGetter<Double>.Create(names, root, allNewNodes) :
        type == Type.String  ? ValueGetter<String>.Create(names, root, allNewNodes) :
        type == Type.Trigger ? TriggerGetter.      Create(names, root, allNewNodes) :
        null;

    /// <summary>Creates a new input node of the given type.</summary>
    /// <param name="type">The type of value to create an input node for.</param>
    /// <returns>The newly created input or null if an unexpected type.</returns>
    static public IInput? CreateInputNode(Type type) =>
        type == Type.Object  ? new InputValue<Object>() :
        type == Type.Bool    ? new InputValue<Bool>() :
        type == Type.Int     ? new InputValue<Int>() :
        type == Type.Uint    ? new InputValue<Uint>() :
        type == Type.Double  ? new InputValue<Double>() :
        type == Type.String  ? new InputValue<String>() :
        type == Type.Trigger ? new InputTrigger() :
        null;

    /// <summary>Creates a new extern node of the given type.</summary>
    /// <param name="type">The type of value to create an extern node for.</param>
    /// <returns>The newly created extern or null if an unexpected type.</returns>
    static public IExtern? CreateExternNode(Type type) =>
        type == Type.Object  ? new ExternValue<Object>() :
        type == Type.Bool    ? new ExternValue<Bool>() :
        type == Type.Int     ? new ExternValue<Int>() :
        type == Type.Uint    ? new ExternValue<Uint>() :
        type == Type.Double  ? new ExternValue<Double>() :
        type == Type.String  ? new ExternValue<String>() :
        type == Type.Trigger ? new ExternTrigger() :
        null;

    /// <summary>Creates a new shell node to wrap the given node.</summary>
    /// <param name="type">The type of value to create a shell node for.</param>
    /// <returns>The newly created shell or null if an unexpected type.</returns>
    static public INode? CreateShell(INode node) =>
        node is IValueParent<Object> objectNode  ? new ShellValue<Object>(objectNode) :
        node is IValueParent<Bool>   boolNode    ? new ShellValue<Bool>(boolNode) :
        node is IValueParent<Int>    intNode     ? new ShellValue<Int>(intNode) :
        node is IValueParent<Uint>   uintNode    ? new ShellValue<Uint>(uintNode) :
        node is IValueParent<Double> doubleNode  ? new ShellValue<Double>(doubleNode) :
        node is IValueParent<String> stringNode  ? new ShellValue<String>(stringNode) :
        node is ITriggerParent       triggerNode ? new ShellTrigger(triggerNode) :
        null;
}
