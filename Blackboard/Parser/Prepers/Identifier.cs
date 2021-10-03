using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>An action for storing an identifier to be created, accesses, etc.</summary>
    sealed internal class Identifier: IPreper  {

        /// <summary>Creates a new identifier action.</summary>
        /// <param name="location">The location this identifier was defined.</param>
        /// <param name="scopes">The scope stack that existed when this identifier was created.</param>
        /// <param name="receiver">The receiver this identifier is part of.</param>
        /// <param name="name">The name of this identifier.</param>
        public Identifier(Location location, Namespace[] scopes, IPreper receiver, string name) {
            this.Location = location;
            this.Scopes = scopes;
            this.Receiver = receiver;
            this.Name = name;
        }

        /// <summary>The scope stack that existed when this identifier was created.</summary>
        public Namespace[] Scopes;

        /// <summary>The receiver object to read this identifier from.</summary>
        public IPreper Receiver;

        /// <summary>The name of the identifier to read.</summary>
        public string Name;

        /// <summary>The location this actor was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>Finds the node in by the given identifier in the scopes stack.</summary>
        /// <returns>The found node in the scope or null.</returns>
        private INode resolveInScope() {
            for (int i = this.Scopes.Length-1; i >= 0; --i) {
                Namespace scope = this.Scopes[i];
                INode node = scope.ReadField(this.Name);
                if (node is not null) return node;
            }

            throw new Exception("Not identifier found in the scope stack.").
                With("Identifier", this.Name).
                With("Locacation", this.Location.ToString());
        }

        /// <summary>Finds the node in the current receiver after evaluating the receiver.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>The found node in the receiver or null.</returns>
        private INode resolveInReceiver(Formula formula, Options option) {
            IPerformer receiver = this.Receiver.Prepare(formula, option);

            if (receiver.Returns().IsAssignableTo(typeof(IFieldReader)))
                throw new Exception("Node can not be used as receiver, so it can not be used with an identifier.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Locacation", this.Location.ToString());


            if (receiver is Node receiverNode) {

                // TODO: IMPLEMENT

            } else if (receiver is NodeRef receiverRef) {

                // TODO: IMPLEMENT

            }




            INode node = receiver.ReadField(this.Name);
            return node is not null ? node :
                throw new Exception("Not identifier found in the receiver.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location.ToString());
        }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, Options option) {


            INode node = this.Receiver is null ? this.resolveInScope() : this.resolveInReceiver();
            return new Node(this.Location, node);


        }


        /// <summary>This will attempt to write this node to the given receiver or to the top of the stack.</summary>
        /// <param name="node">The node to assign to this receiver as this identifier.</param>
        /// <param name="mustCreate">Indicates that the value must be created and does not exist yet.</param>
        public void Assign(INode node, bool mustCreate) {
            IFieldWriter receiver;
            if (this.Receiver is null) receiver = this.Scopes[^1];
            else {
                INode receiverNode = this.Receiver.Evaluate();
                if (receiverNode is not IFieldWriter)
                    throw new Exception("Node is not a writable receiver, so it can not be used to assign an identifier's node.").
                        With("Identifier", this.Name).
                        With("Attempted Receiver", receiverNode).
                        With("Node", node).
                        With("Locacation", this.Location.ToString());

                receiver = receiverNode as IFieldWriter;
            }

            if (mustCreate && receiver.ContainsField(this.Name))
                throw new Exception("A field by the given name already exists on the receiver.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Node", node).
                    With("Locacation", this.Location.ToString());

            receiver.WriteField(this.Name, node);
        }
    }
}
