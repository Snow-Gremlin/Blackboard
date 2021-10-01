using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Linq;

namespace Blackboard.Parser.Performers {

    /// <summary>This is an performer for performing an operation on some inputs.</summary>
    sealed internal class Function: IPerformer {

        /// <summary>Creates a new function performer.</summary>
        /// <param name="func">The function for the function performer.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="loc">The location this function was called in the code.</pararm>
        /// <param name="arguments">The input arguments for this function.</param>
        public Function(IFunction func, string name, Location loc, IPerformer[] arguments) {
            this.Func = func;
            this.Name = name;
            this.Location = loc;
            this.Arguments = arguments;
            this.Evaluate = false;
        }

        /// <summary>The function for the function performer.</summary>
        public IFunction Func;

        /// <summary>The name of the function.</summary>
        public string Name;

        /// <summary>The location this function was defind in the code.</summary>
        public Location Location { get; private set; }

        /// <summary>The input arguments for this function.</summary>
        public IPerformer[] Arguments;

        /// <summary>Indicates the result should be turned into a constant.</summary>
        public bool Evaluate;

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public System.Type Returns() => this.Func.Returns();

        /// <summary>This will build or evaluate the actor and apply it to Blackboard.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <returns>This is the newly created node or null.</returns>
        public INode Perform(Formula formula) {
            INode[] inputs = this.Arguments.Select((arg) => arg.Perform(formula)).NotNull().ToArray();
            INode node = this.Func.Build(inputs);
            if (this.Evaluate) node = (node as IDataNode)?.ToConstant();
            return node is not null ? node :
                // This expection should never be hit if the preper is working as expected.
                throw new Exception("The function can not be called with given input.").
                    With("Name", this.Name).
                    With("Evaluate", this.Evaluate).
                    With("Inputs", string.Join(", ", inputs.Types().Strings())).
                    With("Location", this.Location.ToString());
        }
    }
}
