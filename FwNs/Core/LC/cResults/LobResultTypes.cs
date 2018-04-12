namespace FwNs.Core.LC.cResults
{
    using System;

    public static class LobResultTypes
    {
        public const int RequestGetBytes = 1;
        public const int RequestSetBytes = 2;
        public const int RequestGetChars = 3;
        public const int RequestSetChars = 4;
        public const int RequestGetBytePatternPosition = 5;
        public const int RequestGetCharPatternPosition = 6;
        public const int RequestCreateBytes = 7;
        public const int RequestCreateChars = 8;
        public const int RequestTruncate = 9;
        public const int RequestGetLength = 10;
        public const int RequestGetLob = 11;
        public const int ResponseGetBytes = 0x15;
        public const int ResponseSet = 0x16;
        public const int ResponseGetChars = 0x17;
        public const int ResponseGetBytePatternPosition = 0x19;
        public const int ResponseGetCharPatternPosition = 0x1a;
        public const int ResponseCreateBytes = 0x1b;
        public const int ResponseCreateChars = 0x1c;
        public const int ResponseTruncate = 0x1d;
    }
}

