using Blackboard.Core;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Linq;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is an preper for performing a function on some inputs.</summary>
    sealed internal class FuncPrep: IPreper {

        /// <summary>Creates a new function preper.</summary>
        /// <param name="loc">The location this function was defind in the code.</param>
        /// <param name="source">The source of the function to call.</param>
        /// <param name="arguments">The input arguments for this function.</param>
        public FuncPrep(Location loc, IPreper source, IPreper[] arguments) {
            this.Location = loc;
            this.Source = source;
            this.Arguments = arguments;
        }

        /// <summary>The location this function was called in the code.</summary>
        public Location Location;

        /// <summary>The source of the function to call.</summary>
        public IPreper Source;

        /// <summary>The input arguments for this function.</summary>
        public IPreper[] Arguments;

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public Performers.Performer Prepare(Formula formula, Options option) {
            IPerformer sperf = this.Source.Prepare(formula, option);
            if (sperf is not WrappedNodeReader nodeRef)
                throw new Exception("Expected the identifier to return a NodeRef performer for a function name").
                    With("Source", this.Source).
                    With("Location", this.Location);

            // Check that the id node already exists and is a FuncGroup since
            // we don't have a method for virtualizing single function instances yet.
            // To make this work, individual function definitions need to be nodes which
            // can be children to the function group and looked up with the parameter types.
            if (nodeRef.WrappedNode.Virtual || nodeRef.WrappedNode.Node is not FuncGroup funcGroup)
                throw new Exception("Currenty all functions must be already defined prior to a function performer being created.").
                    With("Wrapped Node", nodeRef).
                    With("Source", this.Source).
                    With("Location", this.Location);

            IPerformer[] inputs = this.Arguments.Select((arg) => arg.Prepare(formula, option)).NotNull().ToArray();
            Type[] types = inputs.Select((arg) => Type.FromType(arg.ReturnType)).ToArray();

            IFuncDef func = funcGroup.Find(types);
            if (func is null)
                throw new Exception("No function found which accepts the the input types.").
                    With("Source", this.Source).
                    With("Inputs", string.Join(", ", types.Select((t) => t.ToString()))).
                    With("Location", this.Location);

            Function performer = new(func, this.Location, inputs);

            if (option == Options.Create) return performer;
            if (option == Options.Evaluate) {
                if (performer.ReturnType is not IDataNode)
                    throw new Exception("Unable to evaluate the function into a constant.").
                        With("Source", this.Source).
                        With("Inputs", string.Join(", ", types.Strings())).
                        With("Location", this.Location);
                performer.Evaluate = true;
                return performer;
            }

            throw new Exception("Invalid option for a function preper.").
                With("Option", option).
                With("Source", this.Source).
                With("Inputs", string.Join(", ", types.Strings())).
                With("Location", this.Location);
        }
    }
}
