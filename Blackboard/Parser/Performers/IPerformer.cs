using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Performers {

    /// <summary>The interface for all performers.</summary>
    internal interface IPerformer {

        /// <summary>The location the thing being performed was defind in the code being parsed.</summary>
        public Location Location { get; }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <remarks>This may not be set until after Prepare is called.</remarks>
        /// <returns>The returned value type.</returns>
        public System.Type ReturnType { get; }

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if the preper is working correctly.
        /// If the performer does throw an exception the preper should be fixed to prevent this.
        /// </remarks>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform(Formula formula);
    }
}
