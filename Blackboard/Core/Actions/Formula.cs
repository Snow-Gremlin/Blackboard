using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>This is a collection of actions to perform on the Blackboard slate.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes virtually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed public class Formula: IAction {

        /// <summary>This gets all the actions from the given action.</summary>
        /// <param name="action">The action to get all inner actions from.</param>
        /// <returns>Either the given action or all the inner actions from a function.</returns>
        static private IEnumerable<IAction> allActions(IAction action) {
            if (action is null) yield break;
            if (action is Formula formula) {
                foreach (IAction inner in formula.Actions.Select(allActions).Expand().NotNull())
                    yield return inner;
            } else yield return action;
        }

        /// <summary>The collection of performers for this formula.</summary>
        private readonly IAction[] actions;

        /// <summary>Creates a new formula to perform changes to the slate.</summary>
        public Formula(params IAction[] actions) :
            this(actions as IEnumerable<IAction>) { }

        /// <summary>Creates a new formula to perform changes to the slate.</summary>
        public Formula(IEnumerable<IAction> actions) =>
            this.actions = actions.Select(allActions).Expand().NotNull().ToArray();

        /// <summary>The actions for this formula.</summary>
        public IReadOnlyList<IAction> Actions => this.actions;

        /// <summary>Performs all the actions for this formula.</summary>
        /// <remarks>
        /// The given slate MUST be the slate this formula was created for
        /// since several of these actions will hold onto nodes from a specific slate.
        /// </remarks>
        /// <param name="slate">The slate for this formula.</param>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(Slate slate, ILogger logger = null) {
            logger?.Log("Formula:");
            ILogger sub = logger?.Sub;
            this.actions.Foreach(action => action.Perform(slate, sub));
        }

        /// <summary>Gets a human readable string for this formula as all internal actions on different lines.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
