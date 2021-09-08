using S = System;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core;

namespace Blackboard.Parser.Actors {

    sealed internal class Identifier: IActor {

        public Identifier(IActor receiver, string name) {
            this.Receiver = receiver;
            this.Name = name;
        }

        /// <summary>The receiver object to read this identifier from.</summary>
        public IActor Receiver;

        /// <summary>The name of the identifier to read.</summary>
        public string Name;

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() {
            throw new S.NotImplementedException();
        }

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() {
            throw new S.NotImplementedException();
        }

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() {
            throw new S.NotImplementedException();
        }
    }
}
