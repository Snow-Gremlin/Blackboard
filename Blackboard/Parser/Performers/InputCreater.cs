using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Data.Caps;

namespace Blackboard.Parser.Performers {
    sealed internal class InputCreater: IPerformer {

        public Type Type;

        public VirtualNode VirtualNode;

        public IPerformer Value;

        public InputCreater(Location loc, Type t, VirtualNode node, string name, IPerformer value = null) {
            this.Location = loc;
            this.Type = t;
            this.VirtualNode = node;
            this.Value = value;
        }

        public Location Location { get; private set; }

        public System.Type ReturnType => this.Type.RealType;

        private INode newInput() =>
            this.Type == Type.Bool    ? new InputValue<Bool>() :
            this.Type == Type.Int     ? new InputValue<Int>() :
            this.Type == Type.Double  ? new InputValue<Double>() :
            this.Type == Type.String  ? new InputValue<String>() :
            this.Type == Type.Trigger ? new InputTrigger() :
            throw new Exception("Unsupported type for new input").
                With("Type", this.Type);

        private INode newInputWithValue(Formula formula) {
            INode value = this.Value.Perform(formula);
            return
                this.Type == Type.Bool    ? new InputValue<Bool>(  (value as IValue<Bool>  ).Value) :
                this.Type == Type.Int     ? new InputValue<Int>(   (value as IValue<Int>   ).Value) :
                this.Type == Type.Double  ? new InputValue<Double>((value as IValue<Double>).Value) :
                this.Type == Type.String  ? new InputValue<String>((value as IValue<String>).Value) :
                this.Type == Type.Trigger ? new InputTrigger(      (value as ITrigger      ).Provoked) :
                throw new Exception("Unsupported type for new input").
                    With("Type", this.Type);
        }

        public INode Perform(Formula formula) {
            INode input = this.Value is null ? this.newInput() : this.newInputWithValue(formula);
            this.VirtualNode.Node = input;
            return input;
        }
    }
}
