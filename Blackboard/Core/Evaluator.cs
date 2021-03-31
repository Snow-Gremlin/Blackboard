using System.Collections.Generic;
using System.IO;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core {

    static public class Evaluator {

        static public void Eval(TextWriter log, params IInput[] touchedInput) =>
            Eval(log, touchedInput as IEnumerable<IInput>);

        static public void Eval(TextWriter log = null, IEnumerable<IInput> touchedInput = null) {
            LinkedList<INode> pending = new LinkedList<INode>();
            pending.SortInsertUnique(touchedInput);
            while (pending.Count > 0) {
                INode node = pending.TakeFirst();
                if (!(log is null))
                    log.WriteLine("Eval("+node.Depth+"): "+node);
                pending.SortInsertUnique(node.Eval());
            }
        }
    }
}
