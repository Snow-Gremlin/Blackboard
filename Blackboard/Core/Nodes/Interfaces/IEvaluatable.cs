using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Interfaces {

    /// <summary>Indicates a node which can be evaludated.</summary>
    public interface IEvaluatable: INode {

        /// <summary>The depth in the graph from the furthest input of this node.</summary>
        /// <remarks>This is used to determine the order of evaluation.</remarks>
        int Depth { get; }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of children that should be updated based on the results of this update.
        /// If this evaluation made no change then typically no children will be returned.
        /// Usually the entire set of children are returned on change, but it is not required.
        /// </returns>
        IEnumerable<IEvaluatable> Eval();
    }
}
