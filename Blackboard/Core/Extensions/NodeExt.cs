using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Inspect;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using Blackboard.Core.Nodes.Outer;
using System.Collections.Generic;
using System.Linq;
using S = System;

namespace Blackboard.Core.Extensions {

    /// <summary>The set of extensions for working with different types of Nodes.</summary>
    static public class NodeExt {
        #region INode...

        /// <summary>The values from the given input values.</summary>
        /// <typeparam name="T">The type of the values to get.</typeparam>
        /// <param name="nodes">The set of nodes to get all the values from.</param>
        /// <returns>The values from the given non-null nodes.</returns>
        static public IEnumerable<T> Values<T>(this IEnumerable<IValue<T>> nodes)
            where T : IData =>
            from node in nodes select node is not null ? node.Value : default;

        /// <summary>The triggers from the given input nodes.</summary>
        /// <param name="nodes">The set of nodes to get all the triggers from.</param>
        /// <returns>The triggers from the given non-null nodes.</returns>
        static public IEnumerable<bool> Triggers(this IEnumerable<ITrigger> nodes) =>
            from node in nodes select node?.Provoked ?? false;

        /// <summary>Determines if the node is constant or if all of it's parents are constant.</summary>
        /// <param name="node">The node to check if constant.</param>
        /// <returns>True if constant, false otherwise.</returns>
        static public bool IsConstant(this INode node) =>
            node is IConstant ||
            (node is not IInput && node is IChild child && child.Parents.IsConstant());

        /// <summary>This determines if all the given nodes are constant.</summary>
        /// <param name="nodes">The nodes to check if constant.</param>
        /// <returns>True if all nodes are constant, false otherwise.</returns>
        static public bool IsConstant(this IEnumerable<INode> nodes) =>
            nodes.All(node => IsConstant(node));

        /// <summary>Converts this node to a constant.</summary>
        /// <remarks>
        /// This will make a constant with the current value of the node.
        /// Evaluate the correct node value before converting it to a constant.
        /// </remarks>
        /// <param name="node">The node to try to convert into a constant.</param>
        /// <returns>A constant of this node or null if not able to be made into a constant.</returns>
        static public IConstant ToConstant(this INode node) =>
            node switch {
                IConstant c => c,
                IDataNode v => Literal.Data(v.Data),
                ITrigger  t => new ConstTrigger(t.Provoked),
                _           => null,
            };

        /// <summary>This will reset all the given trigger nodes.</summary>
        /// <param name="nodes">The trigger nodes to reset.</param>
        static public void Reset(this IEnumerable<ITrigger> nodes) =>
            nodes.NotNull().Foreach(t => t.Reset());

        /// <summary>
        /// This will get all the non-virtual nodes from the given nodes.
        /// For any virtual nodes, the real node (receiver) inside of it is returned.
        /// </summary>
        /// <param name="nodes">The nodes to actualize.</param>
        /// <returns>All real nodes and the receivers from any virtual nodes.</returns>
        static public IEnumerable<INode> Actualize(this IEnumerable<INode> nodes) =>
            nodes.Select(node => node is VirtualNode virt ? virt.Receiver : node);

        #endregion
        #region IChild...

        /// <summary>
        /// Adding this node into the parents will make this node be automatically called when
        /// the parents are updated. This should be done if the node is part of a definition.
        /// Do not add to parents if the node is only suppose to evaluate, that way we aren't updating what
        /// we don't need to update and aren't constantly adding and removing children from parents.
        /// </summary>
        /// <param name="child">The child to add to the parents.</param>
        /// <returns>True if this child was added to any parent.</returns>
        static public bool AddToParents(this IChild child) {
            bool anyAdded = false;
            foreach (IParent parent in child.Parents)
                anyAdded = (parent?.AddChildren(child) ?? false) || anyAdded;
            return anyAdded;
        }

        /// <summary>Checks if any parent doesn't contain this child.</summary>
        /// <param name="child">The child to check.</param>
        /// <returns>True if any parent doesn't contain this child, false otherwise.</returns>
        static public bool NeedsToAddParents(this IChild child) =>
            !child.Parents.All(parent => parent.Children.Contains(child));

        /// <summary>This adds parents to this node.</summary>
        /// <typeparam name="T">The type of parent used for this child.</typeparam>
        /// <param name="parents">The set of parents to add.</param>
        static public void AddParents<T>(this INaryChild<T> child, params T[] parents)
            where T : IParent => child.AddParents(parents);

        /// <summary>This removes the given parents from this node.</summary>
        /// <typeparam name="T">The type of parent used for this child.</typeparam>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        static public bool RemoveParents<T>(this INaryChild<T> child, params T[] parents)
            where T : IParent => child.RemoveParents(parents);

