﻿using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has a single parent as the source of the value.</summary>
    /// <typeparam name="T">The type of the parent's value for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    public class Function<T, TReturn>: FuncDef<TReturn>
        where T : class, INode
        where TReturn : class, INode {

        /// <summary>Indicates that at least one argument must not be a cast.</summary>
        readonly private bool needOneNoCast;

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T, TReturn> hndl;

        /// <summary>Creates a new singular node factory.</summary>
        /// <param name="hndl">The factory handle.</param>
        /// <param name="needOneNoCast">Indicates that at least one argument must not be a cast.</param>
        public Function(S.Func<T, TReturn> hndl, bool needOneNoCast = false) {
            this.hndl = hndl;
            this.needOneNoCast = needOneNoCast;

            if (Type.FromType<T>() is null) throw Exception.UnknownFunctionParamType(typeof(T), "T1");
        }

        /// <summary>Determines how closely matching the given nodes are for this match.</summary>
        /// <param name="types">The input types to match against the function signatures with.</param>
        /// <returns>The matching results for this function.</returns>
        public override FuncMatch Match(Type[] types) =>
            types.Length != 1 ? FuncMatch.NoMatch :
            FuncMatch.Create(this.needOneNoCast, Type.Match<T>(types[0]));

        /// <summary>Builds and returns the function object.</summary>
        /// <remarks>Before this is called, Match must have been possible.</remarks>
        /// <param name="nodes">The nodes as parameters to the function.</param>
        /// <returns>The new function.</returns>
        public override INode Build(INode[] nodes) {
            T node = Type.Implicit<T>(nodes[0]);
            return this.hndl(node);
        }
    }
}