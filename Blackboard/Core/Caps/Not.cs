using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean NOT of one boolean parent.</summary>
    /// <see cref="https://mathworld.wolfram.com/NOT.html"/>
    public class Not: Unary<bool, bool> {

        public Not(IValue<bool> source = null, bool value = default) :
            base(source, value) { }

        protected override bool OnEval(bool value) => !value;

        public override string ToString() => "Not"+base.ToString();
    }
}
