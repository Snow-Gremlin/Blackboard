using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Nodes.Collections;

/// <summary>This is a collection of parent nodes for an IChild.</summary>
/// <remarks>
/// This should not contain null parents, but it might contain repeat parents.
/// For example, if a number is the sum of itself (x + x), then the Sum node will return the 'x' parent twice.
/// Since null parents are removed, this list may not be the same as the Count even if fixed.
/// </remarks>
sealed internal partial class ParentCollection : IEnumerable<IParent?> {
 
    /// <summary>The parameters for each fixed parent in this collection.</summary>
    private readonly List<IFixedParent> fixedParents;

    /// <summary>The list of variable parents.</summary>
    /// <remarks>This will be null if there are no variable parents.</remarks>
    private IVarParent? varParents;

    /// <summary>Creates a new parent collection.</summary>
    /// <param name="child">The child node that this collection is for.</param>
    /// <param name="fixedCapacity">
    /// The initial capacity for the fixed parent part.
    /// This should be set when this parent collection has a fixed part.
    /// </param>
    internal ParentCollection(IChild child, int fixedCapacity = 0) {
        this.Child        = child;
        this.fixedParents = new List<IFixedParent>(fixedCapacity);
        this.varParents   = null;
    }

    /// <summary>This adds a new parent parameter type to this fixed parameter list.</summary>
    /// <remarks>
    /// The fixed parents will be in the order that this method is called.
    /// Fixed parents will always be before the variable parent no matter the order.
    /// </remarks>
    /// <typeparam name="T">The type of the parent for this parameter.</typeparam>
    /// <param name="getParent">The getter to get the parent value from the child.</param>
    /// <param name="setParent">
    /// The setter to set a parent value to the child.
    /// This should directly set the member without additional processing on the current value.
    /// The current and new parent will have already been processes as needed before being set.
    /// </param>
    /// <returns>This returns the fixed parent which this was called on so that calls can be chained.</returns>
    internal ParentCollection With<T>(S.Func<T?> getParent, S.Action<T?> setParent)
        where T : class, IParent {
        this.fixedParents.Add(new FixedParent<T>(getParent, setParent));
        return this;
    }

    /// <summary>This sets a new parent parameter type to the variable parameter.</summary>
    /// <typeparam name="T">The type of the parent for this parameter.</typeparam>
    /// <param name="source">
    /// The source for all the variable parents. This list will be read from and modified
    /// so should be the list inside of the variable parent node.
    /// </param>
    /// <param name="min">The optional inclusive minimum allowed number of variable parents.</param>
    /// <param name="max">The optional inclusive maximum allowed number of variable parents.</param>
    /// <returns>This returns the fixed parent which this was called on so that calls can be chained.</returns>
    internal ParentCollection With<T>(List<T> source, int min = 0, int max = int.MaxValue)
        where T : class, IParent {
        if (this.HasVariable)
            throw this.newException("May not set a variable parent part after one has already been set.");
        this.varParents = new VarParent<T>(source, min, max);
        return this;
    }

    /// <summary>Creates an exception with information about this parent collection already added to it.</summary>
    /// <param name="format">The format for the message to create.</param>
    /// <param name="args">The arguments to fill out the format for the message.</param>
    /// <returns>The new exception with the parent context.</returns>
    private BlackboardException newException(string format, params object[] args) {
        BlackboardException ex = new BlackboardException(format, args).
            With("child", this.Child);
        if (this.HasVariable)
            ex.With("variable count", this.VarCount).
               With("minimum count",  this.MinimumCount).
               With("maximum count",  this.MaximumCount);
        if (this.HasFixed) ex.With("fixed count", this.FixedCount);
        if (this.HasFixed && this.HasVariable) ex.With("total count", this.Count);
        return ex;
    }

    /// <summary>The child that this parent collection is from.</summary>
    public IChild Child { get; }

    /// <summary>The set of type that each parent could have in this collection.</summary>
    /// <remarks>
    /// For nodes with variable length parents, this can be very long
    /// so use this with zip or limit the enumerations.
    /// </remarks>
    public IEnumerable<S.Type> Types {
        get {
            IEnumerable<S.Type> e = this.fixedParents.Select(p => p.Type);
            if (this.varParents is not null)
                e = e.Concat(Enumerable.Repeat(this.varParents.Type, this.varParents.Maximum));
            return e;
        }
    }

    /// <summary>This gets the enumerator for all the parents currently set.</summary>
    /// <remarks>
    /// This will at return enough null parents to fill the minimum parent count
    /// if there aren't enough variable parents to do so.
    /// </remarks>
    private IEnumerable<IParent?> parents {
        get {
            IEnumerable<IParent?> e = this.fixedParents.Select(p => p.Node);
            if (this.varParents is not null) {
                e = e.Concat(this.varParents);
                int minDiff = this.varParents.Minimum - this.varParents.Count;
                if (minDiff > 0)
                    e = e.Concat(Enumerable.Repeat<IParent?>(null, minDiff));
            }
            return e;
        }
    }

