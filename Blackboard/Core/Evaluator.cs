using Blackboard.Core.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Blackboard.Core {

    /// <summary>This is used to propogate the changes to inputs through the blackboard nodes.</summary>
    static public class Evaluator {

        /// <summary>Updates and propogates the changes from the givne inputs through the blackboard nodes.</summary>
        /// <param name="log">An optional log to keep track of which nodes and what order they are evaluated.</param>
        /// <param name="touchedInput">The input nodes which have been modified.</param>
        static public void Eval(TextWriter log, params IInput[] touchedInput) =>
            Eval(log, touchedInput as IEnumerable<IInput>);

        /// <summary>Updates and propogates the changes from the givne inputs through the blackboard nodes.</summary>
        /// <param name="log">An optional log to keep track of which nodes and what order they are evaluated.</param>
        /// <param name="touchedInput">The input nodes which have been modified.</param>
        static public void Eval(TextWriter log = null, IEnumerable<IInput> touchedInput = null) {
            LinkedList<INode> pending = new LinkedList<INode>();
            LinkedList<ITrigger> needsReset = new LinkedList<ITrigger>();
            pending.SortInsertUnique(touchedInput);
            while (pending.Count > 0) {
                INode node = pending.TakeFirst();
                if (!(log is null))
                    log.WriteLine("Eval("+node.Depth+"): "+node);
                pending.SortInsertUnique(node.Eval());
                if (node is ITrigger)
                    needsReset.AddLast(node as ITrigger);
            }
            foreach (ITrigger trigger in needsReset)
                trigger.Reset();
        }
    }
}
