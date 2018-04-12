namespace FwNs.Core.LC.cRows
{
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cIndexes;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cTables;
    using System;
    using System.IO;

    public sealed class RowAVLDisk : RowAVL
    {
        public const int NO_POS = -1;
        private int storageSize;
        private int _keepCount;
        private bool _isInMemory;
        private int _accessCount;
        private bool hasDataChanged;
        private bool _hasNodesChanged;

        public RowAVLDisk(TableBase t, object[] o) : base(t, o)
        {
            this.SetNewNodes();
            this.hasDataChanged = this._hasNodesChanged = true;
        }

        public RowAVLDisk(TableBase t, IRowInputInterface _in) : base(t, null)
        {
            base.Position = _in.GetPos();
            this.storageSize = _in.GetSize();
            int indexCount = t.GetIndexCount();
            base.NPrimaryNode = new NodeAVLDisk(this, _in, 0);
            NodeAVL nPrimaryNode = base.NPrimaryNode;
            for (int i = 1; i < indexCount; i++)
            {
                nPrimaryNode.nNext = new NodeAVLDisk(this, _in, i);
                nPrimaryNode = nPrimaryNode.nNext;
            }
            base.RowData = _in.ReadData(base.table.GetColumnTypes());
        }

        public override void Delete(IPersistentStore store)
        {
            RowAVLDisk disk = this;
            if (!disk.KeepInMemory(true))
            {
                disk = (RowAVLDisk) store.Get(disk, true);
            }
            base.Delete(store);
            disk.KeepInMemory(false);
        }

        public override void Destroy()
        {
            base.NPrimaryNode = null;
            base.table = null;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            RowAVLDisk disk = obj as RowAVLDisk;
            return ((disk != null) && (disk.Position == base.Position));
        }

        public override int GetAccessCount()
        {
            return this._accessCount;
        }

        public override int GetHashCode()
        {
            return base.Position;
        }

        public override int GetRealSize(IRowOutputInterface _out)
        {
            return (_out.GetSize(this) + (base.table.GetIndexCount() * 0x10));
        }

        public override int GetStorageSize()
        {
            return this.storageSize;
        }

        public override bool HasChanged()
        {
            lock (this)
            {
                return (this._hasNodesChanged || this.hasDataChanged);
            }
        }

        public override NodeAVL InsertNode(int index)
        {
            return null;
        }

        public override bool IsInMemory()
        {
            lock (this)
            {
                return this._isInMemory;
            }
        }

        public override bool IsKeepInMemory()
        {
            lock (this)
            {
                return (this._keepCount > 0);
            }
        }

        public override bool IsMemory()
        {
            return false;
        }

        public override bool KeepInMemory(bool keep)
        {
            lock (this)
            {
                if (!this._isInMemory)
                {
                    return false;
                }
                if (keep)
                {
                    this._keepCount++;
                }
                else
                {
                    this._keepCount--;
                    if (this._keepCount < 0)
                    {
                        throw Error.RuntimeError(0xc9, "RowAVLDisk - keep count");
                    }
                }
                return true;
            }
        }

        public override void SetChanged()
        {
            lock (this)
            {
                this.hasDataChanged = true;
            }
        }

        public override void SetInMemory(bool _in)
        {
            lock (this)
            {
                this._isInMemory = _in;
                if (!_in)
                {
                    for (NodeAVL eavl = base.NPrimaryNode; eavl != null; eavl = eavl.nNext)
                    {
                        eavl.SetInMemory(_in);
                    }
                }
            }
        }

        public override void SetNewNodes()
        {
            int indexCount = base.table.GetIndexCount();
            base.NPrimaryNode = new NodeAVLDisk(this, 0);
            NodeAVL nPrimaryNode = base.NPrimaryNode;
            for (int i = 1; i < indexCount; i++)
            {
                nPrimaryNode.nNext = new NodeAVLDisk(this, i);
                nPrimaryNode = nPrimaryNode.nNext;
            }
        }

        public void SetNodesChanged()
        {
            lock (this)
            {
                this._hasNodesChanged = true;
            }
        }

        public override void SetPos(int pos)
        {
            base.Position = pos;
            for (NodeAVL eavl = base.NPrimaryNode; eavl != null; eavl = eavl.nNext)
            {
                ((NodeAVLDisk) eavl).iData = base.Position;
            }
        }

        public override void SetStorageSize(int size)
        {
            this.storageSize = size;
        }

        public override void UpdateAccessCount(int count)
        {
            this._accessCount = count;
        }

        public override void Write(IRowOutputInterface output)
        {
            try
            {
                this.WriteNodes(output);
                if (this.hasDataChanged)
                {
                    output.WriteData(base.RowData, base.table.ColTypes);
                    output.WriteEnd();
                    this.hasDataChanged = false;
                }
            }
            catch (IOException)
            {
            }
        }

        public override void Write(IRowOutputInterface output, IntLookup lookup)
        {
            output.WriteSize(this.storageSize);
            for (NodeAVL eavl = base.NPrimaryNode; eavl != null; eavl = eavl.nNext)
            {
                ((NodeAVLDisk) eavl).Write(output, lookup);
            }
            output.WriteData(base.RowData, base.table.ColTypes);
            output.WriteEnd();
        }

        private void WriteNodes(IRowOutputInterface _out)
        {
            _out.WriteSize(this.storageSize);
            for (NodeAVL eavl = base.NPrimaryNode; eavl != null; eavl = eavl.nNext)
            {
                eavl.Write(_out);
            }
            this._hasNodesChanged = false;
        }
    }
}

