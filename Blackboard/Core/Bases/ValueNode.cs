using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using System.Linq;

namespace Blackboard.Core.Bases {
    public abstract class ValueNode<T>: Node, IValue<T> {

        protected ValueNode(T value = default) {
            this.Value = value;
        }

        public T Value { get; private set; }

        protected bool SetNodeValue(T value) {
            if (EqualityComparer<T>.Default.Equals(this.Value, value)) return false;
            this.Value = value;
            return true;
        }

        abstract protected bool UpdateValue();

        sealed public override IEnumerable<INode> Eval() =>
            this.UpdateValue() ? this.Children : Enumerable.Empty<INode>();
    }
}
