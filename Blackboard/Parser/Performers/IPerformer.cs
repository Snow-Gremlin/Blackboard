using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>The interface for all performers.</summary>
    internal interface IPerformer {

        /// <summary>Gets the type of the node which will be returned.</summary>
        public S.Type Type { get; }

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the preppers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform();
    }
}
