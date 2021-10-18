using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Parser.Performers {

    /// <summary>Will reduce the given value into a constant value.</summary>
    sealed internal class Reducer: IPerformer {

        /// <summary>
        /// If the given value can be reduced to a constant then it will
        /// be put inside a reducer, otherwise it will be returned.
        /// </summary>
        /// <param name="value">The value to reduce.</param>
        /// <param name="reduce">This can be set to false to always pass through the value without checking it.</param>
        /// <returns>The value by itself or within a reducer.</returns>
        static public IPerformer Wrap(IPerformer value, bool reduce = true) =>
            reduce && value.Type.IsAssignableTo(typeof(IConstantable)) ? new Reducer(value) : value;

        /// <summary>The value to reduce.</summary>
        public readonly IPerformer Value;

        /// <summary>Creates a new reducer.</summary>
        /// <param name="value">The performer to get the node to reduce.</param>
        private Reducer(IPerformer value) {
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
