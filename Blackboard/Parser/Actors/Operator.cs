using Blackboard.Core;
using Blackboard.Core.Functions;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Data.Caps;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PP = PetiteParser;
using S = System;

namespace Blackboard.Parser.Actors {
    internal class Operator: IActor {
        private FuncGroup op;
        private string name;
        private PP.Scanner.Location loc;
        private IActor[] arguments;

        public Operator(FuncGroup op, string name, PP.Scanner.Location loc, IActor[] arguments) {
            this.op = op;
            this.name = name;
            this.loc = loc;
            this.arguments = arguments;
        }

        public INode CreateNode() {
            INode[] inputs = this.arguments.Select((arg) => arg.CreateNode()).ToArray();
            INode node = op.Build(inputs);

            if (node is null) {
                throw new Exception("The operator can not be called with given input.").
                    With("Operation", name).
                    With("Inputs", string.Join(", ", inputs.TypeNames())).
                    With("Location", loc.ToString());
            }

            if (inputs.IsConstant()) node = node.ToLiteral();
            return node;
        }

        public INode Evaluate() {
            INode[] inputs = this.arguments.Select((arg) => arg.Evaluate()).ToArray();


        }
    }
}
