using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This is an implicit cast from a bool to a trigger.</summary>
    /// <remarks>
    /// This forces a trigger into an always provoked or not provoked state so that a boolean value can be used with
    /// a trigger (e.g. `Trigger && (10 > 3)`). However this trigger will only act provoked, i.e. update its children,
    /// when the boolean changes value. This means that you can get an update from this trigger when it is not provoked.
    /// </remarks>
    sealed public class BoolAsTrigger: Bases.Evaluatable, ITriggerAdopter, IDataNode {

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory =
            new Function<IValueParent<Bool>, BoolAsTrigger>((value) => new BoolAsTrigger(value));

        /// <summary>This is the parent node to read from.</summary>
        private IValueParent<Bool> source;

        /// <summary>Creates a new bool value to trigger conversion.</summary>
        public BoolAsTrigger(IValueParent<Bool> source = null, bool provoked = default) {
            this.Provoked = provoked;
            this.Parent = source;
            this.updateTrigger();
        }

        /// <summary>The parent node to get the source value from.</summary>
        public IValueParent<Bool> Parent {
            get => this.source;
            set {
                this.SetParent(ref this.source, value);
                this.updateTrigger();
            }
        }

        /// <summary>The set of parent nodes to this node in the graph.</summary>
        public override IEnumerable<IAdopter> Parents => INode.NotNull(this.source);

        /// <summary>This gets the data being stored in this node.</summary>
        /// <remarks>This returns the data from the souce boolean value or null if not set.</remarks>
        /// <returns>The data being stored.</returns>
        public IData Data => this.source?.Data;

        /// <summary>Determines if the node is constant or if all of it's parents are constant.</summary>
        /// <remarks>This returns if the source boolean value is constent or not.</remarks>
        /// <returns>True if constant, false otherwise.</returns>
        public bool IsConstant => this.source?.IsConstant ?? false;

        /// <summary>Converts this node to a literal.</summary>
        /// <returns>
        /// Returns a bool literal or null, normally a trigger can not be a literal
        /// but in this case it just passes through the literal for the source.
        /// </returns>
        public IConstant ToConstant() => this.source?.ToConstant();

        /// <summary>
        /// Indicates if this trigger should be treated as if it had been fired during
        /// the current evaluation even though this will not reset. It returns the value
        /// from the parent bool value so that the bool can be used as a trigger.
        /// </summary>
        public bool Provoked { get; private set; }

        /// <summary>Resets the trigger at the end of the evaluation.</summary>
        /// <remarks>For this conversion from a bool value, this method will have no effect.</remarks>
        public void Reset() { }

        /// <summary>Updates the node's provoked state during node evaluation.</summary>
        /// <returns>True indicates that the value changed, false otherwise.</returns>
        private bool updateTrigger() {
            bool boolValue = this.source?.Value.Value ?? false;
            if (boolValue != this.Provoked) {
                this.Provoked = boolValue;
                return true;
            }
            return false;
        }

        /// <summary>Evaluates this node and updates it.</summary>
        /// <returns>
        /// The set of all the children if the value changed during update.
        /// If the value hasn't changed then no children are returned.
        /// </returns>
        sealed public override IEnumerable<IEvaluatable> Eval() =>
            this.updateTrigger() ? this.Children.OfType<IEvaluatable>() : Enumerable.Empty<IEvaluatable>();

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "BoolAsTrigger";
    }
}
