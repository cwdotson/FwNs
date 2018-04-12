namespace FwNs.Core.LC.cExpressions
{
    using System;

    public static class OpTypes
    {
        public const int Value = 1;
        public const int Column = 2;
        public const int Coalesce = 3;
        public const int Default = 4;
        public const int SimpleColumn = 5;
        public const int Variable = 6;
        public const int Parameter = 7;
        public const int DynamicParam = 8;
        public const int Asterisk = 9;
        public const int Sequence = 10;
        public const int ScalarSubquery = 0x15;
        public const int RowSubquery = 0x16;
        public const int TableSubquery = 0x17;
        public const int Row = 0x19;
        public const int Table = 0x1a;
        public const int Function = 0x1b;
        public const int SqlFunction = 0x1c;
        public const int RoutineFunction = 0x1d;
        public const int Negate = 0x1f;
        public const int Add = 0x20;
        public const int Subtract = 0x21;
        public const int Multiply = 0x22;
        public const int Divide = 0x23;
        public const int Concat = 0x24;
        public const int Equal = 0x29;
        public const int GreaterEqual = 0x2a;
        public const int Greater = 0x2b;
        public const int Smaller = 0x2c;
        public const int SmallerEqual = 0x2d;
        public const int NotEqual = 0x2e;
        public const int IsNull = 0x2f;
        public const int Not = 0x30;
        public const int And = 0x31;
        public const int Or = 50;
        public const int AllQuantified = 0x33;
        public const int AnyQuantified = 0x34;
        public const int Like = 0x35;
        public const int In = 0x36;
        public const int Exists = 0x37;
        public const int Overlaps = 0x38;
        public const int Unique = 0x39;
        public const int NotDistinct = 0x3a;
        public const int MatchSimple = 0x3b;
        public const int MatchPartial = 60;
        public const int MatchFull = 0x3d;
        public const int MatchUniqueSimple = 0x3e;
        public const int MatchUniquePartial = 0x3f;
        public const int MatchUniqueFull = 0x40;
        public const int Count = 0x47;
        public const int Sum = 0x48;
        public const int Min = 0x49;
        public const int Max = 0x4a;
        public const int Avg = 0x4b;
        public const int Every = 0x4c;
        public const int Some = 0x4d;
        public const int StddevPop = 0x4e;
        public const int StddevSamp = 0x4f;
        public const int VarPop = 80;
        public const int VarSamp = 0x51;
        public const int Cast = 0x5b;
        public const int ZoneModifier = 0x5c;
        public const int CaseWhen = 0x5d;
        public const int OrderBy = 0x5e;
        public const int Limit = 0x5f;
        public const int Alternative = 0x60;
        public const int MultiColumn = 0x61;
        public const int UserVariable = 0x62;
        public const int Cursor = 100;
        public const int Mod = 0x65;
        public const int Aggregate = 0x66;
        public const int TvfSubquery = 0x67;
        public const int UserAggregate = 0x68;
        public const int ArrayAccess = 0x69;
        public const int ArraySubquery = 0x6a;
        public const int Array = 0x6b;
        public const int MultiSet = 0x6c;
        public const int SequenceNextval = 0x6d;
        public const int SequenceCurrval = 110;
        public const int AndOrChain = 0x6f;
    }
}

