using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;
using PP = PetiteParser;

namespace Blackboard.Parser.Actors {

    /// <summary>This is an actor for performing an operation on some inputs.</summary>
    sealed internal class Operator: IActor {

        /// <summary>Creates a new operator actor.</summary>
        /// <param name="func">The function for the operator.</param>
        /// <param name="name">The name of the operator.</param>
        /// <param name="loc">The location this operator was defind in the code.</param>
        /// <param name="arguments">The input arguments for this operator.</param>
        public Operator(IFunction func, string name, PP.Scanner.Location loc, IActor[] arguments) {
            this.Function = func;
            this.Name = name;
            this.Location = loc;
            this.Arguments = arguments;
        }

        /// <summary>The function for the operator.</summary>
        public IFunction Function;

        /// <summary>The name of the operator.</summary>
        public string Name;

        /// <summary>The location this operator was defind in the code.</summary>
        public PP.Scanner.Location Location;

        /// <summary>The input arguments for this operator.</summary>
        public IActor[] Arguments;

        /// <summary>This will build the node for this operator with the given input.</summary>
        /// <param name="inputs">The input nodes to give as parameters into the operator.</param>
        /// <returns>The resulting node from this operation.</returns>
        private INode build(INode[] inputs) {
            INode node = this.Function.Build(inputs);
            return node is not null ? node :
                throw new Exception("The operator can not be called with given input.").
                    With("Operation", Name).
                    With("Inputs", string.Join(", ", inputs.TypeNames())).
                    With("Location", Location.ToString());
        }

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() {
            INode[] inputs = this.Arguments.Select((arg) => arg.Evaluate()).ToArray();
            INode node = (this.build(inputs) as IDataNode)?.ToConstant();
            return node is not null ? node :
                throw new Exception("Unable to evaluate an operator into a constant.").
                    With("Operation", Name).
                    With("Inputs", string.Join(", ", inputs.TypeNames())).
                    With("Location", Location.ToString());
        }

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() {
            INode[] inputs = this.Arguments.Select((arg) => arg.BuildNode()).ToArray();
            return this.build(inputs);
        }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => this.Function.Returns();
    }
}
