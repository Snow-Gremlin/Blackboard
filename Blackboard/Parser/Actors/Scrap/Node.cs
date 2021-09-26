using Blackboard.Core;
using Blackboard.Core.Nodes.Interfaces;
using PetiteParser.Scanner;

namespace Blackboard.Parser.Actors {

    /// <summary>An actor which contains an existing node.</summary>
    sealed internal class Node: IActor {

        /// <summary>Creates a new node actor.</summary>
        /// <param name="location">The nearest location in the code that requested this node.</param>
        /// <param name="node">The existing node to store.</param>
        public Node(Location location, INode node) {
            this.Location = location;
            this.Existing = node;
        }

        /// <summary>The existing node being stored.</summary>
        public INode Existing;

        /// <summary>The location this actor was defind in the code being parsed.</summary>
        public Location Location { get; private set; }

        /// <summary>Prepare will check and simplify the actor as much as possible.</summary>
        /// <returns>
        /// This is the actor to replace this one with,
        /// if this actor is returned then it should not be replaced.
        /// if null then this actor should be removed.
        /// </returns>
        public IActor Prepare() => this.Existing is null ? null : this;

        /// <summary>Reduces this actor to a literal of the value without writing any nodes.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode Evaluate() => this.Existing;

        /// <summary>Creates and writes a node to Blackboard.</summary>
        /// <returns>The node value of this actor.</returns>
        public INode BuildNode() => this.Existing;

        /// <summary>Determines the type of the value which will be returned.</summary>
        /// <returns>The returned value type.</returns>
        public Type Returns() => Type.TypeOf(this.Existing);
    }
}
