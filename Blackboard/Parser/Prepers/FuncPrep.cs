using Blackboard.Core;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is an preper for performing a function on some inputs.</summary>
    sealed internal class FuncPrep: IPreper {

        /// <summary>Creates a new function preper.</summary>
        /// <param name="loc">The location this function was defind in the code.</param>
        /// <param name="source">The source of the function to call.</param>
        /// <param name="arguments">The input arguments for this function.</param>
        public FuncPrep(Location loc, IPreper source, params IPreper[] arguments) {
            this.Location = loc;
            this.Source = source;
            this.Arguments = new(arguments);
        }

        /// <summary>The location this function was called in the code.</summary>
        public Location Location;

        /// <summary>The source of the function to call.</summary>
        public IPreper Source;

        /// <summary>The input arguments for this function.</summary>
        public List<IPreper> Arguments;

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="evaluate">True to evaluate to a constant, false otherwise.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, bool evaluate = false) {
            IPerformer source = this.Source.Prepare(formula, evaluate);
            if (source is not WrappedNodeReader nodeRef)
                throw new Exception("Expected the identifier to return a NodeRef performer for a function name").
                    With("Source", this.Source).
                    With("Location", this.Location);

            if (nodeRef.WrappedNode.Virtual)
                throw new Exception("Function must be already defined prior to a function performer being called.").
                    With("Wrapped Node", nodeRef).
                    With("Source", this.Source).
                    With("Location", this.Location);

            IPerformer[] inputs = this.Arguments.Select((arg) => arg.Prepare(formula, evaluate)).NotNull().ToArray();
            Type[] types = inputs.Select((arg) => Type.FromType(arg.Type)).ToArray();

            IFuncDef func;
            if (nodeRef.WrappedNode.Node is FuncGroup funcGroup) {
                func = funcGroup.Find(types);
                if (func is null)
                    throw new Exception("No function found which accepts the the input types.").
                        With("Source", this.Source).
                        With("Inputs", string.Join(", ", types.Strings())).
                        With("Location", this.Location);
            } else if (nodeRef.WrappedNode.Node is IFuncDef funcDef) {
                func = funcDef;
                if (func.Match(types).IsMatch)
                    throw new Exception("Function defenition from source does not match types.").
                        With("Function", func).
                        With("Source", this.Source).
                        With("Inputs", string.Join(", ", types.Strings())).
                        With("Location", this.Location);
            } else throw new Exception("Function source must be either a function group or defenition.").
                    With("Wrapped Node", nodeRef).
                    With("Source", this.Source).
                    With("Inputs", string.Join(", ", types.Strings())).
                    With("Location", this.Location);

            Function performer = new(func, inputs);
            return !evaluate ? performer :
                performer.Type is IDataNode ? new Evaluator(performer) :
                throw new Exception("Unable to evaluate the function into a constant.").
                    With("Function", func).
                    With("Source", this.Source).
                    With("Inputs", string.Join(", ", types.Strings())).
                    With("Location", this.Location);
        }
    }
}
