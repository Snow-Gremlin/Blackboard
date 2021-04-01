using System.Collections.Generic;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Bases {

    public abstract class Binary<T1, T2, TResult>: ValueNode<TResult> {

        private IValue<T1> source1;
        private IValue<T2> source2;

        protected Binary(IValue<T1> source1 = null, IValue<T2> source2 = null, TResult value = default) : base(value) {
            this.Parent1 = source1;
            this.Parent2 = source2;
            this.UpdateValue();
        }

        public IValue<T1> Parent1 {
            get => this.source1;
            set {
                if (!(this.source1 is null))
                    this.source1.RemoveChildren(this);
                this.source1 = value;
                if (!(this.source1 is null))
                    this.source1.AddChildren(this);
                this.UpdateValue();
            }
        }

        public IValue<T2> Parent2 {
            get => this.source2;
            set {
                if (!(this.source2 is null))
                    this.source2.RemoveChildren(this);
                this.source2 = value;
                if (!(this.source2 is null))
                    this.source2.AddChildren(this);
                this.UpdateValue();
            }
        }

        public override IEnumerable<INode> Parents {
            get {
                if (!(this.source1 is null)) yield return this.source1;
                if (!(this.source2 is null)) yield return this.source2;
            }
        }

        protected abstract TResult OnEval(T1 value1, T2 value2);

        protected override bool UpdateValue() {
            if (this.source1 is null || this.source2 is null) return false;
            TResult value = this.OnEval(this.source1.Value, this.source2.Value);
            return this.SetNodeValue(value);
        }

        public override string ToString() =>
            "("+NodeString(this.source1)+", "+NodeString(this.source2)+")";
    }
}
