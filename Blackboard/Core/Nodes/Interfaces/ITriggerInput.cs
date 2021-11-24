using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>The interface for an input trigger.</summary>
    public interface ITriggerInput: IInput, ITrigger {

        /// <summary>Provokes this trigger so that this node is provoked during the next evaluation.</summary>
        /// <param name="value">True will provoke, false will reset the trigger.</param>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        /// <returns>
        /// The set of nodes (should be any evaluatable children of this node)
        /// that should be updated based on the results of this node's update.
        /// If this evaluation made no change then no node should be returned.
        /// </returns>
        public IEnumerable<IEvaluatable> Provoke(bool value = true);
    }
}
