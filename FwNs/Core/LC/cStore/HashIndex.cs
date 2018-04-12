namespace FwNs.Core.LC.cStore
{
    using System;

    public class HashIndex
    {
        public int[] HashTable;
        public int[] LinkTable;
        public int NewNodePointer;
        public int ElementCount;
        public int ReclaimedNodePointer = -1;
        public bool FixedSize;
        public bool Modified;

        public HashIndex(int hashTableSize, int capacity, bool fixedSize)
        {
            if (capacity < hashTableSize)
            {
                capacity = hashTableSize;
            }
            this.Reset(hashTableSize, capacity);
            this.FixedSize = fixedSize;
        }

        public void Clear()
        {
            int length = this.LinkTable.Length;
            int[] linkTable = this.LinkTable;
            while (--length >= 0)
            {
                linkTable[length] = 0;
            }
            this.ResetTables();
        }

        public int GetHashIndex(int hash)
        {
            return ((hash & 0x7fffffff) % this.HashTable.Length);
        }

        public int GetLookup(int hash)
        {
            if (this.ElementCount == 0)
            {
                return -1;
            }
            int index = (hash & 0x7fffffff) % this.HashTable.Length;
            return this.HashTable[index];
        }

        public int GetNextLookup(int lookup)
        {
            return this.LinkTable[lookup];
        }

        public int LinkNode(int index, int lastLookup)
        {
            int reclaimedNodePointer = this.ReclaimedNodePointer;
            if (reclaimedNodePointer == -1)
            {
                int newNodePointer = this.NewNodePointer;
                this.NewNodePointer = newNodePointer + 1;
                reclaimedNodePointer = newNodePointer;
            }
            else
            {
                this.ReclaimedNodePointer = this.LinkTable[reclaimedNodePointer];
            }
            if (lastLookup == -1)
            {
                this.HashTable[index] = reclaimedNodePointer;
            }
            else
            {
                this.LinkTable[lastLookup] = reclaimedNodePointer;
            }
            this.LinkTable[reclaimedNodePointer] = -1;
            this.ElementCount++;
            this.Modified = true;
            return reclaimedNodePointer;
        }

        public bool RemoveEmptyNode(int lookup)
        {
            bool flag = false;
            int index = -1;
            for (int i = this.ReclaimedNodePointer; i >= 0; i = this.LinkTable[i])
            {
                if (i == lookup)
                {
                    if (index == -1)
                    {
                        this.ReclaimedNodePointer = this.LinkTable[lookup];
                    }
                    else
                    {
                        this.LinkTable[index] = this.LinkTable[lookup];
                    }
                    flag = true;
                    break;
                }
                index = i;
            }
            if (!flag)
            {
                return false;
            }
            for (int j = 0; j < this.NewNodePointer; j++)
            {
                if (this.LinkTable[j] > lookup)
                {
                    this.LinkTable[j]--;
                }
            }
            Array.Copy(this.LinkTable, lookup + 1, this.LinkTable, lookup, (this.NewNodePointer - lookup) - 1);
            this.LinkTable[this.NewNodePointer - 1] = 0;
            this.NewNodePointer--;
            for (int k = 0; k < this.HashTable.Length; k++)
            {
                if (this.HashTable[k] > lookup)
                {
                    this.HashTable[k]--;
                }
            }
            return true;
        }

        public void Reset(int hashTableSize, int capacity)
        {
            int[] numArray = new int[hashTableSize];
            int[] numArray2 = new int[capacity];
            this.HashTable = numArray;
            this.LinkTable = numArray2;
            this.ResetTables();
        }

        public void ResetTables()
        {
            int length = this.HashTable.Length;
            int[] hashTable = this.HashTable;
            while (--length >= 0)
            {
                hashTable[length] = -1;
            }
            this.NewNodePointer = 0;
            this.ElementCount = 0;
            this.ReclaimedNodePointer = -1;
            this.Modified = false;
        }

        public void UnlinkNode(int index, int lastLookup, int lookup)
        {
            if (lastLookup == -1)
            {
                this.HashTable[index] = this.LinkTable[lookup];
            }
            else
            {
                this.LinkTable[lastLookup] = this.LinkTable[lookup];
            }
            this.LinkTable[lookup] = this.ReclaimedNodePointer;
            this.ReclaimedNodePointer = lookup;
            this.ElementCount--;
        }
    }
}

