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
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, ILogger logger = null);
    }
}
