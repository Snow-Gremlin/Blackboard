using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
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
            this.CreationType = null;
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

        /// <summary>Creates a new virtual node for this identifier.</summary>
        /// <param name="recRef">The receiver to create this id into.</param>
        /// <returns>The newly created virtual node.</returns>
        private Performer createNode(IWrappedNode recRef) {
            IWrappedNode existing = recRef.ReadField(this.Name);
            if (existing is not null)
                throw new Exception("May not create a node which already exists.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", recRef).
                    With("Excisting", existing).
                    With("Locacation", this.Location);

            if (this.CreationType is null)
                throw new Exception("May not create a node without a creation type set.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", recRef).
                    With("Locacation", this.Location);

            VirtualNode node = new(this.Name, this.CreationType, recRef);
            return new WrappedNodeReader(this.Location, node, false);
        }

        /// <summary>Finds the node in by the given identifier in the scopes stack.</summary>
        /// <param name="option">The option for preparing this preper. Will only be either create or evaluate.</param>
        /// <returns>The found node in the scope or null.</returns>
        private Performer resolveInScope(Options option) {
            if (option == Options.Define)
                return this.createNode(this.Scopes[0]);

            for (int i = this.Scopes.Length-1; i >= 0; --i) {
                IWrappedNode scope = this.Scopes[i];
                IWrappedNode node = scope.ReadField(this.Name);
                if (node is not null)
                    return new WrappedNodeReader(this.Location, node, option == Options.Evaluate);
            }

            throw new Exception("No identifier found in the scope stack.").
                With("Identifier", this.Name).
                With("Locacation", this.Location);
        }

        /// <summary>Finds the node in the current receiver after evaluating the receiver.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper. Will only be either create or evaluate.</param>
        /// <returns>The found node in the receiver or null.</returns>
        private Performer resolveInReceiver(Formula formula, Options option) {
            Performer receiver = this.Receiver.Prepare(formula, option);

            if (receiver.ReturnType.IsAssignableTo(typeof(IFieldReader)))
                throw new Exception("Node can not be used as receiver, so it can not be used with an identifier.").
                    With("Identifier", this.Name).
                    With("Attempted Receiver", receiver).
                    With("Locacation", this.Location);

            if (receiver is not WrappedNodeReader recRef)
                throw new Exception("Not identifier found in the receiver.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location);

            if (option == Options.Define)
                return this.createNode(recRef.WrappedNode);

            IWrappedNode node = recRef.WrappedNode.ReadField(this.Name);
            return node is not null ?
                new WrappedNodeReader(this.Location, node, option == Options.Evaluate) :
                throw new Exception("Not identifier found in the receiver.").
                    With("Identifier", this.Name).
                    With("Receiver", receiver).
                    With("Locacation", this.Location);
        }

        /// <summary>This will check and prepare the node as much as possible.</summary>
        /// <param name="formula">This is the complete set of performers being prepared.</param>
        /// <param name="option">The option for preparing this preper.</param>
        /// <returns>
        /// This is the performer to replace this preper with,
        /// if null then no performer is used by parent for this node.
        /// </returns>
        public Performer Prepare(Formula formula, Options option) =>
            this.Receiver is null ? this.resolveInScope(option) : this.resolveInReceiver(formula, option);
    }
}
