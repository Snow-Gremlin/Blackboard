using S = System;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core;

namespace Blackboard.Parser.Actors {

    /// <summary>An action for storing an identifier to be created, accesses, etc.</summary>
    sealed internal class Identifier: IActor {
        private INode node;

        /// <summary>Creates a new identifier action.</summary>
        /// <param name="receiver">The receiver this identifier is part of.</param>
        /// <param name="name">The name of this identifier.</param>
        public Identifier(IActor receiver, string name) {
            this.Receiver = receiver;
            this.Name = name;
            this.node = null;
        }

        /// <summary>The receiver object to read this identifier from.</summary>
        public IActor Receiver;

        /// <summary>The name of the identifier to read.</summary>
        public string Name;

        /// <summary>
        /// This will attempt to lookup the node that this identifier is for.
        /// This will assume the node should exist and will throw an exception if it doesn't.
        /// </summary>
        public INode Node {
            get {
                if (this.node is not null) return this.node;
                //INode receiver = this.Receiver.Evaluate();
                //this.node = receiver as 

                // TODO: FINISH.

                return this.node;
            }
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
