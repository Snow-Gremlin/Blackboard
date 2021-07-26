using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Bases {

    /// <summary>A base node for any trigger node.</summary>
    public abstract class TriggerNode: Node, ITrigger {

        /// <summary>Creates a new trigger node.</summary>
        public TriggerNode(bool provoked = false) {
            this.Provoked = provoked;
        }

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>Returns null because a trigger can not be a literal.</returns>
        public override INode ToLiteral() => null;

        /// <summary>Indicates if this trigger has been fired during a current evaluation.</summary>
        public bool Provoked { get; protected set; }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        public void Reset() => this.Provoked = false;

        /// <summary>This is called when the trigger is evaluated.</summary>
        /// <returns>True if this trigger got provoked, false if not.</returns>
        abstract protected bool UpdateTrigger();

        /// <summary>This evaluates this trigger node.</summary>
        /// <returns>This will always return all the children if provoked, or none if not provoked.</returns>
        sealed public override IEnumerable<INode> Eval() {
            this.Provoked = this.UpdateTrigger();
            return this.Provoked ? this.Children : Enumerable.Empty<INode>();
        }
    }
}
