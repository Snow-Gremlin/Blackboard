using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core {

    /// <summary>Collection of tools for casting and testing node types.</summary>
    static public class Cast {

        /// <summary>Determines if the given node can be cast to bool IValues.</summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int BoolMatch(INode node) =>
            node is IValue<bool> ? 0 : -1;

        /// <summary>Determines if the given node can be cast to triggers.</summary>
        /// <remarks>
        /// A bool can be converted to a trigger for logic equations
        /// but normally isn't implicity cast.
        /// </remarks>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int TriggerMatch(INode node) =>
            node is ITrigger     ? 0 :
            node is IValue<bool> ? 1 :
            -1;

        /// <summary>Determines if the given node can be cast to int IValues.</summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int IntMatch(INode node) =>
            node is IValue<int> ? 0 : -1;

        /// <summary>Determines if the given node can be cast to double IValues.</summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int DoubleMatch(INode node) =>
            node is IValue<double> ? 0 :
            node is IValue<int>    ? 1 :
            -1;

        /// <summary>Determines if the given node can be cast to the given type.</summary>
        /// <typeparam name="T">Must be a bool, int, ITrigger, or double otherwise -1 is always returned.</typeparam>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int Match<T>(INode node) {
            System.Type type = typeof(T);
            return type == typeof(IValue<bool>)   ? BoolMatch(node) :
                   type == typeof(ITrigger)       ? TriggerMatch(node) :
                   type == typeof(IValue<int>)    ? IntMatch(node) :
                   type == typeof(IValue<double>) ? DoubleMatch(node) :
                   -1;
        }

        /// <summary>Joins all the matches.</summary>
        /// <param name="values">The values to join.</param>
        /// <returns>The resulting matching or -1 if no match.</returns>
        static public int JoinMatches(params int[] values) =>
            JoinMatches(values as IEnumerable<int>);

        /// <summary>Joins all the matches.</summary>
        /// <param name="values">The values to join.</param>
        /// <returns>The resulting matching or -1 if no match.</returns>
        static public int JoinMatches(IEnumerable<int> values) {
            int sum = 0;
            foreach (int value in values) {
                if (value < 0) return -1;
                sum += value;
            }
            return sum;
        }

        /// <summary>Casts the given node to a bool IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The bool IValue or it throws an exception if it can't cast.</returns>
        static public IValue<bool> AsBool(INode node) =>
            node is IValue<bool> value ? value :
            throw new Exception("Can not cast "+node+" to IValue<bool>.");

        /// <summary>Casts the given node to a trigger.</summary>
        /// <remarks>
        /// A bool can be converted to a trigger for logic equations
        /// but normally isn't implicity cast.
        /// </remarks>
        /// <param name="node">The node to cast.</param>
        /// <returns>The bool IValue or it throws an exception if it can't cast.</returns>
        static public ITrigger AsTrigger(INode node) =>
            node is ITrigger trig ? trig :
            node is IValue<bool> value ? new OnTrue(value) :
            throw new Exception("Can not cast "+node+" to ITrigger.");

        /// <summary>Casts the given node to a int IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The int IValue or it throws an exception if it can't cast.</returns>
        static public IValue<int> AsInt(INode node) =>
            node is IValue<int> value ? value :
            throw new Exception("Can not cast "+node+" to IValue<int>.");

        /// <summary>Casts the given node to a double IValue.</summary>
        /// <param name="node">The node to cast.</param>
        /// <returns>The double IValue or it throws an exception if it can't cast.</returns>
        static public IValue<double> AsDouble(INode node) =>
            node is IValue<double> dValue ? dValue :
            node is IValue<int> iValue ? new IntToFloat(iValue) :
            throw new Exception("Can not cast "+node+" to IValue<double>.");

        /// <summary>Determines if the given node can be cast to the given type.</summary>
        /// <typeparam name="T">Must be a bool, int, ITrigger, or double otherwise an exception is thrown.</typeparam>
        /// <param name="node">The node to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public T As<T>(INode node) {
            System.Type type = typeof(T);
            return type == typeof(IValue<bool>)   ? (T)AsBool(node) :
                   type == typeof(ITrigger)       ? (T)AsTrigger(node) :
                   type == typeof(IValue<int>)    ? (T)AsInt(node) :
                   type == typeof(IValue<double>) ? (T)AsDouble(node) :
                   throw new Exception("Can not cast "+node+" to "+typeof(T)+".");
        }

        /// <summary>Gets a pretty name used for exceptions which can be thrown for invalid input.</summary>
        /// <param name="node">The node to get the name for.</param>
        /// <returns>The name for the node.</returns>
        static public string PrettyName(INode node) =>
            node is Counter        ? "counter" :
            node is Toggler        ? "toggler" :
            node is IValue<bool>   ? "bool" :
            node is IValue<int>    ? "int" :
            node is IValue<double> ? "float" :
            node is ITrigger       ? "trigger" :
            "unknown:"+node;

        /// <summary>Converts the given node into a literal.</summary>
        /// <remarks>Triggers will be returned as a bool value.</remarks>
        /// <param name="node">The node to get a literal version of.</param>
        /// <returns>The literal node or null if can't be converted.</returns>
        static public INode ToLiteral(INode node) =>
            node is IValue<bool>   nodeBool    ? new Literal<bool>(  nodeBool.Value) :
            node is IValue<int>    nodeInt     ? new Literal<int>(   nodeInt.Value) :
            node is IValue<double> nodeFloat   ? new Literal<double>(nodeFloat.Value) :
            node is ITrigger       nodeTrigger ? new Literal<bool>(  nodeTrigger.Triggered) :
            null;

        /// <summary>Determines if all the given nodes are constants.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>True if all the nodes are constants.</returns>
        static public bool IsConstant(IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                if (node is not IConstant) return false;
            }
            return true;
        }
    }
}
