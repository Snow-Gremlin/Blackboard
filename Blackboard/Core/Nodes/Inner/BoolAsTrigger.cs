using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is an implicit cast from a bool to a trigger.</summary>
    /// <remarks>
    /// This forces a trigger into an always provoked or not provoked state so that a boolean value can be used with
    /// a trigger (e.g. `Trigger && (10 > 3)`). However this trigger will only act provoked, i.e. update its children,
    /// when the boolean changes value. This means that you can get an update from this trigger when it is not provoked.
    /// </remarks>
    sealed public class BoolAsTrigger: TriggerNode, IChild, IDataNode {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<Bool>, BoolAsTrigger>((value) => new BoolAsTrigger(value));

        /// <summary>This is the parent node to read from.</summary>
        private IValueParent<Bool> source;

        /// <summary>Creates a new bool value to trigger conversion.</summary>
        /// <param name="source">The boolean parent to get the provoked state from.</param>
        public BoolAsTrigger(IValueParent<Bool> source = null) => this.Parent = source;

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "BoolAsTrigger";

        /// <summary>The parent node to get the source value from.</summary>
        public IValueParent<Bool> Parent {
            get => this.source;
            set => this.SetParent(ref this.source, value);
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public IEnumerable<IParent> Parents => IChild.EnumerateParents(this.source);

        /// <summary>This gets the data being stored in this node.</summary>
        /// <remarks>This returns the data from the source boolean value or null if not set.</remarks>
        /// <returns>The data being stored.</returns>
        public IData Data => this.source?.Data;

        /// <summary>
        /// This is called when the trigger is evaluated and updated.
        /// It will determine if the trigger should be provoked.
        /// </summary>
        /// <returns>True if this trigger should be provoked, false if not.</returns>
        protected override bool ShouldProvoke() => this.source?.Value.Value ?? false;
    }
}
