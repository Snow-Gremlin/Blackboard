using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Outer {

    /// <summary>This is a trigger which can be provoked from user input.</summary>
    sealed public class InputTrigger: TriggerNode, ITriggerInput {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<InputTrigger>(() => new InputTrigger());

        /// <summary>This is a factory function for creating new instances of this node easily with an initial value from the given node.</summary>
        static public readonly IFuncDef FactoryWithInitialValue =
            new Function<ITrigger, InputTrigger>((ITrigger node) => new InputTrigger(node.Provoked));

        /// <summary>
        /// This is a function to assign (provoke or not) to the input trigger.
        /// This will return the input node.
        /// </summary>
        static public readonly IFuncDef Assign =
            new Function<InputTrigger, ITrigger, InputTrigger>((InputTrigger input, ITrigger node) => {
                input.Provoke(node.Provoked);
                return input;
            });

        /// <summary>Creates a new input trigger.</summary>
        /// <param name="provoked">The initial provoked state of the trigger.</param>
        public InputTrigger(bool provoked = false) :
            base(provoked) { }

        /// <summary>Always returns no parents since inputs have no parent.</summary>
        public override IEnumerable<INode> Parents => Enumerable.Empty<INode>();

        /// <summary>This is set this trigger to emit during the next evaluation.</summary>
        /// <param name="value">True will provoke, false will reset the trigger.</param>
        /// <remarks>This is not intended to be be called directly, it should be called via the driver.</remarks>
        public void Provoke(bool value = true) => this.Provoked = value;

        /// <summary>This updates the trigger during the an evaluation.</summary>
        /// <returns>This returns the provoked value as it currently is.</returns>
        protected override bool UpdateTrigger() => this.Provoked;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Input";

        /// <summary>Creates a pretty string for this node.</summary>
        /// <param name="showFuncs">Indicates if functions should be shown or not.</param>
        /// <param name="nodeDepth">The depth of the nodes to get the string for.</param>
        /// <returns>The pretty string for debugging and testing this node.</returns>
        public override string PrettyString(bool showFuncs = true, int nodeDepth = int.MaxValue) =>
            this.TypeName + "<trigger>(" + (this.Provoked ? "provoked" : "") + ")";
    }
}
