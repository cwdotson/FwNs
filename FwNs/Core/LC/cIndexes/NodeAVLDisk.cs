namespace FwNs.Core.LC.cIndexes
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using System;

    public sealed class NodeAVLDisk : NodeAVL
    {
        public const int SizeInByte = 0x10;
        private readonly int _iId;
        public int iData;
        private int _iLeft;
        private int _iParent;
        private int _iRight;
        private RowAVLDisk row;

        public NodeAVLDisk(RowAVLDisk r, int id)
        {
            this._iLeft = -1;
            this._iParent = -1;
            this._iRight = -1;
            this.row = r;
            this._iId = id;
            this.iData = r.GetPos();
        }

        public NodeAVLDisk(RowAVLDisk r, IRowInputInterface input, int id)
        {
            this._iLeft = -1;
            this._iParent = -1;
            this._iRight = -1;
            this.row = r;
            this._iId = id;
            this.iData = r.GetPos();
            base.IBalance = input.ReadInt();
            this._iLeft = input.ReadInt();
            if (this._iLeft <= 0)
            {
                this._iLeft = -1;
            }
            this._iRight = input.ReadInt();
            if (this._iRight <= 0)
            {
                this._iRight = -1;
            }
            this._iParent = input.ReadInt();
            if (this._iParent <= 0)
            {
                this._iParent = -1;
            }
        }

        public override NodeAVL Child(IPersistentStore store, bool isleft)
        {
            if (!isleft)
            {
                return this.GetRight(store);
            }
            return this.GetLeft(store);
        }

        public override void Delete()
        {
            this._iLeft = -1;
            this._iRight = -1;
            this._iParent = -1;
            base.nLeft = null;
            base.nRight = null;
            base.nParent = null;
            base.IBalance = 0;
            this.row.SetNodesChanged();
        }

        public override void Destroy()
        {
        }

        public bool Equals(NodeAVL n)
        {
            if (!(n is NodeAVLDisk))
            {
                return false;
            }
            if (this != n)
            {
                return (this.GetPos() == n.GetPos());
            }
            return true;
        }

        private NodeAVLDisk FindNode(IPersistentStore store, int pos)
        {
            NodeAVLDisk node = null;
            RowAVLDisk disk2 = (RowAVLDisk) store.Get(pos, false);
            if (disk2 != null)
            {
                node = (NodeAVLDisk) disk2.GetNode(this._iId);
            }
            return node;
        }

        public override int GetAccessCount()
        {
            return 0;
        }

        public override int GetBalance(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            return node.IBalance;
        }

        public override object[] GetData(IPersistentStore store)
        {
            return this.row.RowData;
        }

        public override NodeAVL GetLeft(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            if (node._iLeft == -1)
            {
                return null;
            }
            if ((node.nLeft == null) || !node.nLeft.IsInMemory())
            {
                node.nLeft = this.FindNode(store, node._iLeft);
                node.nLeft.nParent = node;
            }
            return node.nLeft;
        }

        public override NodeAVL GetParent(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            if (node._iParent == -1)
            {
                return null;
            }
            if ((node.nParent == null) || !node.nParent.IsInMemory())
            {
                node.nParent = this.FindNode(store, this._iParent);
            }
            return node.nParent;
        }

        public override int GetPos()
        {
            return this.iData;
        }

        public override int GetRealSize(IRowOutputInterface output)
        {
            return 0x10;
        }

        public override NodeAVL GetRight(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            if (node._iRight == -1)
            {
                return null;
            }
            if ((node.nRight == null) || !node.nRight.IsInMemory())
            {
                node.nRight = this.FindNode(store, node._iRight);
                node.nRight.nParent = node;
            }
            return node.nRight;
        }

        public override Row GetRow(IPersistentStore store)
        {
            if (!this.row.IsInMemory())
            {
                return (RowAVLDisk) store.Get(this.row, false);
            }
            this.row.UpdateAccessCount(store.GetAccessCount());
            return this.row;
        }

        public override int GetStorageSize()
        {
            return 0;
        }

        public override bool HasChanged()
        {
            return false;
        }

        public override bool IsFromLeft(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            if (node._iParent == -1)
            {
                return true;
            }
            if ((node.nParent == null) || !node.nParent.IsInMemory())
            {
                node.nParent = this.FindNode(store, this._iParent);
            }
            return (this.GetPos() == ((NodeAVLDisk) node.nParent)._iLeft);
        }

        public override bool IsInMemory()
        {
            return this.row.IsInMemory();
        }

        public override bool IsKeepInMemory()
        {
            return false;
        }

        public override bool IsLeft(NodeAVL n)
        {
            if (n == null)
            {
                return (this._iLeft == -1);
            }
            return (this._iLeft == ((NodeAVLDisk) n).iData);
        }

        public override bool IsMemory()
        {
            return false;
        }

        public override bool IsRight(NodeAVL n)
        {
            if (n == null)
            {
                return (this._iRight == -1);
            }
            return (this._iRight == ((NodeAVLDisk) n).iData);
        }

        public override bool IsRoot(IPersistentStore store)
        {
            NodeAVLDisk node = this;
            if (!this.row.IsInMemory())
            {
                node = (NodeAVLDisk) ((RowAVLDisk) store.Get(this.row, false)).GetNode(this._iId);
            }
            return (node._iParent == -1);
        }

        public override bool KeepInMemory(bool keep)
        {
            return false;
        }

        public override void Replace(IPersistentStore store, Index index, NodeAVL n)
        {
            NodeAVLDisk node = this;
            RowAVLDisk row = this.row;
            if (!row.KeepInMemory(true))
            {
                row = (RowAVLDisk) store.Get(this.row, true);
                node = (NodeAVLDisk) row.GetNode(this._iId);
            }
            if (node._iParent == -1)
            {
                if (n != null)
                {
                    n = n.SetParent(store, null);
                }
                store.SetAccessor(index, n);
            }
            else
            {
                bool isLeft = node.IsFromLeft(store);
                node.GetParent(store).Set(store, isLeft, n);
            }
            row.KeepInMemory(false);
        }

        public override void Restore()
        {
        }

        public override NodeAVL Set(IPersistentStore store, bool isLeft, NodeAVL n)
        {
            if (n != null)
            {
                n.SetParent(store, this);
            }
            return (isLeft ? this.SetLeft(store, n) : this.SetRight(store, n));
        }

        public override NodeAVL SetBalance(IPersistentStore store, int b)
        {
            NodeAVLDisk node = this;
            RowAVLDisk row = this.row;
            if (!row.KeepInMemory(true))
            {
                row = (RowAVLDisk) store.Get(this.row, true);
                node = (NodeAVLDisk) row.GetNode(this._iId);
            }
            if (!row.IsInMemory())
            {
                throw Error.RuntimeError(0xc9, "NodeAVLDisk");
            }
            row.SetNodesChanged();
            node.IBalance = b;
            row.KeepInMemory(false);
            return node;
        }

        public override void SetInMemory(bool input)
        {
            if (!input)
            {
                if (base.nLeft != null)
                {
                    base.nLeft.nParent = null;
                }
                if (base.nRight != null)
                {
                    base.nRight.nParent = null;
                }
                if (base.nParent != null)
                {
                    if (this.iData == ((NodeAVLDisk) base.nParent)._iLeft)
                    {
                        base.nParent.nLeft = null;
                    }
                    else
                    {
                        base.nParent.nRight = null;
                    }
                }
                base.nLeft = base.nRight = (NodeAVL) (base.nParent = null);
            }
        }

        public override NodeAVL SetLeft(IPersistentStore store, NodeAVL n)
        {
            NodeAVLDisk node = this;
            RowAVLDisk row = this.row;
            if (!row.KeepInMemory(true))
            {
                row = (RowAVLDisk) store.Get(this.row, true);
                node = (NodeAVLDisk) row.GetNode(this._iId);
            }
            if (!row.IsInMemory())
            {
                throw Error.RuntimeError(0xc9, "NodeAVLDisk");
            }
            row.SetNodesChanged();
            node._iLeft = (n == null) ? -1 : n.GetPos();
            if ((n != null) && !n.IsInMemory())
            {
                n = this.FindNode(store, n.GetPos());
            }
            node.nLeft = n;
            row.KeepInMemory(false);
            return node;
        }

        public override NodeAVL SetParent(IPersistentStore store, NodeAVL n)
        {
            NodeAVLDisk node = this;
            RowAVLDisk row = this.row;
            if (!row.KeepInMemory(true))
            {
                row = (RowAVLDisk) store.Get(this.row, true);
                node = (NodeAVLDisk) row.GetNode(this._iId);
            }
            if (!row.IsInMemory())
            {
                row.KeepInMemory(false);
                throw Error.RuntimeError(0xc9, "NodeAVLDisk");
            }
            row.SetNodesChanged();
            node._iParent = (n == null) ? -1 : n.GetPos();
            if ((n != null) && !n.IsInMemory())
            {
                n = this.FindNode(store, n.GetPos());
            }
            node.nParent = n;
            row.KeepInMemory(false);
            return node;
        }

        public override void SetPos(int pos)
        {
        }

        public override NodeAVL SetRight(IPersistentStore store, NodeAVL n)
        {
            NodeAVLDisk node = this;
            RowAVLDisk row = this.row;
            if (!row.KeepInMemory(true))
            {
                row = (RowAVLDisk) store.Get(this.row, true);
                node = (NodeAVLDisk) row.GetNode(this._iId);
            }
            if (!row.IsInMemory())
            {
                throw Error.RuntimeError(0xc9, "NodeAVLDisk");
            }
            row.SetNodesChanged();
            node._iRight = (n == null) ? -1 : n.GetPos();
            if ((n != null) && !n.IsInMemory())
            {
                n = this.FindNode(store, n.GetPos());
            }
            node.nRight = n;
            row.KeepInMemory(false);
            return node;
        }

        public override void SetStorageSize(int size)
        {
        }

        public override void UpdateAccessCount(int count)
        {
        }

        public override void Write(IRowOutputInterface output)
        {
            output.WriteInt(base.IBalance);
            output.WriteInt((this._iLeft == -1) ? 0 : this._iLeft);
            output.WriteInt((this._iRight == -1) ? 0 : this._iRight);
            output.WriteInt((this._iParent == -1) ? 0 : this._iParent);
        }

        public override void Write(IRowOutputInterface output, IntLookup lookup)
        {
            output.WriteInt(base.IBalance);
            WriteTranslatePointer(this._iLeft, output, lookup);
            WriteTranslatePointer(this._iRight, output, lookup);
            WriteTranslatePointer(this._iParent, output, lookup);
        }

        private static void WriteTranslatePointer(int pointer, IRowOutputInterface output, IntLookup lookup)
        {
            int i = 0;
            if (pointer != -1)
            {
                i = lookup.LookupFirstEqual(pointer);
            }
            output.WriteInt(i);
        }
    }
}

