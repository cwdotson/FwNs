namespace FwNs.Core.LC.cPersist
{
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cStore;
    using System;
    using System.Collections.Generic;

    public sealed class Cache : BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>
    {
        private readonly long _bytesCapacity;
        private readonly int _capacity;
        private readonly CachedObjectComparator _rowComparator;
        private readonly ICachedObject[] _rowTable;
        private readonly StopWatch _saveAllTimer;
        private readonly StopWatch _sortTimer;
        private long _cacheBytesLength;
        private int _saveRowCount;
        public DataFileCache dataFileCache;

        public Cache(DataFileCache dfc) : base(dfc.Capacity(), 1, 3, true)
        {
            this._saveAllTimer = new StopWatch(false);
            this._sortTimer = new StopWatch(false);
            base.MaxCapacity = dfc.Capacity();
            this.dataFileCache = dfc;
            this._capacity = dfc.Capacity();
            this._bytesCapacity = dfc.BytesCapacity();
            this._rowComparator = new CachedObjectComparator();
            this._rowTable = new ICachedObject[this._capacity];
            this._cacheBytesLength = 0L;
        }

        private void CleanUp()
        {
            lock (this)
            {
                this.UpdateAccessCounts();
                int count = base.Size() / 2;
                int accessCountCeiling = this.GetAccessCountCeiling(count, count / 8);
                BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>.BaseHashIterator<int, ICachedObject, ICachedObject, ICachedObject> iterator = new BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>.BaseHashIterator<int, ICachedObject, ICachedObject, ICachedObject>(this);
                int num3 = 0;
                while (iterator.HasNext())
                {
                    ICachedObject obj2 = iterator.Next();
                    if (iterator.GetAccessCount() <= accessCountCeiling)
                    {
                        lock (obj2)
                        {
                            if (obj2.IsKeepInMemory())
                            {
                                iterator.SetAccessCount(accessCountCeiling + 1);
                            }
                            else
                            {
                                obj2.SetInMemory(false);
                                if (obj2.HasChanged())
                                {
                                    this._rowTable[num3++] = obj2;
                                }
                                iterator.Remove();
                                this._cacheBytesLength -= obj2.GetStorageSize();
                                count--;
                            }
                        }
                    }
                    if (num3 == this._rowTable.Length)
                    {
                        this.SaveRows(num3);
                        num3 = 0;
                    }
                }
                base.SetAccessCountFloor(accessCountCeiling);
                this.SaveRows(num3);
            }
        }

        public override void Clear()
        {
            lock (this)
            {
                base.Clear();
                this._cacheBytesLength = 0L;
            }
        }

        public void ForceCleanUp()
        {
            lock (this)
            {
                BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>.BaseHashIterator<int, ICachedObject, ICachedObject, ICachedObject> iterator = new BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>.BaseHashIterator<int, ICachedObject, ICachedObject, ICachedObject>(this);
                while (iterator.HasNext())
                {
                    ICachedObject obj2 = iterator.Next();
                    lock (obj2)
                    {
                        if (!obj2.IsKeepInMemory())
                        {
                            obj2.SetInMemory(false);
                            iterator.Remove();
                            this._cacheBytesLength -= obj2.GetStorageSize();
                        }
                        continue;
                    }
                }
            }
        }

        public ICachedObject Get(int pos)
        {
            lock (this)
            {
                if (base.AccessCount == 0x7fefffff)
                {
                    this.UpdateAccessCounts();
                    base.ResetAccessCount();
                    this.UpdateObjectAccessCounts();
                }
                int lookup = base.GetLookup(pos, pos);
                if (lookup == -1)
                {
                    return null;
                }
                int num2 = base.AccessCount + 1;
                base.AccessCount = num2;
                base.AccessTable[lookup] = num2;
                return base.ObjectValueTable[lookup];
            }
        }

        public long GetTotalCachedBlockSize()
        {
            return this._cacheBytesLength;
        }

        public static void Init(int capacity, long bytesCapacity)
        {
        }

        public void Put(int key, ICachedObject row)
        {
            lock (this)
            {
                int storageSize = row.GetStorageSize();
                if ((base.Size() >= this._capacity) || ((storageSize + this._cacheBytesLength) > this._bytesCapacity))
                {
                    this.CleanUp();
                }
                if (base.AccessCount > 0x7fefffff)
                {
                    this.UpdateAccessCounts();
                    base.ResetAccessCount();
                    this.UpdateObjectAccessCounts();
                }
                base.AddOrRemove(key, key, row, null, false);
                row.SetInMemory(true);
                this._cacheBytesLength += storageSize;
            }
        }

        public ICachedObject Release(int i)
        {
            lock (this)
            {
                ICachedObject obj3 = base.AddOrRemove(i, i, null, null, true);
                if (obj3 == null)
                {
                    return null;
                }
                this._cacheBytesLength -= obj3.GetStorageSize();
                obj3.SetInMemory(false);
                return obj3;
            }
        }

        public void SaveAll()
        {
            lock (this)
            {
                Iterator<ICachedObject> iterator = new BaseHashMap<int, ICachedObject, ICachedObject, ICachedObject>.BaseHashIterator<int, ICachedObject, ICachedObject, ICachedObject>(this);
                int count = 0;
                while (iterator.HasNext())
                {
                    if (count == this._rowTable.Length)
                    {
                        this.SaveRows(count);
                        count = 0;
                    }
                    ICachedObject obj2 = iterator.Next();
                    if (obj2.HasChanged())
                    {
                        this._rowTable[count++] = obj2;
                    }
                }
                this.SaveRows(count);
            }
        }

        private void SaveRows(int count)
        {
            lock (this)
            {
                if (count != 0)
                {
                    this._rowComparator.SetType(1);
                    this._sortTimer.Start();
                    ArraySort.Sort<ICachedObject>(this._rowTable, 0, count, this._rowComparator);
                    this._sortTimer.Stop();
                    this._saveAllTimer.Start();
                    this.dataFileCache.SaveRows(this._rowTable, 0, count);
                    this._saveRowCount += count;
                    this._saveAllTimer.Stop();
                }
            }
        }

        private void UpdateAccessCounts()
        {
            for (int i = 0; i < base.ObjectValueTable.Length; i++)
            {
                ICachedObject obj2 = base.ObjectValueTable[i];
                if (obj2 != null)
                {
                    int accessCount = obj2.GetAccessCount();
                    if (accessCount > base.AccessTable[i])
                    {
                        base.AccessTable[i] = accessCount;
                    }
                }
            }
        }

        private void UpdateObjectAccessCounts()
        {
            for (int i = 0; i < base.ObjectValueTable.Length; i++)
            {
                ICachedObject obj2 = base.ObjectValueTable[i];
                if (obj2 != null)
                {
                    int count = base.AccessTable[i];
                    obj2.UpdateAccessCount(count);
                }
            }
        }

        private class CachedObjectComparator : IComparer<ICachedObject>
        {
            public const int ComparePosition = 1;
            public const int CompareSize = 2;
            private int _compareType;

            public int Compare(ICachedObject a, ICachedObject b)
            {
                switch (this._compareType)
                {
                    case 1:
                        return (a.GetPos() - b.GetPos());

                    case 2:
                        return (a.GetStorageSize() - b.GetStorageSize());
                }
                return 0;
            }

            public void SetType(int type)
            {
                this._compareType = type;
            }
        }
    }
}

