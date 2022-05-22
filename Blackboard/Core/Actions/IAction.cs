using Blackboard.Core.Inspect;

namespace Blackboard.Core.Actions {

    /// <summary>This represents an action which can be performed as part of a formula.</summary>
    public interface IAction {

        /// <summary>This will perform the action.</summary>
        /// <remarks>
        /// The given slate MUST be the slate this action was created for
        /// since several of the action types will hold onto nodes from a specific slate.
        /// </remarks>
        /// <param name="slate">The slate for this action.</param>
        /// <param name="result">The result being created and added to.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, Result result, Logger logger = null);

        // TODO: Add a method which can validate the action and check if it can still be performed.
        //       For things like define, it would check if the value hasn't been defined so that
        //       when it is performed the already-exists exception is thrown.
        //       Optionally Log any warnings or errors but return a boolean indicating if it can be performed.
    }
}
