namespace FwNs.Core.LC.cRows
{
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cTables;
    using System;

    public class RowAVL : Row
    {
        public NodeAVL NPrimaryNode;

        protected RowAVL(TableBase table, object[] data) : base(table, data)
        {
        }

        public RowAVL(TableBase table, object[] data, int position) : base(table, data)
        {
            base.Position = position;
        }

        public void ClearNonPrimaryNodes()
        {
            for (NodeAVL eavl = this.NPrimaryNode.nNext; eavl != null; eavl = eavl.nNext)
            {
                eavl.Delete();
                eavl.IBalance = 0;
            }
        }

        public override void Delete(IPersistentStore store)
        {
            for (NodeAVL eavl = this.NPrimaryNode; eavl != null; eavl = eavl.nNext)
            {
                eavl.Delete();
            }
        }

        public override void Destroy()
        {
            this.ClearNonPrimaryNodes();
            NodeAVL nNext = this.NPrimaryNode.nNext;
            while (nNext != null)
            {
                NodeAVL eavl2 = nNext;
                nNext = nNext.nNext;
                eavl2.nNext = null;
            }
            this.NPrimaryNode = null;
        }

        public virtual NodeAVL GetNextNode(NodeAVL n)
        {
            n = (n == null) ? this.NPrimaryNode : n.nNext;
            return n;
        }

        public virtual NodeAVL GetNode(int index)
        {
            NodeAVL nPrimaryNode = this.NPrimaryNode;
            while (index-- > 0)
            {
                nPrimaryNode = nPrimaryNode.nNext;
            }
            return nPrimaryNode;
        }

        public virtual NodeAVL InsertNode(int index)
        {
            NodeAVL node = this.GetNode(index - 1);
            NodeAVL eavl2 = new NodeAVL(this) {
                nNext = node.nNext
            };
            node.nNext = eavl2;
            return eavl2;
        }

        public override void Restore()
        {
        }

        public virtual void SetNewNodes()
        {
            int indexCount = base.table.GetIndexCount();
            this.NPrimaryNode = new NodeAVL(this);
            NodeAVL nPrimaryNode = this.NPrimaryNode;
            for (int i = 1; i < indexCount; i++)
            {
                nPrimaryNode.nNext = new NodeAVL(this);
                nPrimaryNode = nPrimaryNode.nNext;
            }
        }
    }
}

