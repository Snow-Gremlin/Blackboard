using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Actors {

    /// <summary>The interface for all actors.</summary>
    /// <remarks>
    /// Actors are ... #TODO: FINISH
    /// 
    /// </remarks>
    internal interface IActor {

        INode CreateNode();

        INode Evaluate();
    }
}
