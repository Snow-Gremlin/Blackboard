using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>Will evaluate the given value into a constant value.</summary>
    sealed internal class Evaluator: IPerformer {

        /// <summary>The value to evaluate.</summary>
        public readonly IPerformer Value;

        /// <summary>Creates a new evaluator.</summary>
        /// <param name="value">The performer to get the node to evaluate.</param>
        public Evaluator(IPerformer value) {
            // TODO: Need to add a passthough for already constant values.
            this.Value = value;
        }

        /// <summary>Gets the type of the node which will be returned.</summary>
        /// <remarks>This hasn't been converted into the constant type but should match close enough.</remarks>
        public S.Type Type => this.Value.Type;

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the prepers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform() => (this.Value.Perform() as IConstantable)?.ToConstant();
    }
}
