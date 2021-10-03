using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;
using System.Linq;

namespace Blackboard.Parser.Prepers {

    /// <summary>This is an preper for performing an operation on some inputs.</summary>
    sealed internal class FunctionPrep: IPreper {

        /// <summary>Creates a new function preper.</summary>
        /// <param name="funcs">The function group for the function preper.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="loc">The location this function was defind in the code.</param>
        /// <param name="arguments">The input arguments for this function.</param>
        public FunctionPrep(FuncGroup funcs, string name, Location loc, IPreper[] arguments) {
            this.Functions = funcs;
            this.Name = name;
            this.Location = loc;
            this.Arguments = arguments;
        }

        /// <summary>The function group for the function preper.</summary>
        public FuncGroup Functions;

        /// <summary>The name of the function.</summary>
        public string Name;

        /// <summary>The location this function was called in the code.</summary>
        public Location Location { get; private set; }

        /// <summary>The input arguments for this function.</summary>
        public IPreper[] Arguments;

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, Options option) {
            IPerformer[] inputs = this.Arguments.Select((arg) => arg.Prepare(formula, option)).NotNull().ToArray();
            Type[] types = inputs.Select((arg) => Type.FromType(arg.Returns())).ToArray();

            IFunction func = this.Functions.Find(types);
            if (func is null)
                throw new Exception("No function found which acceptsthe the input types.").
                    With("Name", this.Name).
                    With("Inputs", string.Join(", ", types.Select((t) => t.ToString()))).
                    With("Location", this.Location.ToString());

            Function performer = new(func, this.Name, this.Location, inputs);

            if (option == Options.Create) return performer;

            if (option == Options.Evaluate) {
                if (!Type.FromType(performer.Returns()).HasData)
                    throw new Exception("Unable to evaluate the function into a constant.").
                        With("Name", this.Name).
                        With("Inputs", string.Join(", ", types.Strings())).
                        With("Location", this.Location.ToString());
                performer.Evaluate = true;
                return performer;
            }

            throw new Exception("Invalid option for a function preper.").
                With("Option", option).
                With("Name", this.Name).
                With("Inputs", string.Join(", ", types.Strings())).
                With("Location", this.Location.ToString());
        }
    }
}
