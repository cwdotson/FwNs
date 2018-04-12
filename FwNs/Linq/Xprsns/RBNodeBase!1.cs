namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{Key}, {Color}")]
    public class RBNodeBase<TKey> : RBTreeUtils<TKey>.ITree
    {
        public TKey Key;
        internal RBNodeBase<TKey> Left;
        internal RBNodeBase<TKey> Right;
        internal NodeColor Color;

        public RBNodeBase(TKey key)
        {
            this.Key = key;
        }

        public virtual void CopyFrom(RBNodeBase<TKey> otherNode)
        {
            this.Key = otherNode.Key;
        }

        public RBNodeBase<TKey> DoubleRotation(bool direction)
        {
            this[!direction] = this[!direction].SingleRotation(!direction);
            return this.SingleRotation(direction);
        }

        internal void Dump(IndentedTextWriter writer, bool? right)
        {
            if (right.HasValue)
            {
                if (right.Value)
                {
                    writer.Write(">");
                }
                else
                {
                    writer.Write("<");
                }
            }
            writer.WriteLine(this.Color.ToString()[0].ToString() + ":" + this.Key);
            writer.Indent++;
            try
            {
                if (this.Left != null)
                {
                    this.Left.Dump(writer, false);
                }
                if (this.Right != null)
                {
                    this.Right.Dump(writer, true);
                }
            }
            finally
            {
                writer.Indent--;
            }
        }

        internal RBNodeBase<TKey> Find(TKey key, IComparer<TKey> keyComparer)
        {
            int num;
            for (RBNodeBase<TKey> base2 = (RBNodeBase<TKey>) this; base2 != null; base2 = base2[num])
            {
                num = keyComparer.Compare(key, base2.Key);
                if (num == 0)
                {
                    return base2;
                }
            }
            return null;
        }

        public IEnumerable<RBNodeBase<TKey>> Nodes()
        {
            return RBTreeUtils<TKey>.Nodes(this);
        }

        public IEnumerable<RBNodeBase<TKey>> NodesAscending()
        {
            return RBTreeUtils<TKey>.NodesAscending(this);
        }

        public IEnumerable<RBNodeBase<TKey>> NodesDescending()
        {
            return RBTreeUtils<TKey>.NodesDescending(this);
        }

        public RBNodeBase<TKey> SingleRotation(bool direction)
        {
            RBNodeBase<TKey> base2 = this[!direction];
            this[!direction] = base2[direction];
            base2[direction] = (RBNodeBase<TKey>) this;
            this.Color = NodeColor.Red;
            base2.Color = NodeColor.Black;
            return base2;
        }

        internal RBNodeBase<TKey> this[int direction]
        {
            get
            {
                if (direction == 0)
                {
                    throw new ArgumentException(EMDataStructures.RBTree_InvalidNodeDirection, "direction");
                }
                if (direction < 0)
                {
                    return this.Left;
                }
                return this.Right;
            }
            set
            {
                if (direction == 0)
                {
                    throw new ArgumentException(EMDataStructures.RBTree_InvalidNodeDirection, "direction");
                }
                if (direction < 0)
                {
                    this.Left = value;
                }
                else
                {
                    this.Right = value;
                }
            }
        }

        internal RBNodeBase<TKey> this[bool direction]
        {
            get
            {
                if (direction)
                {
                    return this.Right;
                }
                return this.Left;
            }
            set
            {
                if (direction)
                {
                    this.Right = value;
                }
                else
                {
                    this.Left = value;
                }
            }
        }

        RBNodeBase<TKey> RBTreeUtils<TKey>.ITree.Root
        {
            get
            {
                return (RBNodeBase<TKey>) this;
            }
        }
    }
}

