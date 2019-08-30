using System;
using System.Collections.Generic;

namespace AvlTree
{
    public static class TreePrinter
    {
        class NodeInfo<T>
        {
            public AvlTree<T>.Node Node;
            public string Text;
            public int StartPos;
            public int Size { get { return Text.Length; } }
            public int EndPos { get { return StartPos + Size; } set { StartPos = value - Size; } }
            public NodeInfo<T> Parent, Left, Right;
        }
        
        public static void Print<T>(this  AvlTree<T> root, string textFormat = "0", int spacing = 1, int topMargin = 2, int leftMargin = 2)
        {
            if (root == null) return;
            var next = root.root;
            int rootTop = Console.CursorTop + topMargin;
            var last = new List<NodeInfo<T>>();
            for (int level = 0; next != null; level++)
            {
                var item = new NodeInfo<T> { Node = next, Text = next.ToString(textFormat) };
                if (level < last.Count)
                {
                    item.StartPos = last[level].EndPos + spacing;
                    last[level] = item;
                }
                else
                {
                    item.StartPos = leftMargin;
                    last.Add(item);
                }
                if (level > 0)
                {
                    item.Parent = last[level - 1];
                    if (next == item.Parent.Node.left)
                    {
                        item.Parent.Left = item;
                        item.EndPos = Math.Max(item.EndPos, item.Parent.StartPos - 1);
                    }
                    else
                    {
                        item.Parent.Right = item;
                        item.StartPos = Math.Max(item.StartPos, item.Parent.EndPos + 1);
                    }
                }
                next = next.left ?? next.right;
                for (; next == null; item = item.Parent)
                {
                    int top = rootTop + 2 * level;
                    Print(item.Text, top, item.StartPos);
                    if (item.Left != null)
                    {
                        Print("/", top + 1, item.Left.EndPos);
                        Print("_", top, item.Left.EndPos + 1, item.StartPos);
                    }
                    if (item.Right != null)
                    {
                        Print("_", top, item.EndPos, item.Right.StartPos - 1);
                        Print("\\", top + 1, item.Right.StartPos - 1);
                    }
                    if (--level < 0) break;
                    if (item == item.Parent.Left)
                    {
                        item.Parent.StartPos = item.EndPos + 1;
                        next = item.Parent.Node.right;
                    }
                    else
                    {
                        if (item.Parent.Left == null)
                            item.Parent.EndPos = item.StartPos - 1;
                        else
                            item.Parent.StartPos += (item.StartPos - 1 - item.Parent.EndPos) / 2;
                    }
                }
            }
            Console.SetCursorPosition(0, rootTop + 2 * last.Count - 1);
        }

        private static void Print(string s, int top, int left, int right = -1)
        {
            Console.SetCursorPosition(left, top);
            if (right < 0) right = left + s.Length;
            while (Console.CursorLeft < right) Console.Write(s);
        }
    }
    public class AvlTree<T>
    {

        public Node root;
        int numElements;
        public delegate int CompareDelegate(T v1, T v2);
        public delegate bool EqualDelegate(T v1, T v2);
        public readonly CompareDelegate compare = Comparer<T>.Default.Compare;
        
        public int Count
        {
            get { return numElements; }
        }

        public AvlTree()
        {
            root = null;
            numElements = 0;
        }

        public AvlTree(CompareDelegate compare) : this()
        {
            this.compare = compare;
           
        }

        public void Insert(T value)
        {
            numElements++;
            root = (root == null) ? new Node(value, null) : root.Insert(value, compare);
        }

        public Node Find(T value)
        {
            return root.Find(value, compare);
        }

        public class Node
        {
            public readonly T value;
            public Node parent;
            public Node left;
            public Node right;
            public int height;

            internal Node(T value, Node parent)
            {
                this.value = value;
                this.parent = parent;
                left = null;
                right = null;
                height = 1;
            }

            public Node Insert(T value, CompareDelegate compare)
            {
                if (compare(value, this.value) < 0)
                {
                    return Insert(ref left, value, compare);
                }
                return Insert(ref right, value, compare);
            }

            private Node Insert(ref Node node, T value, CompareDelegate compare)
            {
                if (node == null)
                {
                    node = new Node(value, this);
                    return node.Rebalance();
                }
                return node.Insert(value, compare);
            }

            public Node Find(T value, CompareDelegate compare)
            {
                int cmp = compare(this.value, value);
                if (cmp==0) return this;
                if (cmp > 0) 
                {
                    if (left != null)
                        return left.Find(value, compare);
                }
                if (right != null)
                    return right.Find(value, compare);
                return null;
            }

            private Node Rebalance()
            {
                Node v = this;
                Node newRoot = this;
                bool restructured = false;
                while (v != null)
                {
                    if (!restructured && Math.Abs(ChildHeight(v.left) - ChildHeight(v.right)) > 1)
                    {
                        v = Restructure(v);
                        restructured = true;
                    }
                    v.height = 1 + v.MaxChildHeight();
                    newRoot = v;
                    v = v.parent;
                }
                return newRoot;
            }

            private static int ChildHeight(Node child)
            {
                return (child == null) ? 0 : child.height;
            }

            private int MaxChildHeight()
            {
                return Math.Max(ChildHeight(left), ChildHeight(right));
            }

            private Node ChildWithMaxHeight()
            {
                return (ChildHeight(left) > ChildHeight(right)) ? left : right;
            }

            private Node Restructure(Node z)
            {
                var y = z.ChildWithMaxHeight();
                var x = y.ChildWithMaxHeight();
                Node a, b, c;
                Node T1, T2;
                if (x == y.left && y == z.left)
                {
                    a = x; b = y; c = z;
                    T1 = a.right;
                    T2 = b.right;
                }
                else if (x == y.right && y == z.right)
                {
                    a = z; b = y; c = x;
                    T1 = b.left;
                    T2 = c.left;
                }
                else if (x == y.left && y == z.right)
                {
                    a = z; b = x; c = y;
                    T1 = b.left;
                    T2 = b.right;
                }
                else
                {
                    a = y; b = x; c = z;
                    T1 = b.left;
                    T2 = b.right;
                }
                if (z.parent != null)
                {
                    if (z == z.parent.left)
                        z.parent.left = b;
                    else z.parent.right = b;
                }
                b.parent = z.parent;

                b.left = a;
                a.parent = b;
                b.right = c;
                c.parent = b;

                a.right = T1;
                if (T1 != null) T1.parent = a;
                c.left = T2;
                if (T2 != null) T2.parent = c;
                a.height = 1 + a.MaxChildHeight();
                b.height = 1 + b.MaxChildHeight();
                c.height = 1 + c.MaxChildHeight();
                return b;
            }

            public string ToString(string representation)
            {
                return value.ToString();
            }
        }
    }
}
