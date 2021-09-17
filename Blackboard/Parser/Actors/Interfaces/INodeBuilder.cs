using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Parser.Actors.Interfaces {

    /// <summary>This is an actor to evaluate or builds a node.</summary>
    internal interface INodeBuilder: IActor {

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The literal value of this node.</returns>
        public INode Evaluate();

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Build();

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns();
    }
}
