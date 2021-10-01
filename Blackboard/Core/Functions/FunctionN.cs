using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Functions {

    /// <summary>This is the factory for a node which has abritrary parents as the source of the value.</summary>
    /// <typeparam name="Tn">The type of the parents' values for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    public class FunctionN<Tn, TReturn>: IFunction
        where Tn : class, INode
        where TReturn : class, INode {

        /// <summary>Indicates that at least one argument must not be a cast.</summary>
        readonly private bool needOneNoCast;

        /// <summary>Indicates if there is only one argument for a new node, return the argument.</summary>
        private readonly bool passOne;

        /// <summary>The minimum required nodes for the new node's arguments.</summary>
        private readonly int min;

        /// <summary>The factory for creating the node.</summary>
        private readonly S.Func<IEnumerable<Tn>, TReturn> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needOneNoCast">Indicates that at least one argument must not be a cast.</param>
        /// <param name="passOne">Indicates if there is only one argument for a new node, return the argument.</param>
        /// <param name="min">The minimum number of required nodes.</param>
        public FunctionN(S.Func<IEnumerable<Tn>, TReturn> hndl, bool needOneNoCast = false, bool passOne = true, int min = 1) {
            this.hndl = hndl;
            this.needOneNoCast = needOneNoCast;
            this.passOne = passOne;
            this.min = min;

            if (Type.FromType<Tn>() is null) throw Exception.UnknownFunctionParamType(typeof(Tn), "Tn");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public FuncMatch Match(Type[] types) {
            if (types.Length < this.min) return FuncMatch.NoMatch;
            Type t = Type.FromType<Tn>();
            return FuncMatch.Create(this.needOneNoCast, types.Select(t.Match));
        }

        /// <summary>Returns the type that would be return if built.</summary>
        /// <returns>The type which would be returned.</returns>
        public S.Type Returns() => typeof(TReturn);

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public INode Build(INode[] nodes) {
            Type t = Type.FromType<Tn>();
            return this.passOne && nodes.Length == 1 ? t.Implicit(nodes[0]) :
                this.hndl(nodes.Select((node) => t.Implicit(node) as Tn).ToArray());
        }
    }
}
