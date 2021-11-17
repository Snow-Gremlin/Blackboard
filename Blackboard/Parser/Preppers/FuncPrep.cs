using Blackboard.Core;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Preppers {

    /// <summary>This is an prepper for performing a function on some inputs.</summary>
    sealed internal class FuncPrep: IPrepper {

        /// <summary>Creates a new function prepper.</summary>
        /// <param name="loc">The location this function was defind in the code.</param>
        /// <param name="source">The source of the function to call.</param>
        /// <param name="arguments">The input arguments for this function.</param>
        public FuncPrep(Location loc, IPrepper source, params IPrepper[] arguments) {
            this.Location = loc;
            this.Source = source;
            this.Arguments = new(arguments);
        }

        /// <summary>The location this function was called in the code.</summary>
        public Location Location;

        /// <summary>The source of the function to call.</summary>
        public IPrepper Source;

        /// <summary>The input arguments for this function.</summary>
        public List<IPrepper> Arguments;

        /// <summary>Prepares the source node for the function definition from the given source prepper.</summary>
        /// <remarks>
        /// Currently there can't be any virtual functions since there is no way to define a new function in
        /// the language at this time. When there is a virtual node reader for a group will have to 
        /// be able to check a virtual function definition exists on that group and get it with the types
        /// as a wrapped node reader containing the function definition into the function performer.
        /// The performer will also need to take in a performer BUT the type returned needs to be the return type
        /// of the function not the type of the wrapped function definition.
        /// </remarks>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="reduce">True to reduce to a constant, false otherwise.</param>
        /// <returns>The source node to get the function definition from.</returns>
        private INode prepareSource(Formula formula, bool reduce) {
            IPerformer source = this.Source.Prepare(formula, reduce);
            
            // Wrapped nodes are usually from methods called in the input code by name.
            if (source is WrappedNodeReader nodeRef) {
                return !nodeRef.WrappedNode.Virtual ? nodeRef.WrappedNode.Node :
                    throw new Exception("Function must be already defined prior to a function performer being called.").
                        With("Wrapped Node", nodeRef).
                        With("Source", this.Source).
                        With("Location", this.Location);
            }
            
            // Held nodes are usually from operators being called or when specific factories are being used.
            return source is NodeHold nodeHold ? nodeHold.Node :
                throw new Exception("Expected the identifier to return a wrapped node reader or node holder for a function name").
                    With("Source Performer", source).
                    With("Source Prepper", this.Source).
                    With("Location", this.Location);
        }

        /// <summary>Prepares the function definition that will be called by the performer.</summary>
        /// <param name="sourceNode">The source node to get the function from.</param>
        /// <param name="types">The input types used to determine the correct definition from a group.</param>
        /// <returns>The function definition that the preformer will call.</returns>
        private IFuncDef prepareFuncDef(INode sourceNode, Type[] types) {
            // Groups are usually when a function or operator is called and
            // the correct definition has to be looked up for the given input types.
            if (sourceNode is FuncGroup funcGroup) {
                IFuncDef func = funcGroup.Find(types);
                return func is not null ? func :
                    throw new Exception("No function found which accepts the the input types.").
                        With("Source", this.Source).
                        With("Inputs", types.Join(", ")).
                        With("Location", this.Location);
            }

            // Definitions are ususally when a specific factory is being used.
            return sourceNode is IFuncDef funcDef ?
                funcDef.Match(types).IsMatch ? funcDef :
                    throw new Exception("Function definition from source does not match types.").
                        With("Function", funcDef).
                        With("Source", this.Source).
                        With("Inputs", "["+types.Join(", ")+"]").
                        With("Location", this.Location) :
                throw new Exception("Function source must be either a function group or definition.").
                    With("Node", sourceNode).
                    With("Source", this.Source).
                    With("Inputs", types.Join(", ")).
                    With("Location", this.Location);
        }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="reduce">True to reduce to a constant, false otherwise.</param>
        /// <returns>
        /// This is the performer to replace this prepper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, bool reduce = false) {
            INode sourceNode = this.prepareSource(formula, reduce);
            
            IPerformer[] inputs = this.Arguments.Select((arg) => arg.Prepare(formula, reduce)).NotNull().ToArray();
            Type[] types = inputs.Select((arg) => Type.FromType(arg.Type)).ToArray();
            
            IFuncDef func = this.prepareFuncDef(sourceNode, types);
            return Reducer.Wrap(new Function(func, inputs), reduce);
        }

        /// <summary>Gets the prepper debug string.</summary>
        /// <returns>A human readable debug string.</returns>
        public override string ToString() =>
            "FuncPrep([" + this.Location + "], " + this.Source+ ", [" + this.Arguments.Join(", ") + "])";
    }
}