    /// <summary>The set of parent nodes to this node.</summary>
    /// <remarks>
    /// This should not contain null parents, but it might contain repeat parents.
    /// For example, if a number is the sum of itself (x + x),
    /// then the Sum node will return the 'x' parent twice.
    /// </remarks>
    /// <returns>An enumerator for all the parents.</returns>
    public IEnumerator<IParent?> GetEnumerator() => this.parents.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>Determines if there is one or more parent even if they are null.</summary>
    public bool HasParents =>
        this.fixedParents.Count > 0 ||
        (this.varParents is not null && this.varParents.Count > 0);

    /// <summary>Indicates if this collection has a fixed parent part.</summary>
    public bool HasFixed => this.fixedParents.Count > 0;

    /// <summary>Indicates if this collection has a variable parent part.</summary>
    /// <remarks>The parent collection may still have a variable part even if that part is empty.</remarks>
    public bool HasVariable => this.varParents is not null;

    /// <summary>The number of parents currently in the fixed part of the collection.</summary>
    public int FixedCount => this.fixedParents.Count;

    /// <summary>The number of parents currently in the variable part of the collection.</summary>
    public int VarCount => this.varParents?.Count ?? 0;

    /// <summary>The number of parents currently in the collection including null parents.</summary>
    public int Count => this.FixedCount + this.VarCount;

    /// <summary>The maximum allowed number of parents in this collection.</summary>
    public int MaximumCount => this.FixedCount + (this.varParents?.Maximum ?? 0);

    /// <summary>The minimum allowed number of parents in this collection.</summary>
    public int MinimumCount => this.FixedCount + (this.varParents?.Minimum ?? 0);

    /// <summary>This gets the type of the parent at the given index.</summary>
    /// <param name="index">The index to get the parent's type of.</param>
    /// <returns>The type of the parent at the given index.</returns>
    public S.Type TypeAt(int index) =>
        (index < 0 || (!this.HasVariable && index >= this.FixedCount) ?
        throw this.newException("Index out of bounds of node's parent types.").
            With("index", index) :
        index < this.FixedCount ? this.fixedParents[index].Type :
        this.varParents?.Type) ??
            throw this.newException("Null parent from node's parent types.");

    /// <summary>This gets or sets the parent at the given location.</summary>
    /// <remarks>This will throw an exception if out-of-bounds or wrong type.</remarks>
    /// <param name="index">The index to get or set from.</param>
    /// <returns>The parent gotten from the given index. A parent maybe null.</returns>
    public IParent? this[int index] {
        get => index < 0 || index >= this.Count ?
            throw this.newException("Index out of bounds of node's parents.").
                With("index", index) :
            index < this.FixedCount ? this.fixedParents[index].Node :
                this.varParents?[index - this.FixedCount];
        set {
            IParent? parent = this[index]; // This also will check bounds
            if (ReferenceEquals(parent, value)) return;

            if (value is not null) {
                S.Type? type = this.TypeAt(index);
                if (!value.GetType().IsAssignableTo(type))
                    throw this.newException("Incorrect type of a parent being set to a node.").
                        With("index", index).
                        With("expected type", type).
                        With("new parent", value);
            }

            bool removed = parent?.RemoveChildren(this.Child) ?? false;
            if (index < this.FixedCount)
                this.fixedParents[index].Node = value;
            else if (this.varParents is not null) {
                if (value is null)
                    this.varParents.Remove(index - this.FixedCount);
                else this.varParents[index - this.FixedCount] = value;
            }
            if (removed) value?.AddChildren(this.Child);
        }
    }

    #region Replace Method...

    /// <summary>This will check if a replace would work and would make a change in the fixed parent parts.</summary>
    /// <param name="oldParent">The old parent to find all instances with.</param>
    /// <param name="newParent">The new parent to replace each instance with.</param>
    /// <returns>True if any parents would be replaced, false otherwise.</returns>
    private bool checkFixReplace(IParent? oldParent, IParent? newParent) {
        bool wouldChange = false;
        for (int i = this.FixedCount-1; i >= 0; --i) {
            IFixedParent param = this.fixedParents[i];
            if (!ReferenceEquals(param.Node, oldParent) || ReferenceEquals(param.Node, newParent)) continue;

            if (newParent is not null && !newParent.GetType().IsAssignableTo(param.Type))
                throw this.newException("Unable to replace old parent with new parent in fixed parent part.").
                    With("index", i).
                    With("old parent", oldParent).
                    With("new parent", newParent).
                    With("target type", param.Type);
            wouldChange = true;
        }
        return wouldChange;
    }

