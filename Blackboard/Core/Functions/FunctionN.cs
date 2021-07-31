using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has abritrary parents as the source of the value.</summary>
    /// <typeparam name="Tn">The type of the parents' values for this node.</typeparam>
    public class FunctionN<Tn>: IFunction
        where Tn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<IEnumerable<Tn>, INode> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        public FunctionN(S.Func<IEnumerable<Tn>, INode> hndl) {
            this.hndl = hndl;

            if (Type.FromType<Tn>() is null) throw Exception.UnknownFunctionParamType(typeof(Tn), "Tn");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="nodes">The nodes to match against.</param>
        /// <returns>The closest match is lower but not negatve.</returns>
        public int Match(INode[] nodes) {
            Type t = Type.FromType<Tn>();
            return IFunction.Join(nodes.Select((node) => t.Match(Type.TypeOf(node))));
        }

        /// <summary>Builds and returns the function object.</summary>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            Type t = Type.FromType<Tn>();
            return this.hndl(nodes.Select((node) => t.Implicit(node) as Tn));
        }
    }
}