        #endregion
        #region IParent...

        /// <summary>This adds parents to this node.</summary>
        /// <remarks>
        /// This is only intended to be called by the given child internally.
        /// Do not add this child to the new parent yet,
        /// so we can read from the parents when only evaluating.
        /// </remarks>
        /// <param name="sources">The node source list to add the parents to.</param>
        /// <param name="parents">The set of parents to add.</param>
        static internal void AddParents<T>(this List<T> sources, IEnumerable<T> parents)
            where T : class, IParent =>
            sources.AddRange(parents.NotNull());

        /// <summary>This removes the given parents from this node collection.</summary>
        /// <param name="sources">The node source list to remove the parents from.</param>
        /// <param name="child">The child this sources are for.</param>
        /// <param name="parents">The set of parents to remove.</param>
        /// <returns>True if any of the parents are removed, false if none were removed.</returns>
        static internal bool RemoveParents<T>(this List<T> sources, IChild child, IEnumerable<T> parents)
            where T : class, IParent {
            bool anyRemoved = false;
            foreach (T parent in parents.NotNull()) {
                if (sources.Remove(parent)) {
                    parent.RemoveChildren(child);
                    anyRemoved = true;
                }
            }
            return anyRemoved;
        }

        /// <summary>This replaces all instances of given parent from this node collection.</summary>
        /// <param name="sources">The node source list to replace the parents inside of.</param>
        /// <param name="child">The child these sources are for.</param>
        /// <param name="oldParent">The old parent to replace.</param>
        /// <param name="newParent">The new parent to replace the old parent with.</param>
        /// <returns>True if any of the parents are replaced, false if none were removed.</returns>
        static internal bool ReplaceParents<T>(this List<T> sources, IChild child, IParent oldParent, IParent newParent)
            where T : class, IParent {
            bool replaced = false;
            bool typeChecked = false;
            for (int i = sources.Count - 1; i >= 0; i--) {
                T node = sources[i];
                if (!ReferenceEquals(node, oldParent)) continue;

                // Now that at least one parent will be replaced, check that the new parent can be used.
                if (!typeChecked && newParent is not null and not T)
                    throw new Exception("Unable to replace old parent with new parent in a list.").
                        With("child", child).
                        With("index", i).
                        With("node", node).
                        With("old Parent", oldParent).
                        With("new Parent", newParent);
                typeChecked = true;

                // Replace parent in list of sources.
                node?.RemoveChildren(child);
                sources[i] = newParent as T;
                replaced = true;
            }
            return replaced;
        }

        /// <summary>This will check if from the given root node any of the given target nodes can be reachable.</summary>
        /// <param name="root">The root to start checking from.</param>
        /// <param name="targets">The target nodes to try to reach.</param>
        /// <returns>True if any of the targets can be reached, false otherwise.</returns>
        static public bool CanReachAny(this IParent root, IEnumerable<IParent> targets) {
            HashSet<IParent> reached = new();
            Queue<IParent> pending = new();
            pending.Enqueue(root);
            reached.Add(root);

            while (pending.Count > 0) {
                IParent node = pending.Dequeue();
                if (targets.Contains(node)) return true;

                if (node is IChild child) {
                    foreach (IParent parent in child.Parents.NotNull().WhereNot(reached.Contains)) {
                        pending.Enqueue(parent);
                        reached.Add(parent);
                    }
                }
            }
            return false;
        }

        /// <summary>Adds children nodes onto this node.</summary>
        /// <remarks>
        /// Any children which are added need to be put in the pending evaluation list
        /// of the slate so that they will be evaluated and/or depth updated in the next batch.
        /// </remarks>
        /// <param name="children">The children to add.</param>
        /// <returns>True if any children were added, false otherwise.</returns>
        static public bool AddChildren(this IParent node, params IChild[] children) =>
            node.AddChildren(children);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        /// <returns>True if any children were removed, false otherwise.</returns>
        static public bool RemoveChildren(this IParent node, params IChild[] children) =>
            node.RemoveChildren(children);

        #endregion
        #region Evaluable...

        /// <summary>This sort inserts unique nodes into the given linked list.</summary>
        /// <typeparam name="T">The type of evaluable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique<T>(this LinkedList<T> list, params T[] nodes)
            where T : IEvaluable =>
            list.SortInsertUnique(nodes as IEnumerable<T>);

