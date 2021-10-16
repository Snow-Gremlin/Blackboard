using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Parser.Performers;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Prepers {

    /// <summary>A preper for storing an identifier to be created, assigned, or accessed.</summary>
    sealed internal class IdPrep: IPreper {

        /// <summary>Creates a new identifier preper without a receiver.</summary>
        /// <param name="loc">The location this identifier was defined.</param>
        /// <param name="scopes">The scope stack that existed when this identifier was created.</param>
        /// <param name="name">The name of this identifier.</param>
        public IdPrep(Location loc, IWrappedNode[] scopes, string name) {
            this.Location = loc;
            this.Scopes = scopes;
            this.Receiver = null;
            this.Name = name;
        }

        /// <summary>Creates a new identifier preper with a receiver.</summary>
        /// <param name="loc">The location this identifier was defined.</param>
        /// <param name="receiver">The receiver this identifier is part of.</param>
        /// <param name="name">The name of this identifier.</param>
        public IdPrep(Location loc, IPreper receiver, string name) {
            this.Location = loc;
            this.Scopes = null;
            this.Receiver = receiver;
            this.Name = name;
        }

        /// <summary>The location this identifier was defind in the code being parsed.</summary>
        public Location Location;

        /// <summary>The scope stack that existed when this identifier was created.</summary>
        /// <remarks>The first item is the newest scope and the last is the global scope.</remarks>
        public IWrappedNode[] Scopes;

        /// <summary>The receiver object to read this identifier from.</summary>
        /// <remarks>If this is null then the scopes must not be null and vice versa.</remarks>
        public IPreper Receiver;

        /// <summary>The name of the identifier to read.</summary>
        public string Name;

        /// <summary>Finds the node in by the given identifier in the scopes stack.</summary>
        /// <returns>The found node in the scope or null.</returns>
        private IPerformer resolveInScope() {
            for (int i = this.Scopes.Length-1; i >= 0; --i) {
                IWrappedNode scope = this.Scopes[i];
                IWrappedNode node = scope.ReadField(this.Name);
                if (node is not null) return new WrappedNodeReader(node);
            }

            throw new Exception("No identifier found in the scope stack.").
                With("Identifier", this.Name).
                With("Locacation", this.Location);
        }

        /// <summary>Finds the node in the current receiver after evaluating the receiver.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="evaluate">
        /// True to reduce the nodes to constants without writting them to Blackboard.
        /// False to create the nodes and look up Ids when running.
        /// </param>
        /// <returns>The found node in the receiver or null.</returns>
        private IPerformer resolveInReceiver(Formula formula, bool evaluate) {
            IPerformer receiver = this.Receiver.Prepare(formula, evaluate);

            if (receiver.Type.IsAssignableTo(typeof(IFieldReader)))
                throw new Exception("Node can not be used as receiver, so it can not be used with an identifier.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Locacation", this.Location);

            if (receiver is not WrappedNodeReader recRef)
                throw new Exception("Receiver is not a wrapped node so it can not be read from.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location);

            IWrappedNode node = recRef.WrappedNode.ReadField(this.Name);
            return node is not null ? new WrappedNodeReader(node) :
                throw new Exception("Identifier not found in the receiver.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location);
        }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="evaluate">
        /// True to reduce the nodes to constants without writting them to Blackboard.
        /// False to create the nodes and look up Ids when running.
        /// </param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public IPerformer Prepare(Formula formula, bool evaluate = false) {
            IPerformer value = this.Receiver is null ? this.resolveInScope() : this.resolveInReceiver(formula, evaluate);
            return evaluate ? new Evaluator(value) : value;
        }

        /// <summary>Creates new virtual node for this identifier.</summary>
        /// <remarks>The identifier can not exist on the top of scope or in the receiver.</remarks>
        /// <param name="formula">The formula being worked on.</param>
        /// <param name="creationType">The type of the node to create for this identifier.</param>
        /// <returns>The virtual node for the new node for this identifier.</returns>
        public VirtualNode CreateNode(Formula formula, System.Type creationType) {
            if (this.Receiver is null) {
                // No receiver so create the node at the top of the scope.
                return formula.CurrentScope.CreateField(this.Name, creationType);
            }

            // There is a receiver so create the node on the receiver's node.
            IPerformer receiver = this.Receiver.Prepare(formula, false);
            return !receiver.Type.IsAssignableTo(typeof(IFieldReader)) ?
                throw new Exception("Node can not be used as receiver, so it can not be used with an identifier.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Locacation", this.Location) :
                receiver is not WrappedNodeReader recRef ?
                throw new Exception("Receiver is not a wrapped node so it can not be written to.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location) :
                recRef.WrappedNode.CreateField(this.Name, creationType);
        }
    }
}
