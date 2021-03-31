using System.Collections.Generic;
using System.Linq;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core {
    static internal class Extensions {

        static public void AddUnique<T>(this List<T> list, params T[] values) =>
            AddUnique(list, values as IEnumerable<T>);

        static public void AddUnique<T>(this List<T> list, IEnumerable<T> values) {
            foreach (T value in values) {
                if (!list.Contains(value)) list.Add(value);
            }
        }

        static public List<T> AddWhichUnique<T>(this List<T> list, params T[] values) =>
            AddWhichUnique(list, values as IEnumerable<T>);

        static public List<T> AddWhichUnique<T>(this List<T> list, IEnumerable<T> values) {
            List<T> result = new List<T>();
            foreach (T value in values) {
                if (!list.Contains(value)) {
                    list.Add(value);
                    result.Add(value);
                }
            }
            return result;
        }

        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> input) where T: class {
            foreach (T value in input) {
                if (value is null) continue;
                yield return value;
            }
        }

        static public IEnumerable<T> Values<T>(this IEnumerable<IValue<T>> nodes) {
            foreach (IValue<T> node in nodes) {
                yield return node is null ? default : node.Value;
            }
        }

        static public bool ContainsAny<T>(this IEnumerable<T> a, IEnumerable<T> b) {
            foreach (T value in a) {
                if (b.Contains(value)) return true;
            }
            return false;
        }

        static public T TakeFirst<T>(this LinkedList<T> list) {
            T value = list.First.Value;
            list.RemoveFirst();
            return value;
        }

        static public void SortInsertUnique(this LinkedList<INode> pending, params INode[] nodes) =>
            pending.SortInsertUnique(nodes as IEnumerable<INode>);

        static public void SortInsertUnique(this LinkedList<INode> pending, IEnumerable<INode> nodes) {
            foreach (INode node in nodes) {
                bool addToEnd = true;
                for (LinkedListNode<INode> pend = pending.First; !(pend is null); pend = pend.Next) {
                    if (ReferenceEquals(node, pend)) {
                        addToEnd = false;
                        break;
                    }
                    if (node.Depth < pend.Value.Depth) {
                        pending.AddBefore(pend, node);
                        addToEnd = false;
                        break;
                    }
                }
                if (addToEnd) pending.AddLast(node);
            }
        }
    }
}