        /// <summary>This sort inserts unique evaluable nodes into the given linked list.</summary>
        /// <remarks>This assumes that lower depth nodes will be added after their parents typically.</remarks>
        /// <typeparam name="T">The type of evaluable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique<T>(this LinkedList<T> list, IEnumerable<T> nodes)
            where T : IEvaluable {
            if (nodes is null) return;
            foreach (T node in nodes) {
                bool addToEnd = true;
                for (LinkedListNode<T> pend = list.Last; pend is not null; pend = pend.Previous) {
                    if (ReferenceEquals(node, pend.Value)) {
                        addToEnd = false;
                        break;
                    }
                    if (node.Depth > pend.Value.Depth) {
                        list.AddAfter(pend, node);
                        addToEnd = false;
                        break;
                    }
                }
                if (addToEnd) list.AddFirst(node);
            }
        }

        /// <summary>This updates the depth values of the given pending nodes.</summary>
        /// <remarks>
        /// The given list will be emptied by this call. The pending nodes are expected to be
        /// presorted by depth which will usually provide the fastest update.
        /// </remarks>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        /// <param name="logger">The logger to debug the update with.</param>
        static public void UpdateDepths(this LinkedList<IEvaluable> pending, ILogger logger = null) {
            logger?.Log("Start Update (pending: {0})", pending.Count);

            while (pending.Count > 0) {
                IEvaluable node = pending.TakeFirst();

                // Determine the depth that this node should be at based on its parents.
                int depth = node.MinimumAllowedDepth();

                // If the depth has changed then its children also need to be updated.
                bool changed = false;
                if (node.Depth != depth) {
                    node.Depth = depth;
                    pending.SortInsertUnique(node.Children.OfType<Evaluable>());
                    changed = true;
                }

                logger?.Log("  Updated (changed: {0}, depth: {1}, node: {2}, remaining: {3})", changed, node.Depth, node, pending.Count);
            }

            logger?.Log("End Update");
        }

        /// <summary>Updates and propagates the changes from the given inputs through the blackboard nodes.</summary>
        /// <remarks>
        /// The given list will be emptied by this call. The pending nodes are expected to be
        /// presorted by depth which will usually provide the fastest update.
        /// </remarks>
        /// <param name="pending">The initial set of nodes which are pending depth update.</param>
        /// <param name="logger">The logger to debug the evaluate with.</param>
        /// <returns>The list of provoked triggers</returns>
        static public HashSet<ITrigger> Evaluate(this LinkedList<IEvaluable> pending, ILogger logger = null) {
            logger?.Log("Start Eval (pending: {0})", pending.Count);

            HashSet<ITrigger> provoked = new();
            while (pending.Count > 0) {
                IEvaluable node = pending.TakeFirst();

                bool changed = false;
                if (node.Evaluate()) {
                    pending.SortInsertUnique(node.Children.NotNull().OfType<IEvaluable>());
                    if (node is ITrigger trigger && trigger.Provoked)
                        provoked.Add(trigger);
                    changed = true;
                }

                logger?.Log("  Evaluated (changed: {0}, depth: {1}, node: {2}, remaining: {3})", changed, node.Depth, node, pending.Count);
            }

            logger?.Log("End Eval (provoked: {0})", provoked.Count);
            return provoked;
        }

        /// <summary>Gets the maximum depth from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the maximum depth from.</param>
        /// <returns>The maximum found depth.</returns>
        static public int MaxDepth(this IEnumerable<IEvaluable> nodes) =>
            nodes.NotNull().Select((node) => node.Depth).Aggregate(0, S.Math.Max);

        /// <summary>The minimum allowed depth based on the given nodes parents, if it has any.</summary>
        /// <param name="node">The evaluable node to get the minimum allowed depth.</param>
        /// <returns></returns>
        static public int MinimumAllowedDepth(this IEvaluable node) =>
            node is not IChild child ? 0 : child.Parents.OfType<IEvaluable>().MaxDepth() + 1;

        #endregion
        #region Fields...

        /// <summary>Removes fields from this node by name if they exist.</summary>
        /// <param name="node">The field writer to remove fields from.</param>
        /// <param name="names">The names of the fields to remove.</param>
        /// <returns>True if the fields were removed, false otherwise.</returns>
        static public bool RemoveFields(this IFieldWriter node, params string[] names) =>
            node.RemoveFields(names);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="node">The field reader to find a node in.</param>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        static public INode Find(this IFieldReader node, params string[] names) =>
            node.Find(names as IEnumerable<string>);

        /// <summary>Finds the node at the given path.</summary>
        /// <param name="node">The field reader to find a node in.</param>
        /// <param name="names">The names to the node to find.</param>
        /// <returns>The node at the end of the path or null.</returns>
        static public INode Find(this IFieldReader node, IEnumerable<string> names) {
            INode cur = node;
            foreach (string name in names) {
                if (cur is IFieldReader scope) {
                    if (!scope.ContainsField(name)) return null;
                    cur = scope.ReadField(name);
                } else return null;
            }
            return cur;
        }

        #endregion
    }
}
