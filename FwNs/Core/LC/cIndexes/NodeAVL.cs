namespace FwNs.Core.LC.cIndexes
{
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cPersist;
    using FwNs.Core.LC.cRowIO;
    using FwNs.Core.LC.cRows;
    using System;

    public class NodeAVL : ICachedObject
    {
        public const int NoPos = -1;
        public int IBalance;
        public NodeAVL nLeft;
        public NodeAVL nNext;
        public NodeAVL nParent;
        public NodeAVL nRight;
        public Row MemoryRow;

        public NodeAVL()
        {
            this.MemoryRow = null;
        }

        public NodeAVL(Row r)
        {
            this.MemoryRow = r;
        }

        public virtual NodeAVL Child(IPersistentStore store, bool isleft)
        {
            if (!isleft)
            {
                return this.GetRight(store);
            }
            return this.GetLeft(store);
        }

        public virtual void Delete()
        {
            this.IBalance = 0;
            this.nLeft = this.nRight = (NodeAVL) (this.nParent = null);
        }

        public virtual void Destroy()
        {
        }

        public override bool Equals(object n)
        {
            return (n == this);
        }

        public virtual int GetAccessCount()
        {
            return 0;
        }

        public virtual int GetBalance(IPersistentStore store)
        {
            return this.IBalance;
        }

        public virtual object[] GetData(IPersistentStore store)
        {
            return this.MemoryRow.RowData;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual NodeAVL GetLeft(IPersistentStore store)
        {
            return this.nLeft;
        }

        public virtual NodeAVL GetParent(IPersistentStore store)
        {
            return this.nParent;
        }

        public virtual int GetPos()
        {
            return 0;
        }

        public virtual int GetRealSize(IRowOutputInterface o)
        {
            return 0;
        }

        public virtual NodeAVL GetRight(IPersistentStore persistentStore)
        {
            return this.nRight;
        }

        public virtual Row GetRow(IPersistentStore store)
        {
            return this.MemoryRow;
        }

        public virtual int GetStorageSize()
        {
            return 0;
        }

        public virtual bool HasChanged()
        {
            return false;
        }

        public virtual bool IsFromLeft(IPersistentStore store)
        {
            if (this.nParent != null)
            {
                return (this == this.nParent.nLeft);
            }
            return true;
        }

        public virtual bool IsInMemory()
        {
            return false;
        }

        public virtual bool IsKeepInMemory()
        {
            return false;
        }

        public virtual bool IsLeft(NodeAVL node)
        {
            return (this.nLeft == node);
        }

        public virtual bool IsMemory()
        {
            return true;
        }

        public virtual bool IsRight(NodeAVL node)
        {
            return (this.nRight == node);
        }

        public virtual bool IsRoot(IPersistentStore store)
        {
            return (this.nParent == null);
        }

        public virtual bool KeepInMemory(bool keep)
        {
            return true;
        }

        public virtual void Replace(IPersistentStore store, Index index, NodeAVL n)
        {
            if (this.nParent == null)
            {
                if (n != null)
                {
                    n = n.SetParent(store, null);
                }
                store.SetAccessor(index, n);
            }
            else
            {
                this.nParent.Set(store, this.IsFromLeft(store), n);
            }
        }

        public virtual void Restore()
        {
        }

        public virtual NodeAVL Set(IPersistentStore store, bool isLeft, NodeAVL n)
        {
            if (isLeft)
            {
                this.nLeft = n;
            }
            else
            {
                this.nRight = n;
            }
            if (n != null)
            {
                n.nParent = this;
            }
            return this;
        }

        public virtual NodeAVL SetBalance(IPersistentStore store, int b)
        {
            this.IBalance = b;
            return this;
        }

        public virtual void SetInMemory(bool i)
        {
        }

        public virtual NodeAVL SetLeft(IPersistentStore persistentStore, NodeAVL n)
        {
            this.nLeft = n;
            return this;
        }

        public virtual NodeAVL SetParent(IPersistentStore persistentStore, NodeAVL n)
        {
            this.nParent = n;
            return this;
        }

        public virtual void SetPos(int pos)
        {
        }

        public virtual NodeAVL SetRight(IPersistentStore persistentStore, NodeAVL n)
        {
            this.nRight = n;
            return this;
        }

        public virtual void SetStorageSize(int size)
        {
        }

        public virtual void UpdateAccessCount(int count)
        {
        }

        public virtual void Write(IRowOutputInterface o)
        {
        }

        public virtual void Write(IRowOutputInterface o, IntLookup lookup)
        {
        }
    }
}

