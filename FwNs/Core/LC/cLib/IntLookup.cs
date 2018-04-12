namespace FwNs.Core.LC.cLib
{
    using System;

    public interface IntLookup
    {
        bool Add(int key, int value);
        int LookupFirstEqual(int key);
        int LookupFirstGreaterEqual(int key);
    }
}

