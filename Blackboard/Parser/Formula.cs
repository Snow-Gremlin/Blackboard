using Blackboard.Core;
using Blackboard.Core.Nodes.Caps;
using Blackboard.Parser.Performers;
using System.Collections.Generic;
using PetiteParser.Scanner;

namespace Blackboard.Parser {

    /// <summary>This is the complete set of performers being prepared.</summary>
    /// <remarks>
    /// It contains the set of actions pending to be performed on the Blackboard.
    /// This holds onto virtual nodes being added and nodes vertually removed
    /// prior to the actions being performed. 
    /// </remarks>
    sealed internal class Formula {

        /// <summary>The driver for the Blackboard to apply this formula to.</summary>
        public readonly Driver Driver;

        private readonly LinkedList<IWrappedNode> scopes;
        private readonly LinkedList<IPerformer> pending;
        private IWrappedNode global;

        // TODO: Add a collection of nodes which can NOT be gotten because they are pending removal.

        /// <summary>Creates a new formula to store pending changes to the given driver.</summary>
        /// <param name="driver">The driver this formula will change.</param>
        public Formula(Driver driver) {
            this.Driver = driver;
            this.scopes = new LinkedList<IWrappedNode>();
            this.pending = new LinkedList<IPerformer>();

            // Call reset to prepare the formula.
            this.Reset();
        }

        /// <summary>Adds a pending performer into this formula.</summary>
        /// <param name="performer">The performer to add.</param>
        public void Add(IPerformer performer) =>
            this.pending.AddLast(performer);

        /// <summary>Performs all pending actions then resets the formula.</summary>
        public void Perform() {
            foreach (IPerformer performer in this.pending)
                performer.Perform(this);
            this.Reset();
        }

        /// <summary>Resets the formula back to the initial state.</summary>
        public void Reset() {
            this.pending.Clear();
            this.scopes.Clear();
            this.global = new RealNode(this.Driver.Global);
            this.scopes.AddFirst(this.global);
        }

        /// <summary>Gets the current top of the scope stack.</summary>
        public IWrappedNode CurrentScope => this.scopes.First.Value;

        /// <summary>Pushes a new node onto he scope.</summary>
        /// <param name="node">The node to push on the scope.</param>
        public void PushScope(IWrappedNode node) => this.scopes.AddFirst(node);
    }
}
