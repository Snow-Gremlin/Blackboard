using System.Collections.Generic;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core {

    public abstract class Unary<T1, TResult>: ValueNode<TResult> {

        private IValue<T1> source;

        protected Unary(IValue<T1> source = null, TResult value = default) : base(value) {
            this.Parent = source;
            this.UpdateValue();
        }

        public IValue<T1> Parent {
            get => this.source;
            set {
                if (!(this.source is null))
                    this.source.RemoveChildren(this);
                this.source = value;
                if (!(this.source is null))
                    this.source.AddChildren(this);
                this.UpdateValue();
            }
        }

        public override IEnumerable<INode> Parents {
            get {
                if (!(this.source is null)) yield return this.source;
            }
        }

        protected abstract TResult OnEval(T1 value);

        protected override bool UpdateValue() {
            if (this.source is null) return false;
            TResult value = this.OnEval(this.source.Value);
            return this.SetNodeValue(value);
        }

        public override string ToString() =>
            "("+NodeString(this.source)+")";
    }
}
