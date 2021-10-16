using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is for storing a performer which needs to be returned directly in a parent performer.</summary>
    sealed internal class NoPrep: IPreper {

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="node">The node that should be passed through.</param>
        public NoPrep(INode node) {
            this.Performer = new NodeHold(node);
        }

        /// <summary>Creates a new non-preper preper.</summary>
        /// <param name="perf">The performer to return when prepared.</param>
        public NoPrep(IPerformer perf) {
            this.Performer = perf;
        }

        /// <summary>The performer to return when prepared.</summary>
        public IPerformer Performer;

        /// <summary>Does no preperation and just returns the held performer.</summary>
        /// <param name="formula">Not Used.</param>
        /// <param name="evaluate">Not Used.</param>
        /// <returns>The performer being held.</returns>
        public IPerformer Prepare(Formula formula, bool evaluate = false) => this.Performer;
    }
}
