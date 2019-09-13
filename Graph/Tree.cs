using System.Collections.Generic;
using System.Linq;

namespace Serpen.Uni.Graph
{

    public class TreeNode<T>
    {

        public TreeNode(T Value, TreeNode<T> parent)
        {
            this.Parent = parent;
            this.Value = Value;
            this._Childs = new List<TreeNode<T>>();
        }
        public TreeNode<T> Parent { get; }

        internal List<TreeNode<T>> _Childs;
        public TreeNode<T>[] Childs => _Childs.ToArray();

        public TreeNode<T> AddChild(T c) {
            var n = new TreeNode<T>(c, this);
            _Childs.Add(n);
            return n;
        } 
        public void AddChild(T[] c) {
            for (int i = 0; i < c.Length; i++)
            {
                AddChild(c);
            }
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


    }
    public class Tree<T>
    {
        public Tree(TreeNode<T> StartNode)
        {
            this.StartNode = StartNode;

        }

        public TreeNode<T> StartNode {get;}

        public TreeNode<T>[] TiefenDurchLauf() {
            List<TreeNode<T>> list = new List<TreeNode<T>>();
            list.Add(StartNode);

            TiefenDurchLauf(ref list);
            return list.ToArray();
        }
        
        void TiefenDurchLauf(ref List<TreeNode<T>> list) {
            foreach (var c in list.Last()._Childs) {
                if (!list.Contains(c)) {
                    list.Add(c);
                    TiefenDurchLauf(ref list);
                } else 
                    throw new System.ApplicationException("shouldnt happen!");
            }
        }

        public TreeNode<T>[] BreitenDurchLauf() {
            Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
            List<TreeNode<T>> list = new List<TreeNode<T>>();

            queue.Enqueue(StartNode);

            while (queue.Any()) {
                var r = queue.Dequeue();
                list.Add(r);
                foreach (var c in r._Childs) {
                    if (!list.Contains(c)) {
                        queue.Enqueue(c);   
                    } else 
                        throw new System.ApplicationException("shouldnt happen!");
                    
                }
            }
            return list.ToArray();
        }
    }
}