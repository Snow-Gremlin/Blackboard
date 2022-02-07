using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Params {
    internal interface IParams {
        public IEnumerable<S.Type> Types { get; }
        public IEnumerable<IParent> Parents { get; }
    }
}
