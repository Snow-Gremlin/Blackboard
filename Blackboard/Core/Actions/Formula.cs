using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Actions {

    /// <summary>This is a collection of actions pending to perform on the Blackboard slate.</summary>
    sealed public class Formula {

        /// <summary>The collection of performers for this formula.</summary>
        private readonly IAction[] actions;

        /// <summary>Creates a new formula to perform changes to the slate.</summary>
        /// <param name="slate">The slate that these actions were created for.</param>
        /// <param name="actions">The actions that this formula will perform.</param>
        public Formula(Slate slate, params IAction[] actions) :
            this(slate, actions as IEnumerable<IAction>) { }

        /// <summary>Creates a new formula to perform changes to the slate.</summary>
        /// <param name="slate">The slate that these actions were created for.</param>
        /// <param name="actions">The actions that this formula will perform.</param>
        public Formula(Slate slate, IEnumerable<IAction> actions) {
            this.actions = actions.NotNull().ToArray();
            this.Slate = slate;
        }

        /// <summary>The slate that this formula was built for and will be run on.</summary>
        public readonly Slate Slate;

        /// <summary>The actions for this formula.</summary>
        public IReadOnlyList<IAction> Actions => this.actions;

        /// <summary>Performs all the actions for this formula.</summary>
        /// <param name="logger">The optional logger to debug with.</param>
        public void Perform(ILogger logger = null) {
            logger?.Log("Formula:");
            ILogger sub = logger?.Sub;
            this.actions.Foreach(action => action.Perform(this.Slate, sub));
        }

        /// <summary>Gets a human readable string for this formula as all internal actions on different lines.</summary>
        /// <returns>The human readable string for debugging.</returns>
        public override string ToString() => Stringifier.Simple(this);
    }
}
