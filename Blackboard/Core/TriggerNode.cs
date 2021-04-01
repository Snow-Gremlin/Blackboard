using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using System.Linq;

namespace Blackboard.Core {
    public abstract class TriggerNode: Node, ITrigger {

        protected TriggerNode() {
            this.Triggered = false;
        }

        public bool Triggered { get; protected set; }

        public void Reset() => this.Triggered = false;

        abstract protected bool Trigger();

        sealed public override IEnumerable<INode> Eval() {
            this.Triggered = this.Trigger();
            return this.Triggered ? this.Children : Enumerable.Empty<INode>();
        }
    }
}
