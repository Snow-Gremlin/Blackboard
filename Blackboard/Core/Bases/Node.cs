using System.Collections.Generic;
using System.Linq;
using Blackboard.Core.Interfaces;

namespace Blackboard.Core.Bases {

    public abstract class Node: INode {

        static public string NodeString(INode node) =>
            node is null ? "null" : node.ToString();

        static public string NodeString(IEnumerable<INode> nodes) =>
            string.Join(", ", nodes.Select((INode node) => NodeString(node)));

        private List<INode> children;

        protected Node() {
            this.children = new List<INode>();
            this.Depth = 0;
        }

        public int Depth { get; private set; }

        public abstract IEnumerable<INode> Eval();

        public abstract IEnumerable<INode> Parents { get; }

        public IEnumerable<INode> Children => this.children;

        public void AddChildren(params INode[] children) =>
            this.AddChildren(children as IEnumerable<INode>);

        public void AddChildren(IEnumerable<INode> children) {
            if (this.containsLoop(children))
                throw new System.Exception("May not add children: Loop detected.");
            LinkedList<INode> needsDepthUpdate = new LinkedList<INode>();
            foreach (Node child in children) {
                if ((child is null) || this.children.Contains(child)) continue;
                this.children.Add(child);
                needsDepthUpdate.SortInsertUnique(child);
            }
            this.updateDepths(needsDepthUpdate);
        }

        public void RemoveChildren(params INode[] children) =>
            this.RemoveChildren(children as IEnumerable<INode>);

        public void RemoveChildren(IEnumerable<INode> children) {
            LinkedList<INode> needsDepthUpdate = new LinkedList<INode>();
            foreach (Node child in children) {
                if ((child is null) || !this.children.Contains(child)) continue;
                this.children.Remove(child);
                needsDepthUpdate.SortInsertUnique(child);
            }
            this.updateDepths(needsDepthUpdate);
        }

        private bool containsLoop(IEnumerable<INode> children) {
            // TODO:  Could the loop check be improved using the depth?
            List<INode> touched = new List<INode>();
            Queue<INode> pending = new Queue<INode>();
            pending.Enqueue(this);
            while (pending.Count > 0) {
                INode node = pending.Dequeue();
                touched.Add(node);
                if (children.Contains(node)) return true;
                foreach (INode parent in node.Parents) {
                    if (!touched.Contains(parent)) pending.Enqueue(parent);
                }
            }
            return false;
        }

        private void updateDepths(LinkedList<INode> pending) {
            while (pending.Count > 0) {
                INode node = pending.TakeFirst();
                int depth = 0;
                foreach (INode parent in node.Parents)
                    depth = System.Math.Max(depth, parent.Depth);
                depth++;
                if (node.Depth != depth) {
                    (node as Node).Depth = depth;
                    pending.SortInsertUnique(node.Children);
                }
            }
        }
    }
}
