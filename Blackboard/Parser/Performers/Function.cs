using Blackboard.Core.Nodes.Interfaces;
using S = System;
using System.Linq;
using Blackboard.Core;

namespace Blackboard.Parser.Performers {

    /// <summary>This will create a function node when prepared.</summary>
    sealed internal class Function: IPerformer {

        /// <summary>The function to performer.</summary>
        public readonly IFuncDef Func;

        /// <summary>>The input arguments for the function to perform.</summary>
        public readonly IPerformer[] Args;

        /// <summary>Creates a new function performer.</summary>
        /// <param name="func">The function to performer.</param>
        /// <param name="args">The input arguments for the function to perform.</param>
        public Function(IFuncDef func, params IPerformer[] args) {
            this.Func = func;
            this.Args = args;
        }

        /// <summary>Gets the type of the node which will be returned.</summary>
        public S.Type Type => this.Func.ReturnType;

        /// <summary>This will perform the actions that need to be run.</summary>
        /// <remarks>
        /// This should not throw an exception if prepared correctly.
        /// If this does throw an exception the preppers should be fixed to prevent this.
        /// </remarks>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform() {
            INode[] input = this.Args.Select((arg) => arg.Perform()).NotNull().ToArray();
            return this.Func.Build(input);
        }

        /// <summary>Gets the performer debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() => ("Function("+this.Func+", "+
            ", [" + this.Args.Select((IPerformer perf) => perf.ToString()).Join(", ") + "])").Replace("Blackboard.Core.", "");
    }
}
