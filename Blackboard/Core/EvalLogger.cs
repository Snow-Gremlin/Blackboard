using Blackboard.Core.Nodes.Interfaces;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core {

    /// <summary>This is a logger used for debugging the evaluation of nodes in the driver.</summary>
    public class EvalLogger {

        /// <summary>The string buffer to write to.</summary>
        private StringWriter fout;

        /// <summary>Creates a new evaluation logger.</summary>
        public EvalLogger() {
            this.fout = new StringWriter();
        }

        /// <summary>Clears this logger.</summary>
        public void Clear() => this.fout = new StringWriter();

        /// <summary>This will write to this log followed by a new line.</summary>
        /// <param name="text">The test to write.</param>
        public void Log(string text) => this.fout.WriteLine(text);

        /// <summary>This is called when an evaluation is started.</summary>
        /// <param name="pending">The initial nodes pending evaluation.</param>
        virtual public void StartEval(IEnumerable<INode> pending) =>
            this.Log("Start(Pending: "+pending.Count()+")");

        /// <summary>This is called when an evaluation has ended.</summary>
        /// <param name="provoked">The triggers which where provoked during this evaluation.</param>
        virtual public void EndEval(IEnumerable<ITrigger> provoked) =>
            this.Log("End(Provoked: "+provoked.Count()+")");

        /// <summary>This is called when a node is evaluated.</summary>
        /// <param name="node">The node about to be evaluated.</param>
        virtual public void Eval(INode node) =>
            this.Log("  Eval("+node.Depth+"): "+node);

        /// <summary>This is called when a node has been evaluated.</summary>
        /// <param name="children">The resulting children from the evaluation.</param>
        /// <remarks>By default this will not log anything.</remarks>
        virtual public void EvalResult(IEnumerable<INode> children) { }

        /// <summary>This will get the logs.</summary>
        /// <returns>The logs which have been written.</returns>
        public override string ToString() {
            this.fout.Flush();
            return this.fout.ToString();
        }
    }
}
