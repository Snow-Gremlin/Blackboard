using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Parser.Actors {

    /// <summary>An action for storing an identifier to be created, accesses, etc.</summary>
    sealed internal class Identifier: IActor {
        private INode node;

        /// <summary>Creates a new identifier action.</summary>
        /// <param name="scopes">The scope stack that existed when this identifier was created.</param>
        /// <param name="receiver">The receiver this identifier is part of.</param>
        /// <param name="name">The name of this identifier.</param>
        public Identifier(Namespace[] scopes, IActor receiver, string name) {
            this.Receiver = receiver;
            this.Scopes = scopes;
            this.Name = name;
            this.node = null;
        }

        /// <summary>The scope stack that existed when this identifier was created.</summary>
        public Namespace[] Scopes;

        /// <summary>The receiver object to read this identifier from.</summary>
        public IActor Receiver;

        /// <summary>The name of the identifier to read.</summary>
        public string Name;

        /// <summary>Finds the node in by the given identifier in the scopes stack.</summary>
        /// <returns>The found node in the scope or null.</returns>
        private INode resolveInScope() {
            for (int i = this.Scopes.Length-1; i >= 0; --i) {
                Namespace scope = this.Scopes[i];
                INode node = scope.ReadField(this.Name);
                if (node is not null) return node;
            }
            throw new Exception("Not identifier found in the scope stack.").
                With("Identifier", this.Name);
        }

        /// <summary>Finds the node in the current receiver after evaluating the receiver.</summary>
        /// <returns>The found node in the receiver or null.</returns>
        private INode resolveInReceiver() {
            INode receiverNode = this.Receiver.Evaluate();
            if (receiverNode is not IFieldReader receiver)
                throw new Exception("Node can not be used as receiver, so it can not be used with an identifier.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiverNode);

            INode node = receiver.ReadField(this.Name);
            return node is not null ? node :
                throw new Exception("Not identifier found in the receiver.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver);
        }

        /// <summary>
        /// This will attempt to lookup, resolve, the node that this identifier is for.
        /// This will assume the node should exist and will throw an exception if it doesn't.
        /// This can be assigned to override or reset the node to null
        /// so the next time this is read from it will be resolved again.
        /// </summary>
        public INode Node {
            get {
                if (this.node is not null) return this.node;
                this.node = this.Receiver is null ? this.resolveInScope() : this.resolveInReceiver();
                return this.node;
            }
            set => this.node = value;
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
                        With("Node", node);
                receiver = receiverNode as IFieldWriter;
            }

            if (mustCreate && receiver.ContainsField(this.Name))
                throw new Exception("A field by the given name already exists on the receiver.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Node", node);

            receiver.WriteField(this.Name, node);
            this.node = node;
        }

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() => this.Node;

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() => this.Node;

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => Type.TypeOf(this.Node);
    }
}