    /// <summary>This will check if a replace would work and would make a change in the variable parent part.</summary>
    /// <param name="oldParent">The old parent to find all instances with.</param>
    /// <param name="newParent">The new parent to replace each instance with.</param>
    /// <returns>True if any parents would be replaced, false otherwise.</returns>
    private bool checkVarReplace(IParent? oldParent, IParent? newParent) {
        if (this.varParents is null) return false;
        for (int i = this.VarCount-1; i >= 0; --i) {
            IParent parent = this.varParents[i];
            if (!ReferenceEquals(parent, oldParent) || ReferenceEquals(parent, newParent)) continue;

            if (newParent is not null && !newParent.GetType().IsAssignableTo(this.varParents.Type))
                throw this.newException("Unable to replace old parent with new parent in variable parent part.").
                    With("index", i + this.FixedCount).
                    With("old parent", oldParent).
                    With("new parent", newParent).
                    With("target type", this.varParents.Type);

            // Since the type doesn't change across a variable parent list,
            // once we know one will be replaced we no longer need to check the rest, so leave.
            return true;
        }
        return false;
    }

    /// <summary>This performs replace on the fixed parents.</summary>
    /// <param name="oldParent">The old parent to find all instances with.</param>
    /// <param name="newParent">The new parent to replace each instance with.</param>
    private void replaceFix(IParent? oldParent, IParent? newParent) {
        for (int i = this.fixedParents.Count - 1; i >= 0; --i) {
            IFixedParent parent = this.fixedParents[i];
            if (!ReferenceEquals(parent.Node, oldParent)) continue;
            parent.Node = newParent;
        }
    }

