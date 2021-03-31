using System.Collections.Generic;
using System.Linq;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core {

    public abstract class Nary<TIn, TResult>: ValueNode<TResult> {

        private List<IValue<TIn>> sources;

        protected Nary(IEnumerable<IValue<TIn>> parents = null, TResult value = default) : base(value) {
            this.sources = new List<IValue<TIn>>();
            this.AddParents(parents);
            this.UpdateValue();
        }

        public void AddParents(params IValue<IValue<TIn>>[] parents) =>
            this.AddParents(parents as IEnumerable<IValue<TIn>>);

        public void AddParents(IEnumerable<IValue<TIn>> parents) {
            this.sources.AddRange(parents);
            foreach (IValue<TIn> parent in parents)
                parent.AddChildren(this);
            this.UpdateValue();
        }

        public void RemoveParents(params IValue<TIn>[] parents) =>
            this.RemoveParents(parents as IEnumerable<IValue<TIn>>);

        public void RemoveParents(IEnumerable<IValue<TIn>> parents) {
            foreach (IValue<TIn> parent in parents) {
                if (this.sources.Remove(parent))
                    parent.RemoveChildren(this);
            }
            this.UpdateValue();
        }

        public override IEnumerable<INode> Parents => this.sources;

        protected abstract TResult OnEval(TIn[] values);

        protected override bool UpdateValue() =>
            this.SetNodeValue(this.OnEval(this.sources.NotNull().Values().ToArray()));

        public override string ToString() =>
            "("+NodeString(this.sources)+")";
    }
}
