using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Params {
    public class Params: IParams {
        private interface ISingleParam {
            public S.Type Types { get; }
            public IParent Parents { get; set; }
        }

        private class SingleParam<T>: ISingleParam
                where T : class, IParent {

            private readonly S.Func<T> getParent;

            private readonly S.Action<T> setParent;

            public SingleParam(S.Func<T> getParent, S.Action<T> setParent) {
                this.getParent = getParent;
                this.setParent = setParent;
            }

            public S.Type Types => typeof(T);

            public IParent Parents {
                get => this.getParent();
                set => this.setParent(value as T);
            }
        }

        private List<ISingleParam> parameters;

        public Params(IChild child) {
            this.Child = child;
            this.parameters = new List<ISingleParam>();
        }

        public readonly IChild Child;

        public IEnumerable<S.Type> Types => this.parameters.Select((p) => p.Types);

        public IEnumerable<IParent> Parents => this.parameters.Select((p) => p.Parents);

        internal void With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent =>
            this.parameters.Add(new SingleParam<T>(getParent, setParent));
    }
}
