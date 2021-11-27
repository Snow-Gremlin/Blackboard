using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;

namespace Blackboard.Core.Nodes.Inner {

    /// <summary>This will return the value of one of two parents based on a boolean parent.</summary>
    /// <remarks>This functions just like a typical ternary statement.</remarks>
    /// <typeparam name="T">The type of input for the two value providing parents.</typeparam>
    sealed public class Select<T>: TernaryValue<Bool, T, T, T>
        where T : IComparable<T> {

        // TODO: Determine if it is possible to select between two triggers (i.e. pick the trigger to listen too.
        //       Also namespace, funcGroups, funcDefs, where it uses one of two but must also match the same interface.

        /// <summary>This is a factory function for creating new instances of this node easily.</summary>
        static public readonly IFuncDef Factory = CreateFactory((test, left, right) => new Select<T>(test, left, right));

        /// <summary>Creates a selection value node.</summary>
        /// <param name="test">This is the first parent for the source value.</param>
        /// <param name="left">This is the second parent for the source value.</param>
        /// <param name="right">This is the third parent for the source value.</param>
        public Select(IValueParent<Bool> test = null, IValueParent<T> left = null, IValueParent<T> right = null) :
            base(test, left, right) { }

        /// <summary>This is the type name of the node.</summary>
        public override string TypeName => "Select";

        /// <param name="test">
        /// The parent to select with,
        /// true will return the second parent's value,
        /// false will return the third parent's value.
        /// </param>
        /// <param name="left">The value to return if the first parent is true.</param>
        /// <param name="right">The value to return if the first parent is false.</param>
        /// <returns>The selected value to set to this node.</returns>
        protected override T OnEval(Bool test, T left, T right) => test.Value ? left : right;
    }
}
