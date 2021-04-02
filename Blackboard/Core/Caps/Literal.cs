using System.Collections.Generic;
using Blackboard.Core.Interfaces;
using Blackboard.Core.Bases;
using System.Linq;

namespace Blackboard.Core.Caps {

    public class Literal<T>: ValueNode<T> {

        public bool SetValue(T value) => this.SetNodeValue(value);

        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        protected override bool UpdateValue() => true;

        /// <summary>Gets the string for this node.</summary>
        /// <returns>The debug string for this node.</returns>
        public override string ToString() => this.Value.ToString();
    }
}
