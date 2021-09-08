using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core;

namespace Blackboard.Parser.Actors {

    /// <summary>The interface for all actors.</summary>
    internal interface IActor {

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The literal value of this node.</returns>
        INode Evaluate();

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        INode BuildNode();

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        Type Returns();
    }
}
