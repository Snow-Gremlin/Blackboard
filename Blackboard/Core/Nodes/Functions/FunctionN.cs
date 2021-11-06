using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has abritrary parents as the source of the value.</summary>
    /// <typeparam name="Tn">The type of the parents' values for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    sealed public class FunctionN<Tn, TReturn>: FuncDef<TReturn>
        where Tn : class, INode
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        private readonly S.Func<IEnumerable<Tn>, TReturn> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        /// <param name="passOne">Indicates if there is only one argument for a new node, return the argument.</param>
        /// <param name="min">The minimum number of required nodes.</param>
        public FunctionN(S.Func<IEnumerable<Tn>, TReturn> hndl, bool needsOneNoCast = false, bool passOne = true, int min = 1) :
            base(min, int.MaxValue, needsOneNoCast, passOne, Type.FromType<Tn>()) {
            this.hndl = hndl;
        }

        /// <summary>This will implicity cast the given parameter,</summary>
        /// <param name="t">The type to get the cast from.</param>
        /// <param name="node">The node to cast.</param>
        /// <returns>The resulting parameter in the expected type.</returns>
        static private Tn castParam(Type t, INode node) {
            INode cast = t.Implicit(node);
            return cast is Tn result ? result :
                throw new Exception("Error casting parameter").
                    With("Type", typeof(Tn)).
                    With("Implicit", t).
                    With("Result", cast);
        }

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public override INode Build(INode[] nodes) {
            Type t = Type.FromType<Tn>();
            return this.PassthroughOne && nodes.Length == 1 ? t.Implicit(nodes[0]) :
                this.hndl(nodes.Select((node) => castParam(t, node)).ToArray());
        }

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) =>
            this.TypeName + "<" + Type.FromType<TReturn>() + ">(" + Type.FromType<Tn>() + "..." + ")";
    }
}
