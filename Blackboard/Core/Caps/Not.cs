using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    /// <summary>Performs a boolean NOT of one boolean parent.</summary>
    /// <see cref="https://mathworld.wolfram.com/NOT.html"/>
    public class Not: Unary<bool, bool> {

        protected override bool OnEval(bool value) => !value;

        public override string ToString() => "Not"+base.ToString();
    }
}
