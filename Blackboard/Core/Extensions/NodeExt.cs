using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
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

        #endregion
        #region IChild...

        /// <summary>This is a helper method for setting a parent to the node.</summary>
        /// <remarks>
        /// This is only intended to be called by the given child internally.
        /// Do not add this child to the new parent yet,
        /// so we can read from the parents when only evaluating.
        /// This allows nodes which aren't parents for nodes like select.
        /// </remarks>
        /// <typeparam name="T">The node type for the parent.</typeparam>
        /// <param name="child">The child to set the parent to.</param>
        /// <param name="node">The parent node variable being set.</param>
        /// <param name="newParent">The new parent being set, or null</param>
        /// <returns>True if the parent has changed, false otherwise.</returns>
        static internal bool SetParent<T>(this IChild child, ref T node, T newParent)
            where T : INode {
            if (ReferenceEquals(node, newParent)) return false;
            if (node is IParent parent) parent?.RemoveChildren(child);
            node = newParent;
            return true;
        }

        /// <summary>
        /// Adding this node into the parents will make this node be automatically called when
        /// the parents are updated. This should be done if the node is part of a definition.
        /// Do not add to parents if the node is only suppose to evaluate, that way we aren't updating what
        /// we don't need to update and aren't constantly adding and removing children from parents.
        /// </summary>
        /// <param name="child">The child to add to the parents.</param>
        static public void AddToParents(this IChild child) {
            foreach (IParent parent in child.Parents) parent?.AddChildren(child);
        }

        /// <summary>Checks if any parent doesn't contain this child.</summary>
        /// <param name="child">The child to check.</param>
        /// <returns>True if any parent doesn't contain this child, false otherwise.</returns>
        static public bool NeedsToAddParents(this IChild child) =>
            !child.Parents.All(parent => parent.Children.Contains(child));

        #endregion
        #region INaryChild...

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
        /// <param name="parents">The set of parents to add.</param>
        static internal void AddParents<T>(this List<T> sources, IEnumerable<T> parents)
            where T : class, IParent =>
            sources.AddRange(parents.NotNull());

        /// <summary>This removes the given parents from this node.</summary>
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
        /// of the driver so that they will be evaluated in the next batch.
        /// </remarks>
        /// <param name="children">The children to add.</param>
        static public void AddChildren(this IParent node, params IChild[] children) =>
            node.AddChildren(children);

        /// <summary>Removes all the given children from this node if they exist.</summary>
        /// <param name="children">The children to remove.</param>
        static public void RemoveChildren(this IParent node, params IChild[] children) =>
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
        /// <typeparam name="T">The type of evaluable node being worked with.</typeparam>
        /// <param name="list">The list of values to sort insert into.</param>
        /// <param name="nodes">The set of nodes to insert.</param>
        static public void SortInsertUnique<T>(this LinkedList<T> list, IEnumerable<T> nodes)
            where T : IEvaluable {
            foreach (T node in nodes) {
                bool addToEnd = true;
                for (LinkedListNode<T> pend = list.First; pend is not null; pend = pend.Next) {
                    if (ReferenceEquals(node, pend.Value)) {
                        addToEnd = false;
                        break;
                    }
                    if (node.Depth < pend.Value.Depth) {
                        list.AddBefore(pend, node);
                        addToEnd = false;
                        break;
                    }
                }
                if (addToEnd) list.AddLast(node);
            }
        }

        /// <summary>Gets the maximum depth from the given nodes.</summary>
        /// <param name="nodes">The nodes to get the maximum depth from.</param>
        /// <returns>The maximum found depth.</returns>
        static public int MaxDepth(this IEnumerable<IEvaluable> nodes) =>
            nodes.NotNull().Select((node) => node.Depth).Aggregate(0, S.Math.Max);

        #endregion
        #region IFieldReader...

        /// <summary>Removes fields from this node by name if they exist.</summary>
        /// <param name="node">The field writer to remove fields from.</param>
        /// <param name="names">The names of the fields to remove.</param>
        /// <returns>True if the fields were removed, false otherwise.</returns>
        static public bool RemoveFields(this IFieldWriter node, params string[] names) =>
            node.RemoveFields(names);

        #endregion
        #region IFieldWriter...

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
