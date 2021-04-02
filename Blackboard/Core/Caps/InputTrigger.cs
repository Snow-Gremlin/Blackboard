using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System.Linq;

namespace Blackboard.Core.Caps {

    public class InputTrigger: TriggerNode, ITriggerInput {

        public InputTrigger(string name = "Input") {
            this.Name = name;
        }

        public string Name { get; }

        public void Trigger() => this.Triggered = true;

        protected override bool UpdateTrigger() => this.Triggered;

        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        public override string ToString() => this.Name;
    }
}
