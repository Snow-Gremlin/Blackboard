using Blackboard.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>This is the complete set of performers being prepared.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes virtually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed public class Formula: IAction {

        /// <summary>The collection of performers for this formula.</summary>
        private readonly IAction[] actions;

        /// <summary>Creates a new formula to perform changes to the given driver.</summary>
        public Formula(params IAction[] actions) :
            this(actions as IEnumerable<IAction>) { }

        /// <summary>Creates a new formula to perform changes to the given driver.</summary>
        public Formula(IEnumerable<IAction> actions) =>
            this.actions = expandActions(actions).ToArray();

        /// <summary>This will expand any formulas in the given actions so that </summary>
        /// <param name="actions">The actions to expand.</param>
        /// <returns>All the expanded formulas and actions.</returns>
        static private IEnumerable<IAction> expandActions(IEnumerable<IAction> actions) {
            foreach (IAction action in actions.NotNull()) {
                if (action is Formula form) {
                    foreach (IAction child in expandActions(form.actions))
                        yield return child;
                } else yield return action;
            }
        }

        /// <summary>Performs all the actions for this formula.</summary>
        public void Perform(Driver driver) =>
            this.actions.Foreach(action => action.Perform(driver));
    }
}
