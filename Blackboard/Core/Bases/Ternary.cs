using System.Collections.Generic;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Bases {

    public abstract class Ternary<T1, T2, T3, TResult>: ValueNode<TResult> {

        private IValue<T1> source1;
        private IValue<T2> source2;
        private IValue<T3> source3;

        protected Ternary(IValue<T1> source1 = null, IValue<T2> source2 = null,
            IValue<T3> source3 = null, TResult value = default) : base(value) {
            this.Parent1 = source1;
            this.Parent2 = source2;
            this.Parent3 = source3;
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

        public IValue<T3> Parent3 {
            get => this.source3;
            set {
                if (!(this.source3 is null))
                    this.source3.RemoveChildren(this);
                this.source3 = value;
                if (!(this.source3 is null))
                    this.source3.AddChildren(this);
                this.UpdateValue();
            }
        }

        public override IEnumerable<INode> Parents {
            get {
                if (!(this.source1 is null)) yield return this.source1;
                if (!(this.source2 is null)) yield return this.source2;
                if (!(this.source3 is null)) yield return this.source3;
            }
        }

        protected abstract TResult OnEval(T1 value1, T2 value2, T3 value3);

        protected override bool UpdateValue() {
            if (this.source1 is null || this.source2 is null) return false;
            TResult value = this.OnEval(this.source1.Value, this.source2.Value, this.source3.Value);
            return this.SetNodeValue(value);
        }

        public override string ToString() =>
            "("+NodeString(this.source1)+", "+NodeString(this.source2)+", "+NodeString(this.source3)+")";
    }
}