    /// <summary>This performs replace on the variable parents.</summary>
    /// <param name="oldParent">The old parent to find all instances with.</param>
    /// <param name="newParent">The new parent to replace each instance with.</param>
    private void replaceVar(IParent? oldParent, IParent? newParent) {
        if (this.varParents is null) return;
        for (int i = this.varParents.Count - 1; i >= 0; --i) {
            IParent node = this.varParents[i];
            if (!ReferenceEquals(node, oldParent)) continue;
            if (newParent is not null)
                this.varParents[i] = newParent;
            else this.varParents.Remove(i);
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
    public bool Replace(IParent? oldParent, IParent? newParent) {
        if (ReferenceEquals(oldParent, newParent)) return false;

        bool fixChange = this.checkFixReplace(oldParent, newParent);
        bool varChange = this.checkVarReplace(oldParent, newParent);
        if (!fixChange && !varChange) return false;

        if (fixChange) this.replaceFix(oldParent, newParent);
        if (varChange) this.replaceVar(oldParent, newParent);

        if (oldParent?.RemoveChildren(this.Child) ?? !this.Child.Illegitimate())
            newParent?.AddChildren(this.Child);
        return true;
    }

    #endregion
    #region SetAll Method...

    /// <summary>Checks if there would be any change from a SetAll.</summary>
    /// <param name="newParents">The parents to set.</param>
    /// <returns>True if it would change, false if it has n change.</returns>
    private bool setAllIsChanged(List<IParent?> newParents) {
        if (newParents.Count != this.Count) return true;
        int i= 0;
        foreach (IParent? oldParent in this.parents) {
            if (!ReferenceEquals(oldParent, newParents[i])) return true;
            i++;
        }
        return false;
    }

    /// <summary>Verifies that the SetAll has the correct types.</summary>
    /// <param name="newParents">The parents to set.</param>
    private void verifySetAll(List<IParent?> newParents) {
        int newCount = newParents.Count;

        if (newCount < this.FixedCount || (!this.HasVariable && newCount > this.FixedCount))
            throw this.newException("Incorrect number of parents in the list of parents to set to a node.").
                With("new parent count", newCount);

        if (newCount < this.MinimumCount)
            throw this.newException("The number of parents to set is less than the minimum allowed count.").
                With("new parent count", newCount);

        if (newCount > this.MaximumCount)
            throw this.newException("The number of parents to set is more than the maximum allowed count.").
                With("new parent count", newCount);

        int i = 0;
        foreach (S.Type type in this.Types) {
            if (i >= newCount) break;
            IParent? newParent = newParents[i];
            if (newParent is null) {
                if (i >= this.FixedCount)
                    throw this.newException("May not have a null parent in the variable parents.").
                        With("index", i).
                        With("expected type", type);
            } else if (!newParent.GetType().IsAssignableTo(type))
                throw this.newException("Incorrect type of a parent in the list of parents to set to a node.").
                    With("index", i).
                    With("expected type", type).
                    With("new parent", newParent);
            i++;
        }
    }

    /// <summary>This attempts to set all the parents in a node.</summary>
    /// <remarks>This will throw an exception if there isn't the correct count or types.</remarks>
    /// <param name="newParents">The parents to set.</param>
    /// <returns>True if any parents changed, false if they were all the same.</returns>
    public bool SetAll(List<IParent?> newParents) {
        if (!this.setAllIsChanged(newParents)) return false;
        this.verifySetAll(newParents);

        bool illegitimate = this.Child.Illegitimate();
            
        this.parents.NotNull().WhereNot(newParents.Contains).
            Distinct().Foreach(p => p.RemoveChildren(this.Child));
         
        if (!illegitimate)
            newParents.NotNull().WhereNot(this.parents.Contains).
                Distinct().Foreach(p => p.AddChildren(this.Child));

        for (int i = 0; i < this.FixedCount; ++i)
            this.fixedParents[i].Node = newParents[i];

        this.varParents?.Remove(0, this.VarCount);
        this.varParents?.Insert(0, newParents.Skip(this.FixedCount).NotNull());

        return true;
    }

    #endregion
    #region Insert Method...

    /// <summary>This inserts new parents into the given location.</summary>
    /// <remarks>This will throw an exception for fixed parent collections.</remarks>
    /// <param name="index">The index to insert the new parents into.</param>
    /// <param name="newParents">The set of new parents to insert.</param>
    /// <param name="oldChild">
    /// If the parents were being moved from one node onto another node,
    /// then this is the old child to remove when applying the parents to this node.
    /// If the old child has been removed from the parent, then this child is added to the parent.
    /// If there is no old child then this child is not set to the new parents.
    /// </param>
    /// <returns>True if any parents were added, false otherwise.</returns>
    public bool Insert(int index, IEnumerable<IParent?> newParents, IChild? oldChild = null) {
        if (index < 0 || index > this.Count)
            throw this.newException("Index out of bounds of node's parents.").
                With("index", index);

        if (index < this.FixedCount)
            throw this.newException("May not insert a parent into the fixed parent part.").
                With("index", index);

        int newCount = newParents.Count();
        if (newCount <= 0) return false;

        if (this.Count + newCount > this.MaximumCount || this.varParents is null)
            throw this.newException("Inserting the given number of parents would cause there to be more than the maximum allowed count.").
                With("new parent count", newCount);

        S.Type type = this.varParents.Type;
        foreach ((IParent? newParent, int i) in newParents.WithIndex()) {
            if (newParent is null)
                throw this.newException("May not insert a null parent into the variable parents.").
                    With("insert index", index).
                    With("new parent index", i).
                    With("expected type", type);
            else if (!newParent.GetType().IsAssignableTo(type))
                throw this.newException("Incorrect type of a parent in the list of parents to insert into a node.").
                    With("insert index", index).
                    With("new parent index", i).
                    With("expected type", type).
                    With("new parent", newParent);
        }

        if (oldChild is not null) {
            // For any new parent which was a parent of the old child,
            // remove the old child and add this child to that parent.
            newParents.NotNull().Where(p => p.RemoveChildren(oldChild)).
                Foreach(p => p.AddChildren(this.Child));
        }

        this.varParents.Insert(index-this.FixedCount, newParents.NotNull());
        return true;
    }

    #endregion
    #region Remove Method...

    /// <summary>This remove one or more parent at the given location.</summary>
    /// <remarks>This will throw an exception for fixed parent collections.</remarks>>
    /// <param name="index">The index to start removing the parents from.</param>
    /// <param name="length">The number of parents to remove.</param>
    /// <returns>True if any parents were removed, false otherwise.</returns>
    public bool Remove(int index, int length = 1) {
        if (length <= 0) return false;

        if (index < 0 || index + length > this.Count)
            throw this. newException("Index, with length taken into account, is out of bounds of node's parents.").
                With("index", index).
                With("length", length);

        if (index < this.FixedCount)
            throw this.newException("May not remove a parent from the fixed parent part.").
                With("index", index).
                With("length", length);

        if (this.Count - length < this.MinimumCount)
            throw this.newException("Removing the given number of parents would cause there to be fewer than the minimum allowed count.").
                With("index", index).
                With("length", length);
        
        // Because the above passed, the variable parents list should not be null.
        IVarParent? varPars = this.varParents;
        if (varPars is null) return false;

        // Adjust the index to the var list itself.
        index -= this.FixedCount;

        // Collect a set of non-null parents which will be removed.
        HashSet<IParent> remove = varPars.GetRange(index, length).NotNull().ToHashSet();

        // Remove the range of parents.
        varPars.Remove(index, length);
        
        // Remove the child from the removed parent, if there isn't another copy of the parent not being removed.
        remove.WhereNot(this.Contains).Foreach(p => p.RemoveChildren(this.Child));
        return true;
    }

    #endregion
}
