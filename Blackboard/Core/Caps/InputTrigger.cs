using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using System.Linq;

namespace Blackboard.Core.Caps {

    public class InputTrigger: TriggerNode, ITriggerInput, INamed {

        public InputTrigger(string name = "Input") {
            this.Name = name;
        }

        public string Name { get; }

        protected override bool Trigger() => true;

        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        public override string ToString() => this.Name;
    }
}
