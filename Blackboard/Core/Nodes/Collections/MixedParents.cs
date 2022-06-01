using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections {

    /// <summary>
    /// This is a parent collection for a child which has a
    /// fixed number of parents (unary, binary, and ternary).
    /// </summary>
    internal class MixedParents<TVar> : IParentCollection
            where TVar : class, IParent {

        /// <summary>The parameters for each parent in this fixed part of the collection.</summary>
        private readonly FixedParents fixedParents;

        /// <summary>The parameters for each parent in this variable part of the collection.</summary>
        private readonly VarParents<TVar> varParents;

        /// <summary>Constructs a new fixed parent collection.</summary>
        /// <remarks>This is created with no parents. Use the `With` method to add a parent parameter.</remarks>
        /// <param name="child">The child this parent collection is for.</param>
        /// <param name="varSource">The list from the child containing the variable parents.</param>
        public MixedParents(IChild child, List<TVar> varSource) {
            this.Child = child;
            this.fixedParents = new FixedParents(child);
            this.varParents = new VarParents<TVar>(child, varSource);
        }

        /// <summary>This adds a new parent parameter type to this fixed parameter list.</summary>
        /// <typeparam name="T">The type of the parent for this parameter.</typeparam>
        /// <param name="getParent">The getter to get the parent value from the child.</param>
        /// <param name="setParent">
        /// The setter to set a parent value to the child.
        /// This should directly set the member without additional processing on the current value.
        /// The current and new parent will have already been processes as needed before being set.
        /// </param>
        /// <returns>This returns the fixed parent which this was called on so that calls can be chained.</returns>
        internal MixedParents<TVar> With<T>(S.Func<T> getParent, S.Action<T> setParent)
            where T : class, IParent {
            this.fixedParents.With(getParent, setParent);
            return this;
        }

        /// <summary>The child that this parent collection is from.</summary>
        public IChild Child { get; }

        /// <summary>The set of type that each parent could have in this collection.</summary>
        public IEnumerable<S.Type> Types => this.fixedParents.Types.Concat(this.varParents.Types);

        /// <summary>The set of parent nodes to this node.</summary>
        /// <remarks>
        /// This should not contain null parents, but it might contain repeat parents.
        /// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
        /// </remarks>
        /// <returns>An enumerator for all the parents.</returns>
        public IEnumerator<IParent> GetEnumerator() => this.fixedParents.Concat(this.varParents).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>This is the number of parents in the collection.</summary>
        /// <remarks>Nodes may return a smaller number if any parent is null.</remarks>
        public int Count => this.fixedParents.Count + this.varParents.Count;

        /// <summary>This gets or sets the parent at the given location.</summary>
        /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
        /// <param name="index">The index to get or set from.</param>
        /// <returns>The parent gotten from the given index.</returns>
        public IParent this[int index] {
            get => index < this.fixedParents.Count ? this.fixedParents[index] : this.varParents[index - this.fixedParents.Count];
            set {
                if (index < this.fixedParents.Count) this.fixedParents[index] = value;
                else this.varParents[index - this.fixedParents.Count] = value;
            }
        }

        /// <summary>This replaces all instances of the given old parent with the given new parent.</summary>
        /// <remarks>
        /// The new parent must be able to take the place of the old parent,
        /// otherwise this will throw an exception when attempting the replacement of the old parent.
        /// </remarks>
        /// <param name="oldParent">The old parent to find all instances with.</param>
        /// <param name="newParent">The new parent to replace each instance with.</param>
        /// <returns>True if any parent was replaced, false if that old parent wasn't found.</returns>
        public bool Replace(IParent oldParent, IParent newParent) =>
            this.fixedParents.Replace(oldParent, newParent) |
            this.varParents.Replace(oldParent, newParent);

        /// <summary>This will attempt to set all the parents in a node.</summary>
        /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
        /// <param name="newParents">The parents to set.</param>
        /// <returns>True if any parents changed, false if they were all the same.</returns>
        public bool SetAll(List<IParent> newParents) =>
            this.fixedParents.SetAll(newParents.GetRange(0, this.fixedParents.Count)) |
            this.varParents.SetAll(newParents.GetRange(this.fixedParents.Count, newParents.Count));

        /// <summary>This throws an exception because the collection is fixed.</summary>
        /// <param name="index">The index to insert the parents at.</param>
        /// <param name="newParents">The new parents to insert.</param>
        /// <param name="oldChild">The old child these parents are being moved from.</param>
        /// <returns>Nothing is returned because this will throw an exception.</returns>
        public bool Insert(int index, IEnumerable<IParent> newParents, IChild oldChild = null) {

        }

        /// <summary>This throws an exception because the collection is fixed.</summary>>
        /// <param name="index">The index to start removing the parents from.</param>
        /// <param name="length">The number of parents to remove.</param>
        public void Remove(int index, int length = 1) {

        }

        /// <summary>This creates a string for the given set of parents.</summary>
        /// <returns>The string for this set of parents.</returns>
        public override string ToString() => "[" + this.fixedParents.Join(", ") + "|" + this.varParents.Join(", ") + "]";
    }
}
