using Blackboard.Core;
using Blackboard.Core.Caps;
using Blackboard.Core.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Functions {

    /// <summary>The interface for parsable function factory.</summary>
    public interface IFunction {

        /// <summary>Determines if the given nodes can be cast to bool IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int BoolMatch(params INode[] nodes) =>
            BoolMatch(nodes);

        /// <summary>Determines if the given nodes can be cast to bool IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int BoolMatch(IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<bool>) return 0;
            }
            return -1;
        }

        /// <summary>Determines if the given nodes can be cast to triggers.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int TriggerMatch(params INode[] nodes) =>
            TriggerMatch(nodes);

        /// <summary>Determines if the given nodes can be cast to triggers.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int TriggerMatch(IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                if (node is not ITrigger)     return 0;
                if (node is not IValue<bool>) return 1;
            }
            return -1;
        }

        /// <summary>Determines if the given nodes can be cast to int IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int IntMatch(params INode[] nodes) =>
            IntMatch(nodes);

        /// <summary>Determines if the given nodes can be cast to int IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int IntMatch(IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<int>) return 0;
            }
            return -1;
        }

        /// <summary>Determines if the given nodes can be cast to double IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int DoubleMatch(params INode[] nodes) =>
            DoubleMatch(nodes);

        /// <summary>Determines if the given nodes can be cast to double IValues.</summary>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int DoubleMatch(IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                if (node is not IValue<double>) return 0;
                if (node is not IValue<int>)    return 1;
            }
            return -1;
        }

        /// <summary>Determines if the given nodes can be cast to the given type.</summary>
        /// <typeparam name="T">Must be a bool, int, ITrigger, or double otherwise -1 is always returned.</typeparam>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int Match<T>(params INode[] nodes) =>
            Match<T>(nodes);

        /// <summary>Determines if the given nodes can be cast to the given type.</summary>
        /// <typeparam name="T">Must be a bool, int, ITrigger, or double otherwise -1 is always returned.</typeparam>
        /// <param name="nodes">The nodes to check.</param>
        /// <returns>Negative if it can not be cast, smaller value for closer match.</returns>
        static public int Match<T>(IEnumerable<INode> nodes) {
            System.Type type = typeof(T);
            return type == typeof(IValue<bool>)   ? BoolMatch(nodes) :
                   type == typeof(ITrigger)       ? TriggerMatch(nodes) :
                   type == typeof(IValue<int>)    ? IntMatch(nodes) :
                   type == typeof(IValue<double>) ? DoubleMatch(nodes) :
                   -1;
        }

        /// <summary>Joins all the matches.</summary>
        /// <param name="values">The values to join.</param>
        /// <returns>The resulting matching or -1 if no match.</returns>
        static public int JoinMatches(params int[] values) =>
            JoinMatches(values);

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
        /// <param name="node">The node to cast.</param>
        /// <returns>The bool IValue or it throws an exception if it can't cast.</returns>
        static public ITrigger AsTrigger(INode node) =>
            node is ITrigger     trig  ? trig :
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
            node is IValue<int>    iValue ? new IntToFloat(iValue) :
            throw new Exception("Can not cast "+node+" to IValue<double>.");

        /// <summary>Determines if the given nodes can be cast to the given type.</summary>
        /// <typeparam name="T">Must be a bool, int, ITrigger, or double otherwise -1 is always returned.</typeparam>
        /// <param name="nodes">The nodes to check.</param>
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

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        int Match(INode[] nodes);

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        INode Build(INode[] nodes);
    }
}
