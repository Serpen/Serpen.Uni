using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Graph {

    public class TreeNode<T> {

        public TreeNode(T Value, TreeNode<T> parent) {
            this.Parent = parent;
            this.Value = Value;
            this.Childs = new List<TreeNode<T>>();
        }
        internal TreeNode(T Value) {
            this.Parent = null;
            this.Value = Value;
            this.Childs = new List<TreeNode<T>>();
        }
        public TreeNode<T> Parent { get; }

        public ICollection<TreeNode<T>> Childs { get; }

        public TreeNode<T> AddChild(T c) {
            var n = new TreeNode<T>(c, this);
            Childs.Add(n);
            return n;
        }
        public void AddChild(TreeNode<T> c) {
            Childs.Add(c);
        }

        public void AddChild(params T[] c) {
            for (int i = 0; i < c.Length; i++)
                AddChild(c[i]);
        }

        public T Value { get; }

        public override bool Equals(object obj) {
            if (obj is TreeNode<T> tobj) {
                return tobj.GetHashCode() == this.GetHashCode();
            } else
                return false;
        }

        public override int GetHashCode() {
            if (Parent != null)
                return (Parent.ToString() + "\\" + ToString()).GetHashCode();
            else
                return ToString().GetHashCode();
        }

        public override string ToString() {
            return Value.ToString();
        }

        public IEnumerable<TreeNode<T>> TiefenDurchLauf() {
            var list = new List<TreeNode<T>> {
                this
            };

            TiefenDurchLauf(ref list);
            return list.ToArray();
        }

        void TiefenDurchLauf(ref List<TreeNode<T>> list) {
            foreach (var c in list.Last().Childs) {
                if (!list.Contains(c)) {
                    list.Add(c);
                    TiefenDurchLauf(ref list);
                } else
                    throw new System.ApplicationException("shouldnt happen!");
            }
        }

        public IEnumerable<TreeNode<T>> BreitenDurchLauf() {
            var queue = new Queue<TreeNode<T>>();
            var list = new List<TreeNode<T>>();

            queue.Enqueue(this);

            while (queue.Any()) {
                var r = queue.Dequeue();
                list.Add(r);
                yield return r;
                foreach (var c in r.Childs) {
                    if (!list.Contains(c)) {
                        queue.Enqueue(c);
                    } else
                        throw new System.ApplicationException("shouldnt happen!");

                }
            }
        }
    }
}