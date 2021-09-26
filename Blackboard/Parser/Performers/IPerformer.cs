using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>The interface for all actors.</summary>
    internal interface IPerformer {

        /// <summary>The location this actor was defind in the code being parsed.</summary>
        public Location Location { get; }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <remarks>This may not be set until after Prepare is called.</remarks>
        /// <returns>The returned value type.</returns>
        public Type Returns();

        /// <summary>This will build or evaluate the actor and apply it to Blackboard.</summary>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform();
    }
}
