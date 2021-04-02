using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;

namespace Blackboard.Core.Caps {

    public class Select<T>: Ternary<bool, T, T, T> {

        protected override T OnEval(bool value1, T value2, T value3) => value1 ? value2 : value3;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => "Select"+base.ToString();
    }
}
