﻿namespace FwNs.Core.LC.cErrors
{
    using System;

    public static class ErrorCode
    {
        public const string SqlStateNoError = "00000";
        public const int TOKEN_REQUIRED = 1;
        public const int CONSTRAINT = 2;
        public const int M_ERROR_IN_BINARY_SCRIPT_1 = 0x15;
        public const int M_ERROR_IN_BINARY_SCRIPT_2 = 0x16;
        public const int M_DatabaseManager_getDatabase = 0x17;
        public const int M_parse_line = 0x18;
        public const int M_DatabaseScriptReader_read = 0x19;
        public const int M_Message_Pair = 0x1a;
        public const int M_LOAD_SAVE_PROPERTIES = 0x1b;
        public const int M_HsqlProperties_load = 0x1c;
        public const int M_TEXT_SOURCE_FIELD_ERROR = 0x29;
        public const int M_TextCache_openning_file_error = 0x2a;
        public const int M_TextCache_closing_file_error = 0x2b;
        public const int M_TextCache_purging_file_error = 0x2c;
        public const int M_DataFileCache_makeRow = 0x33;
        public const int M_DataFileCache_open = 0x34;
        public const int M_DataFileCache_close = 0x35;
        public const int M_SERVER_OPEN_SERVER_SOCKET_1 = 0x3d;
        public const int M_SERVER_OPEN_SERVER_SOCKET_2 = 0x3e;
        public const int M_SERVER_SECURE_VERIFY_1 = 0x3f;
        public const int M_SERVER_SECURE_VERIFY_2 = 0x40;
        public const int M_SERVER_SECURE_VERIFY_3 = 0x41;
        public const int M_RS_EMPTY = 70;
        public const int M_RS_BEFORE_FIRST = 0x47;
        public const int M_RS_AFTER_LAST = 0x48;
        public const int M_INVALID_LIMIT = 0x51;
        public const int S_00000 = 0;
        public const int U_S0500 = 0xc9;
        public const int X_S0501 = 0x12d;
        public const int X_S0502 = 0x12e;
        public const int X_S0503 = 0x12f;
        public const int X_S0504 = 0x130;
        public const int X_S0521 = 320;
        public const int X_S0522 = 0x141;
        public const int X_S0531 = 0x14b;
        public const int SERVER_TRANSFER_CORRUPTED = 0x191;
        public const int SERVER_DATABASE_DISCONNECTED = 0x192;
        public const int SERVER_VERSIONS_INCOMPATIBLE = 0x193;
        public const int SERVER_UNKNOWN_CLIENT = 0x194;
        public const int SERVER_HTTP_NOT_HSQL_PROTOCOL = 0x195;
        public const int SERVER_INCOMPLETE_HANDSHAKE_READ = 0x196;
        public const int SERVER_NO_DATABASE = 0x197;
        public const int ADO_COLUMN_NOT_FOUND = 0x1a5;
        public const int ADO_INPUTSTREAM_ERROR = 0x1a6;
        public const int ADO_INVALID_ARGUMENT = 0x1a7;
        public const int ADO_PARAMETER_NOT_SET = 0x1a8;
        public const int ADO_CONNECTION_NATIVE_SQL = 0x1a9;
        public const int LOCK_FILE_ACQUISITION_FAILURE = 0x1c3;
        public const int FILE_IO_ERROR = 0x1c4;
        public const int WRONG_DATABASE_FILE_VERSION = 0x1c5;
        public const int SHUTDOWN_REQUIRED = 0x1c6;
        public const int DATABASE_IS_READONLY = 0x1c7;
        public const int DATA_IS_READONLY = 0x1c8;
        public const int ACCESS_IS_DENIED = 0x1c9;
        public const int GENERAL_ERROR = 0x1ca;
        public const int DATABASE_IS_MEMORY_ONLY = 0x1cb;
        public const int OUT_OF_MEMORY = 460;
        public const int ERROR_IN_SCRIPT_FILE = 0x1cd;
        public const int UNSUPPORTED_FILENAME_SUFFIX = 0x1ce;
        public const int COMPRESSION_SUFFIX_MISMATCH = 0x1cf;
        public const int DATABASE_IS_NON_FILE = 0x1d0;
        public const int DATABASE_NOT_EXISTS = 0x1d1;
        public const int DATA_FILE_ERROR = 0x1d2;
        public const int GENERAL_IO_ERROR = 0x1d3;
        public const int DATA_FILE_IS_FULL = 0x1d4;
        public const int DATA_FILE_IN_USE = 0x1d5;
        public const int TEXT_TABLE_UNKNOWN_DATA_SOURCE = 0x1e1;
        public const int TEXT_TABLE_SOURCE = 0x1e2;
        public const int TEXT_FILE = 0x1e3;
        public const int TEXT_FILE_IO = 0x1e4;
        public const int TEXT_STRING_HAS_NEWLINE = 0x1e5;
        public const int TEXT_TABLE_HEADER = 0x1e6;
        public const int TEXT_SOURCE_EXISTS = 0x1e7;
        public const int TEXT_SOURCE_NO_END_SEPARATOR = 0x1e8;
        public const int W_01000 = 0x3e8;
        public const int W_01001 = 0x3e9;
        public const int W_01002 = 0x3ea;
        public const int W_01003 = 0x3eb;
        public const int W_01004 = 0x3ec;
        public const int W_01005 = 0x3ed;
        public const int W_01006 = 0x3ee;
        public const int W_01007 = 0x3ef;
        public const int W_01009 = 0x3f1;
        public const int W_0100A = 0x3f2;
        public const int W_0100B = 0x3f3;
        public const int W_0100C = 0x3f4;
        public const int W_0100D = 0x3f5;
        public const int W_0100E = 0x3f6;
        public const int W_0100F = 0x3f7;
        public const int W_01011 = 0x3f8;
        public const int W_0102F = 0x3f9;
        public const int N_02000 = 0x44c;
        public const int N_02001 = 0x44d;
        public const int X_07000 = 0x4b0;
        public const int X_07001 = 0x4b1;
        public const int X_07002 = 0x4b2;
        public const int X_07003 = 0x4b3;
        public const int X_07004 = 0x4b4;
        public const int X_07005 = 0x4b5;
        public const int X_07006 = 0x4b6;
        public const int X_07007 = 0x4b7;
        public const int X_07008 = 0x4b8;
        public const int X_07009 = 0x4b9;
        public const int X_0700B = 0x4bb;
        public const int X_0700C = 0x4bc;
        public const int X_0700D = 0x4bd;
        public const int X_0700E = 0x4be;
        public const int X_0700F = 0x4bf;
        public const int X_07501 = 0x4e3;
        public const int X_07502 = 0x4e4;
        public const int X_07503 = 0x4e5;
        public const int X_07504 = 0x4e6;
        public const int X_07505 = 0x4e7;
        public const int X_07506 = 0x4e8;
        public const int X_08000 = 0x514;
        public const int X_08001 = 0x515;
        public const int X_08002 = 0x516;
        public const int X_08003 = 0x517;
        public const int X_08004 = 0x518;
        public const int X_08006 = 0x519;
        public const int X_08007 = 0x51a;
        public const int X_08501 = 0x547;
        public const int X_08502 = 0x548;
        public const int X_08503 = 0x549;
        public const int X_09000 = 0x578;
        public const int X_0A000 = 0x5dc;
        public const int X_0A001 = 0x5dd;
        public const int X_0A501 = 0x60f;
        public const int X_0D000 = 0x640;
        public const int X_0E000 = 0x6a4;
        public const int X_0F000 = 0x708;
        public const int X_0F001 = 0x709;
        public const int X_0F502 = 0xd92;
        public const int X_0F503 = 0xd93;
        public const int X_0K000 = 0x76c;
        public const int X_0L000 = 0x7d0;
        public const int X_0L501 = 0x803;
        public const int X_0M000 = 0x834;
        public const int X_0P000 = 0x898;
        public const int X_0P501 = 0x8cb;
        public const int X_0P502 = 0x8cc;
        public const int X_0P503 = 0x8cd;
        public const int X_0S000 = 0x8fc;
        public const int X_0T000 = 0x960;
        public const int X_0U000 = 0x9c4;
        public const int X_0V000 = 0xa28;
        public const int X_0W000 = 0xa8c;
        public const int X_0X000 = 0xaf0;
        public const int X_0Y000 = 0xb54;
        public const int X_0Y001 = 0xb55;
        public const int X_0Y002 = 0xb56;
        public const int X_0Z000 = 0xbb8;
        public const int X_0Z001 = 0xbb9;
        public const int X_0Z002 = 0xbbb;
        public const int X_20000 = 0xc1c;
        public const int X_21000 = 0xc81;
        public const int X_22000 = 0xd48;
        public const int X_22001 = 0xd49;
        public const int X_22002 = 0xd4a;
        public const int X_22003 = 0xd4b;
        public const int X_22004 = 0xd4c;
        public const int X_22005 = 0xd4d;
        public const int X_22006 = 0xd4e;
        public const int X_22007 = 0xd4f;
        public const int X_22008 = 0xd50;
        public const int X_22009 = 0xd51;
        public const int X_2200B = 0xd52;
        public const int X_2200C = 0xd53;
        public const int X_2200D = 0xd54;
        public const int X_2200E = 0xd55;
        public const int X_2200F = 0xd56;
        public const int X_2200G = 0xd57;
        public const int X_2200H = 0xd58;
        public const int X_2200J = 0xd59;
        public const int X_2200K = 0xd5a;
        public const int X_2200L = 0xd5b;
        public const int X_2200M = 0xd5c;
        public const int X_2200N = 0xd5d;
        public const int X_2200P = 0xd5e;
        public const int X_2200Q = 0xd5f;
        public const int X_2200R = 0xd60;
        public const int X_2200S = 0xd61;
        public const int X_2200T = 0xd62;
        public const int X_2200U = 0xd63;
        public const int X_2200V = 0xd64;
        public const int X_2200W = 0xd65;
        public const int X_22010 = 0xd66;
        public const int X_22011 = 0xd67;
        public const int X_22012 = 0xd68;
        public const int X_22013 = 0xd69;
        public const int X_22014 = 0xd6a;
        public const int X_22015 = 0xd6b;
        public const int X_22016 = 0xd6c;
        public const int X_22017 = 0xd6d;
        public const int X_22018 = 0xd6e;
        public const int X_22019 = 0xd6f;
        public const int X_2201A = 0xd70;
        public const int X_2201B = 0xd71;
        public const int X_2201C = 0xd72;
        public const int X_2201D = 0xd73;
        public const int X_2201E = 0xd74;
        public const int X_2201F = 0xd75;
        public const int X_2201G = 0xd76;
        public const int X_2201J = 0xd77;
        public const int X_2201S = 0xd78;
        public const int X_2201T = 0xd79;
        public const int X_2201U = 0xd7a;
        public const int X_2201V = 0xd7b;
        public const int X_2201W = 0xd7c;
        public const int X_2201X = 0xd7d;
        public const int X_22021 = 0xd7e;
        public const int X_22022 = 0xd7f;
        public const int X_22023 = 0xd80;
        public const int X_22024 = 0xd81;
        public const int X_22025 = 0xd82;
        public const int X_22026 = 0xd83;
        public const int X_22027 = 0xd84;
        public const int X_22029 = 0xd85;
        public const int X_22501 = 0xd8f;
        public const int X_22511 = 0xd90;
        public const int X_22521 = 0xd91;
        public const int X_2202A = 0xda0;
        public const int X_2202D = 0xda1;
        public const int X_2202E = 0xda2;
        public const int X_2202F = 0xda3;
        public const int X_2202G = 0xda4;
        public const int X_2202H = 0xda5;
        public const int X_23000 = 0xdac;
        public const int X_23001 = 0xdad;
        public const int X_23502 = 10;
        public const int X_23503 = 0xb1;
        public const int X_23504 = 8;
        public const int X_23505 = 0x68;
        public const int X_23513 = 0x9d;
        public const int X_24000 = 0xe10;
        public const int X_24501 = 0xe11;
        public const int X_24502 = 0xe12;
        public const int X_24504 = 0xe13;
        public const int X_24513 = 0xe14;
        public const int X_24514 = 0xe15;
        public const int X_24515 = 0xe16;
        public const int X_24521 = 0xe25;
        public const int X_25000 = 0xe74;
        public const int X_25001 = 0xe75;
        public const int X_25002 = 0xe76;
        public const int X_25003 = 0xe77;
        public const int X_25004 = 0xe78;
        public const int X_25005 = 0xe79;
        public const int X_25006 = 0xe7a;
        public const int X_25007 = 0xe7b;
        public const int X_25008 = 0xe7c;
        public const int X_26000 = 0xed8;
        public const int X_27000 = 0xf3c;
        public const int X_28000 = 0xfa0;
        public const int X_28501 = 0xfa1;
        public const int X_28502 = 0xfa2;
        public const int X_28503 = 0xfa3;
        public const int X_2A000 = 0x1004;
        public const int X_2B000 = 0x1068;
        public const int X_2C000 = 0x10cc;
        public const int X_2D000 = 0x1130;
        public const int X_2D522 = 0x1131;
        public const int X_2E000 = 0x1194;
        public const int X_2F000 = 0x11f8;
        public const int X_2F002 = 0x11fa;
        public const int X_2F003 = 0x11fb;
        public const int X_2F004 = 0x11fc;
        public const int X_2F005 = 0x11fd;
        public const int X_2H000 = 0x122a;
        public const int X_30000 = 0x1234;
        public const int X_33000 = 0x123e;
        public const int X_34000 = 0x1248;
        public const int X_35000 = 0x1252;
        public const int X_36000 = 0x125c;
        public const int X_36001 = 0x125d;
        public const int X_36002 = 0x125e;
        public const int W_36501 = 0x1267;
        public const int W_36502 = 0x1268;
        public const int W_36503 = 0x1269;
        public const int X_37000 = 0x12b6;
        public const int X_38000 = 0x12c0;
        public const int X_38001 = 0x12c1;
        public const int X_38002 = 0x12c2;
        public const int X_38003 = 0x12c3;
        public const int X_38004 = 0x12c4;
        public const int X_39000 = 0x12ca;
        public const int X_39004 = 0x12cb;
        public const int X_3B000 = 0x12d4;
        public const int X_3B001 = 0x12d5;
        public const int X_3B002 = 0x12d6;
        public const int X_3C000 = 0x12de;
        public const int X_3D000 = 0x12e8;
        public const int X_3F000 = 0x12f2;
        public const int X_40000 = 0x12fc;
        public const int X_40001 = 0x12fd;
        public const int X_40002 = 0x12fe;
        public const int X_40003 = 0x12ff;
        public const int X_40004 = 0x1300;
        public const int X_40501 = 0x1307;
        public const int X_42000 = 0x1388;
        public const int X_42501 = 0x157d;
        public const int X_42502 = 0x157e;
        public const int X_42503 = 0x157f;
        public const int X_42504 = 0x1580;
        public const int X_42505 = 0x1581;
        public const int X_42506 = 0x1582;
        public const int X_42507 = 0x1583;
        public const int X_42508 = 0x1584;
        public const int X_42509 = 0x1585;
        public const int X_42510 = 0x1586;
        public const int X_42512 = 0x1588;
        public const int X_42513 = 0x1589;
        public const int X_42520 = 0x1590;
        public const int X_42521 = 0x1591;
        public const int X_42522 = 0x1592;
        public const int X_42523 = 0x1593;
        public const int X_42524 = 0x1594;
        public const int X_42525 = 0x1595;
        public const int X_42526 = 0x1596;
        public const int X_42527 = 0x1597;
        public const int X_42528 = 0x1598;
        public const int X_42529 = 0x1599;
        public const int X_42530 = 0x159a;
        public const int X_42531 = 0x159b;
        public const int X_42532 = 0x159c;
        public const int X_42533 = 0x159d;
        public const int X_42534 = 0x159e;
        public const int X_42535 = 0x159f;
        public const int X_42536 = 0x15a0;
        public const int X_42537 = 0x15a1;
        public const int X_42538 = 0x15a2;
        public const int X_42539 = 0x15a3;
        public const int X_42541 = 0x15a5;
        public const int X_42542 = 0x15a6;
        public const int X_42543 = 0x15a7;
        public const int X_42544 = 0x15a8;
        public const int X_42545 = 0x15a9;
        public const int X_42546 = 0x15aa;
        public const int X_42547 = 0x15ab;
        public const int X_42548 = 0x15ac;
        public const int X_42549 = 0x15ad;
        public const int X_42551 = 0x15af;
        public const int X_42555 = 0x15b3;
        public const int X_42556 = 0x15b4;
        public const int X_42561 = 0x15b9;
        public const int X_42562 = 0x15ba;
        public const int X_42563 = 0x15bb;
        public const int X_42564 = 0x15bc;
        public const int X_42565 = 0x15bd;
        public const int X_42566 = 0x15be;
        public const int X_42567 = 0x15bf;
        public const int X_42568 = 0x15c0;
        public const int X_42569 = 0x15c1;
        public const int X_42570 = 0x15c2;
        public const int X_42571 = 0x15c3;
        public const int X_42572 = 0x15c4;
        public const int X_42573 = 0x15c5;
        public const int X_42574 = 0x15c6;
        public const int X_42575 = 0x15c7;
        public const int X_42576 = 0x15c8;
        public const int X_42577 = 0x15c9;
        public const int X_42578 = 0x15ca;
        public const int X_42579 = 0x15cb;
        public const int X_42580 = 0x15cc;
        public const int X_42581 = 0x15cd;
        public const int X_42582 = 0x15ce;
        public const int X_42583 = 0x15cf;
        public const int X_42584 = 0x15d0;
        public const int X_42585 = 0x15d1;
        public const int X_42586 = 0x15d2;
        public const int X_42587 = 0x15d3;
        public const int X_42588 = 0x15d4;
        public const int X_42589 = 0x15d5;
        public const int X_42590 = 0x15d6;
        public const int X_42591 = 0x15d7;
        public const int X_42592 = 0x15d8;
        public const int X_42593 = 0x15d9;
        public const int X_42594 = 0x15da;
        public const int X_42595 = 0x15db;
        public const int X_42596 = 0x15dc;
        public const int X_42597 = 0x15dd;
        public const int X_42598 = 0x15de;
        public const int X_42599 = 0x15df;
        public const int X_42601 = 0x15e1;
        public const int X_42602 = 0x15e2;
        public const int X_42603 = 0x15e3;
        public const int X_42604 = 0x15e4;
        public const int X_42605 = 0x15e5;
        public const int X_42606 = 0x15e6;
        public const int X_42607 = 0x15e7;
        public const int X_42608 = 0x15e8;
        public const int X_42609 = 0x15e9;
        public const int X_42610 = 0x15ea;
        public const int X_44000 = 0x1644;
        public const int X_45000 = 0x16a8;
        public const int X_46000 = 0x1770;
        public const int X_46001 = 0x1771;
        public const int X_46002 = 0x1772;
        public const int X_46003 = 0x1773;
        public const int X_46005 = 0x1774;
        public const int X_4600A = 0x1777;
        public const int X_4600B = 0x1778;
        public const int X_4600C = 0x1779;
        public const int X_4600D = 0x177a;
        public const int X_4600E = 0x177b;
        public const int X_46102 = 0x177c;
        public const int X_46103 = 0x177d;
        public const int X_46511 = 0x1785;
        public const int X_99000 = 0x1964;
        public const int X_99099 = 0x1965;
        public const int X_HV000 = 0x19c8;
        public const int X_HV001 = 0x19c9;
        public const int X_HV002 = 0x19ca;
        public const int X_HV004 = 0x19cb;
        public const int X_HV005 = 0x19cc;
        public const int X_HV006 = 0x19cd;
        public const int X_HV007 = 0x19ce;
        public const int X_HV008 = 0x19cf;
        public const int X_HV009 = 0x19d0;
        public const int X_HV00A = 0x19d1;
        public const int X_HV00B = 0x19d2;
        public const int X_HV00C = 0x19d3;
        public const int X_HV00D = 0x19d4;
        public const int X_HV00J = 0x19d5;
        public const int X_HV00K = 0x19d6;
        public const int X_HV00L = 0x19d7;
        public const int X_HV00M = 0x19d8;
        public const int X_HV00N = 0x19d9;
        public const int X_HV00P = 0x19da;
        public const int X_HV00Q = 0x19db;
        public const int X_HV00R = 0x19dc;
        public const int X_HV010 = 0x19dd;
        public const int X_HV014 = 0x19de;
        public const int X_HV021 = 0x19df;
        public const int X_HV024 = 0x19e0;
        public const int X_HV090 = 0x19e1;
        public const int X_HV091 = 0x19e2;
        public const int X_HW000 = 0x1a2c;
        public const int X_HW001 = 0x1a2d;
        public const int X_HW002 = 0x1a2e;
        public const int X_HW003 = 0x1a2f;
        public const int X_HW004 = 0x1a30;
        public const int X_HW005 = 0x1a31;
        public const int X_HW006 = 0x1a32;
        public const int X_HW007 = 0x1a33;
        public const int X_HY093 = 0x1a90;
        public const int X_Z6801 = 0x1a91;

        public static bool IsWarning(string sqlState)
        {
            return sqlState.StartsWith("01");
        }
    }
}
