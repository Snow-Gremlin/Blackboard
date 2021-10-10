using Blackboard.Core;
using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {
    sealed internal class NewInput: IPreper {

        public Type Type;
        public IdPrep Identifier;
        public IPreper Value;

        public NewInput(Location loc, Type t, IdPrep id, IPreper value) {
            this.Location = loc;
            this.Type = t;
            this.Identifier = id;
            this.Value = value;
        }

        public Location Location { get; private set; }

        /*
        private IPerformer typeNoAssign(Formula formula, Options option) {
            INode newNode =
                this.Type == Type.Bool    ? new InputValue<Bool>() :
                this.Type == Type.Int     ? new InputValue<Int>() :
                this.Type == Type.Double  ? new InputValue<Double>() :
                this.Type == Type.String  ? new InputValue<String>() :
                this.Type == Type.Trigger ? new InputTrigger() :
                throw new Exception("Unsupported type for new input").
                    With("Type", this.Type);
        }

        private IPerformer typeWithAssign(Formula formula, Options option) {
            if (idItem.Value is not null)
                throw new Exception("Can not create new input node. Identifier already exists in the receiver.").
                     With("Identifier", idItem);
            if (!idItem.HasId)
                throw new Exception("Can not create new input node with no Identifier.").
                     With("Identifier", idItem);

            Type t = typeItem.ValueAs<Type>();
            INode value = t.Implicit(valueItem.ValueAs<INode>());
            if (value is null)
                throw new Exception("The new input node can not be assigned the value given to it.").
                     With("Value", valueItem).
                     With("Type", typeItem).
                     With("Identifier", idItem);

            // Unless the identifier has a receiver, add the new input node to the current namespace scope.
            Namespace scope = this.scopeStack.First.Value;
            scope[idItem.Id] =
                t == Type.Bool    ? new InputValue<Bool>((value as IValue<Bool>).Value) :
                t == Type.Int     ? new InputValue<Int>((value as IValue<Int>).Value) :
                t == Type.Double  ? new InputValue<Double>((value as IValue<Double>).Value) :
                t == Type.String  ? new InputValue<String>((value as IValue<String>).Value) :
                t == Type.Trigger ? new InputTrigger((value as ITrigger).Provoked) :
                throw new Exception("Unsupported type for new input").
                    With("Type", typeItem);
        }

        private IPerformer assignWithNoType(Formula formula, Options option) {

            if (idItem.Value is not null)
                throw new Exception("Can not create new input node. Identifier already exists in the receiver.").
                     With("Identifier", idItem);
            if (!idItem.HasId)
                throw new Exception("Can not create new input node with no Identifier.").
                     With("Identifier", idItem);

            // Pull the type from the value that is going to be assigned.
            INode value = valueItem.ValueAs<INode>();
            Type t = Type.TypeOf(value);

            // Unless the identifier has a receiver, add the new input node to the current namespace scope.
            Namespace scope = this.scopeStack.First.Value;
            scope[idItem.Id] =
                t == Type.Bool    ? new InputValue<Bool>(  (value as IValue<Bool>  ).Value) :
                t == Type.Int     ? new InputValue<Int>(   (value as IValue<Int>   ).Value) :
                t == Type.Double  ? new InputValue<Double>((value as IValue<Double>).Value) :
                t == Type.String  ? new InputValue<String>((value as IValue<String>).Value) :
                t == Type.Trigger ? new InputTrigger(      (value as ITrigger      ).Provoked) :
                throw new Exception("Unsupported type from assignment for new input").
                    With("Type", t);
           
        }
        */

        public IPerformer Prepare(Formula formula, Options option) {
            throw new System.NotImplementedException();
        }
    }
}
