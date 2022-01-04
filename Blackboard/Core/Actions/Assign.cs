using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>This is an action that will assign an input node to a data.</summary>
    /// <typeparam name="T">The type of data for the input node.</typeparam>
    sealed public class Assign<T>: IAssign
        where T : IData {

        /// <summary>
        /// Creates an assignment from the given nodes after first checking
        /// that the nodes can be used in this type of assignment.
        /// </summary>
        /// <param name="loc">The location that this provoke was created.</param>
        /// <param name="target">The target node to assign to.</param>
        /// <param name="value">The value to assign to the given target.</param>
        /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
        /// <returns>The assignment action.</returns>
        static public Assign<T> Create(Location loc, INode target, INode value, IEnumerable<INode> allNewNodes) =>
            (target is IValueInput<T> input) && (value is IValue<T> data) ?
            new Assign<T>(input, data, allNewNodes) :
            throw new Exception("Unexpected node types for assignment.").
                With("Location", loc).
                With("Type",     typeof(T)).
                With("Target",   target).
                With("Value",    value);

        /// <summary>The target input node to set the value of.</summary>
        private readonly IValueInput<T> target;

        /// <summary>The data node to get the data to assign.</summary>
        private readonly IValue<T> value;

        /// <summary>
        /// This is a subset of all the node for the value which need to be pended
        /// for evaluation in order to perform this assignment.
        /// </summary>
        private readonly IEvaluable[] needPending;

        /// <summary>Creates a new assignment.</summary>
        /// <param name="target">The input node to assign.</param>
        /// <param name="value">The node to get the value from.</param>
        /// <param name="allNewNodes">All the nodes which are new children of the value.</param>
        public Assign(IValueInput<T> target, IValue<T> value, IEnumerable<INode> allNewNodes) {

            // TODO: Need to validate these nodes, value, etc is ready for this type of action.

            this.target = target;
            this.value  = value;

            // Pre-sort the evaluable nodes.
            LinkedList<IEvaluable> nodes = new();
            nodes.SortInsertUnique(allNewNodes.NotNull().OfType<IEvaluable>());
            this.needPending = nodes.ToArray();
        }

        /// <summary>The target input node to set the value of.</summary>
        public IInput Target => this.target;

        /// <summary>The data node to get the data to assign.</summary>
        public IDataNode Value => this.value;

        /// <summary>All the nodes which are new children of the node to write.</summary>
        public IReadOnlyList<IEvaluable> NeedPending => this.needPending;

        /// <summary>This will perform the action.</summary>
        /// <param name="slate">The slate for this action.</param>
        /// <param name="result">The result being created and added to.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, Result result, ILogger logger = null) {
            logger?.Log("Assign: {0}", this);
            slate.PendEval(this.needPending);
            slate.PerformEvaluation(logger?.Sub);
            slate.SetValue(this.value.Value, this.target);
            logger?.Log("Assign Done {0}", this.target);
        }

        /// <summary>Gets a human readable string for this assignment.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
