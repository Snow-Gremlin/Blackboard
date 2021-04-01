using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using System.Linq;

namespace Blackboard.Core.Caps {

    public class InputValue<T>: ValueNode<T>, IValueInput<T>, INamed {

        public InputValue(string name = "Input", T value = default): base(value) {
            this.Name = name;
        }

        public bool SetValue(T value) => this.SetNodeValue(value);

        public string Name { get; }

        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        protected override bool UpdateValue() => true;

        public override string ToString() => this.Name;
    }
}
