using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser {


    // TODO: CHECK ALL COMMENTS

    /// <summary>This is the complete set of performers being prepared.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes vertually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed public class Formula {

        /// <summary>The driver for the Blackboard this formula belongs too.</summary>
        private readonly Driver driver;

        /// <summary>The collection of performers for this formula.</summary>
        private readonly IPerformer[] performers;

        /// <summary>Creates a new formula to perform changes to the given driver.</summary>
        internal Formula(Driver driver, IEnumerable<IPerformer> performers) {
            this.driver = driver;
            this.performers = performers.ToArray();
        }

        /// <summary>Performs all pending actions.</summary>
        public void Perform() =>
            this.driver.Touch(this.performers.Select(perf => perf.Perform()).OfType<IEvaluatable>());

        /// <summary>Gets the formula debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() {
            const string indent = "  ";
            return "[\n" + indent + this.performers.Strings().Indent(indent).Join(",\n" + indent) + "]";
        }
    }
}
