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

        /// <summary>Indicates that at least one argument must not be a cast.</summary>
        readonly private bool needOneNoCast;

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T1, T2, T3, TReturn> hndl;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needOneNoCast">Indicates that at least one argument must not be a cast.</param>
        public Function(S.Func<T1, T2, T3, TReturn> hndl, bool needOneNoCast = false) {
            this.hndl = hndl;
            this.needOneNoCast = needOneNoCast;

            if (Type.FromType<T1>() is null) throw Exceptions.UnknownFunctionParamType<T1>("T1");
            if (Type.FromType<T2>() is null) throw Exceptions.UnknownFunctionParamType<T2>("T2");
            if (Type.FromType<T3>() is null) throw Exceptions.UnknownFunctionParamType<T3>("T3");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>v
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public override FuncMatch Match(Type[] types) =>
            types.Length != 3 ? FuncMatch.NoMatch :
            FuncMatch.Create(this.needOneNoCast, Type.Match<T1>(types[0]), Type.Match<T2>(types[1]), Type.Match<T3>(types[2]));

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

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Function";

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="scopeName">The name of this node from a parent namespace or empty for no name.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(string scopeName = "", int nodeDepth = int.MaxValue) {
            string name = string.IsNullOrEmpty(scopeName) ? this.TypeName : scopeName;
            return name + "<" + Type.FromType<TReturn>() + ">(" + Type.FromType<T1>() + ", " + Type.FromType<T2>() + ", " + Type.FromType<T3>() + ")";
        }
    }
}
