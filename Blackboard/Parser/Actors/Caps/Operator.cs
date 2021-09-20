using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Linq;
using PP = PetiteParser;
using Blackboard.Parser.Actors.Interfaces;

namespace Blackboard.Parser.Actors.Caps {

    /// <summary>This is an actor for performing an operation on some inputs.</summary>
    sealed internal class Operator: INodeBuilder {

        /// <summary>Creates a new operator actor.</summary>
        /// <param name="func">The function for the operator.</param>
        /// <param name="name">The name of the operator.</param>
        /// <param name="loc">The location this operator was defind in the code.</param>
        /// <param name="arguments">The input arguments for this operator.</param>
        public Operator(IFunction func, string name, PP.Scanner.Location loc, INodeBuilder[] arguments) {
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
        public PP.Scanner.Location Location { get; private set; }

        /// <summary>The input arguments for this operator.</summary>
        public INodeBuilder[] Arguments;

        #region Prepare Methods...

        /// <summary>Prepare will check and simplify the actor as much as possible.</summary>
        /// <returns>
        /// This is the actor to replace this one with,
        /// if this actor is returned then it should not be replaced.
        /// if null then this actor should be removed.
        /// </returns>
        public IActor Prepare() {
            this.Arguments = this.Arguments.Select((arg) => arg.Prepare() as INodeBuilder).NotNull().ToArray();
           
            //
            // TODO: Impelment
            //

            return this;
        }

        #endregion

        /// <summary>This will build the node for this operator with the given input.</summary>
        /// <param name="inputs">The input nodes to give as parameters into the operator.</param>
        /// <returns>The resulting node from this operation.</returns>
        private INode build(INode[] inputs) {
            INode node = this.Function.Build(inputs);
            return node is not null ? node :
                throw new Exception("The operator can not be called with given input.").
                    With("Operation", Name).
                    With("Inputs", string.Join(", ", inputs.TypeNames())).
                    With("Location", this.Location.ToString());
        }

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() {
            INode[] inputs = this.Arguments.Select((arg) => arg.Evaluate()).NotNull().ToArray();
            INode node = (this.build(inputs) as IDataNode)?.ToConstant();
            return node is not null ? node :
                throw new Exception("Unable to evaluate an operator into a constant.").
                    With("Operation", Name).
                    With("Inputs", string.Join(", ", inputs.TypeNames())).
                    With("Location", this.Location.ToString());
        }

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Build() {
            INode[] inputs = this.Arguments.Select((arg) => arg.Build()).NotNull().ToArray();
            return this.build(inputs);
        }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => this.Function.Returns();
    }
}
