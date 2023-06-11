using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Types;
using S = System;

namespace Blackboard.Core.Nodes.Functions {

    /// <summary>This is the factory for a node which has two parents as the source of the value.</summary>
    /// <typeparam name="T1">The type of the first parent's value for this node.</typeparam>
    /// <typeparam name="T2">The type of the second parent's value for this node.</typeparam>
    /// <typeparam name="TReturn">The type of this function will return.</typeparam>
    sealed public class Function<T1, T2, TReturn>: FuncDef<TReturn>
        where T1 : class, INode
        where T2 : class, INode
        where TReturn : class, INode {

        /// <summary>The factory for creating the node.</summary>
        readonly private S.Func<T1, T2, TReturn> handle;

        /// <summary>Creates a new dual node factory.</summary>
        /// <param name="handle">The factory handle.</param>
        /// <param name="needsOneNoCast">Indicates that at least one argument must not be a cast.</param>
        public Function(S.Func<T1, T2, TReturn> handle, bool needsOneNoCast = false) :
            base(needsOneNoCast, false, Type.FromType<T1>(), Type.FromType<T2>()) =>
            this.handle = handle;

        /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
        /// <returns>The new instance of this node.</returns>
        public override INode NewInstance() => new Function<T1, T2, TReturn>(this.handle, this.NeedsOneNoCast);

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => nameof(Function<T1, T2, TReturn>);

        /// <summary>Builds and return the function node with the given arguments already casted.</summary>
        /// <param name="nodes">These are the nodes casted into the correct type for the build.</param>
        /// <returns>The resulting function node.</returns>
        protected override INode PostCastBuild(INode[] nodes) => this.handle(nodes[0] as T1, nodes[1] as T2);
    }
}
