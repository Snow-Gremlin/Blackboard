using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has three parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="T3">The type of the thrid parent's value for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>ss
    sealed public class Function<T1, T2, T3, TReturn>: FuncDef<TReturn>
        where T1 : class, INode
        where T2 : class, INode
        where T3 : class, INode
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T1, T2, T3, TReturn> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        public Function(S.Func<T1, T2, T3, TReturn> hndl, bool needsOneNoCast = false) :
            base(needsOneNoCast, false, Type.FromType<T1>(), Type.FromType<T2>(), Type.FromType<T3>()) {
            this.hndl = hndl;
        }

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public override INode Build(INode[] nodes) {
            T1 node1 = Type.Implicit<T1>(nodes[0]);
            T2 node2 = Type.Implicit<T2>(nodes[1]);
            T3 node3 = Type.Implicit<T3>(nodes[2]);
            return this.hndl(node1, node2, node3);
        }

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) =>
            this.TypeName + "<" + Type.FromType<TReturn>() + ">(" + Type.FromType<T1>() + ", " + Type.FromType<T2>() + ", " + Type.FromType<T3>() + ")";
    }
}
