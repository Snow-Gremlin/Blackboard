using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for any trigger node.</summary>
    public abstract class TriggerNode: EvalAdopter, ITriggerAdopter {

        /// <summary>Creates a new trigger node.</summary>
        public TriggerNode(bool provoked = false) {
            this.Provoked = provoked;
        }

        /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
        public bool Provoked { get; protected set; }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        public void Reset() => this.Provoked = false;

        /// <summary>This is called when the trigger is evaluated.</summary>
        /// <returns>True if this trigger got provoked, false if not.</returns>
        abstract protected bool UpdateTrigger();

        /// <summary>This evaluates this trigger node.</summary>
        /// <returns>This will always return all the children if provoked, or none if not provoked.</returns>
        sealed public override IEnumerable<IEvaluatable> Eval() {
            this.Provoked = this.UpdateTrigger();
            return this.Provoked ? this.Children.OfType<IEvaluatable>() : Enumerable.Empty<IEvaluatable>();
        }

        /// <summary>This always returns false because triggers aren't constant but can be converted to one.</summary>
        public virtual bool IsConstant => false;

        /// <summary>Converts this node to a constant trigger.</summary>
        /// <returns>The constant trigger carrying the provoked condition.</returns>
        public virtual IConstant ToConstant() => new ConstTrigger(this.Provoked);

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.PrettyString("", 0);
    }
}
