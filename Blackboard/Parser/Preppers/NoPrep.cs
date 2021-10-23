using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;

namespace Blackboard.Parser.Preppers {

    /// <summary>This is for storing a performer which needs to be returned directly in a parent performer.</summary>
    sealed internal class NoPrep: IPrepper {

        /// <summary>Creates a new non-prepper prepper.</summary>
        /// <param name="node">The node that should be passed through.</param>
        public NoPrep(INode node) {
            this.Performer = new NodeHold(node);
        }

        /// <summary>Creates a new non-prepper prepper.</summary>
        /// <param name="perf">The performer to return when prepared.</param>
        public NoPrep(IPerformer perf) {
            this.Performer = perf;
        }

        /// <summary>The performer to return when prepared.</summary>
        public IPerformer Performer;

        /// <summary>Does no preparation and just returns the held performer.</summary>
        /// <param name="formula">Not Used.</param>
        /// <param name="reduce">Indicates if the returned node should be reduced or not.</param>
        /// <returns>The performer being held.</returns>
        public IPerformer Prepare(Formula formula, bool reduce = false) =>
            Reducer.Wrap(this.Performer, reduce);

        /// <summary>Gets the prepper debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => "NoPrep("+this.Performer+")";
    }
}
