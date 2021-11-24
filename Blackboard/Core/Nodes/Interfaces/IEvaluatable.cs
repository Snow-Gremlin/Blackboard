using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates a node which can be evaludated.</summary>
    public interface IEvaluatable: INode {

        /// <summary>The depth in the graph from the furthest parent evaluatable of this node.</summary>
        /// <remarks>This is used to determine the order of evaluation.</remarks>
        public int Depth { get; }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of nodes (should be any evaluatable children of this node)
        /// that should be updated based on the results of this node's update.
        /// If this evaluation made no change then no node should be returned.
        /// </returns>
        public IEnumerable<IEvaluatable> Eval();
    }
}
