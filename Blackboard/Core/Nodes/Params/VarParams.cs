using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using S = System;

namespace Blackboard.Core.Nodes.Params {
    internal class VarParams<T>: IParams
            where T : class, IParent {

        private readonly List<T> source;

        public VarParams(IChild child, List<T> source) {
            this.Child = child;
            this.source = source;
        }

        public readonly IChild Child;

        public IEnumerable<S.Type> Types => new S.Type[1] { typeof(T) }.RepeatLast();

        public IEnumerable<IParent> Parents => this.source;
    }
}
