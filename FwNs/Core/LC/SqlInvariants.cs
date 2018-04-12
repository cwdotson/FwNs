namespace FwNs.Core.LC
{
    using FwNs.Core.LC.cDataTypes;
    using FwNs.Core.LC.cErrors;
    using FwNs.Core.LC.cSchemas;
    using System;

    public sealed class SqlInvariants
    {
        public const string SystemAuthorizationName = "SYS";
        public const string DbaAdminRoleName = "DBA";
        public const string SchemaCreateRoleName = "CREATE_SCHEMA";
        public const string ChangeAuthRoleName = "CHANGE_AUTHORIZATION";
        public const string SystemSubquery = "SYSTEM_SUBQUERY";
        public const string PublicRoleName = "PUBLIC";
        public const string SystemSchema = "SYSTEM_SCHEMA";
        public const string LobsSchema = "SYSTEM_LOBS";
        public const string DefinitionSchema = "DEFINITION_SCHEMA";
        public const string InformationSchema = "INFORMATION_SCHEMA";
        public const string SqljSchema = "SQLJ";
        public const string PublicSchema = "PUBLIC";
        public const string ClasspathName = "CLASSPATH";
        public const string Module = "MODULE";
        public const string Dual = "DUAL";
        public const string Dummy = "DUMMY";
        public static QNameManager.QName InformationSchemaQname = QNameManager.NewSystemObjectName("INFORMATION_SCHEMA", 2);
        public static QNameManager.QName SystemSchemaQname = QNameManager.NewSystemObjectName("SYSTEM_SCHEMA", 2);
        public static QNameManager.QName LobsSchemaQname = QNameManager.NewSystemObjectName("SYSTEM_LOBS", 2);
        public static QNameManager.QName SqljSchemaQname = QNameManager.NewSystemObjectName("SQLJ", 2);
        public static QNameManager.QName SystemSubqueryQname = GetSystemSubqueryQname();
        public static QNameManager.QName ModuleQname = QNameManager.NewSystemObjectName("MODULE", 2);
        public static QNameManager.QName DualTableQname = QNameManager.NewSystemObjectName("DUAL", 3);
        public static QNameManager.QName DualColumnQname = QNameManager.NewSystemObjectName("DUMMY", 9);
        public static Charset SqlText = new Charset(QNameManager.NewInfoSchemaObjectName("SQL_TEXT", false, 14));
        public static Charset SqlIdentifierCharset = new Charset(QNameManager.NewInfoSchemaObjectName("SQL_IDENTIFIER", false, 14));
        public static Charset SqlCharacter = new Charset(QNameManager.NewInfoSchemaObjectName("SQL_CHARACTER", false, 14));
        public static Charset AsciiGraphic = new Charset(QNameManager.NewInfoSchemaObjectName("ASCII_GRAPHIC", false, 14));
        public static Charset GraphicIrv = new Charset(QNameManager.NewInfoSchemaObjectName("GRAPHIC_IRV", false, 14));
        public static Charset AsciiFull = new Charset(QNameManager.NewInfoSchemaObjectName("ASCII_FULL", false, 14));
        public static Charset Iso8Bit = new Charset(QNameManager.NewInfoSchemaObjectName("ISO8BIT", false, 14));
        public static Charset Latin1 = new Charset(QNameManager.NewInfoSchemaObjectName("LATIN1", false, 14));
        public static Charset Utf32 = new Charset(QNameManager.NewInfoSchemaObjectName("UTF32", false, 14));
        public static Charset Utf16 = new Charset(QNameManager.NewInfoSchemaObjectName("UTF16", false, 14));
        public static Charset Utf8 = new Charset(QNameManager.NewInfoSchemaObjectName("UTF8", false, 14));
        public static SqlType CardinalNumber = GetCardinalNumber();
        public static SqlType YesOrNo = GetYesOrNo();
        public static SqlType CharacterData = GetCharacterData();
        public static SqlType SqlIdentifier = GetSqlIdentifier();
        public static SqlType TimeStamp = GetTimeStamp();

        public static void CheckSchemaNameNotSystem(string name)
        {
            if (IsSystemSchemaName(name))
            {
                throw Error.GetError(0x157f, name);
            }
        }

        private static SqlType GetCardinalNumber()
        {
            QNameManager.QName name = QNameManager.NewInfoSchemaObjectName("CARDINAL_NUMBER", false, 13);
            CardinalNumber = new NumberType(0x19, 0L, 0);
            CardinalNumber.userTypeModifier = new UserTypeModifier(name, 13, CardinalNumber);
            return CardinalNumber;
        }

        private static SqlType GetCharacterData()
        {
            QNameManager.QName name = QNameManager.NewInfoSchemaObjectName("CHARACTER_DATA", false, 13);
            CharacterData = new CharacterType(12, 0x10000L);
            CharacterData.userTypeModifier = new UserTypeModifier(name, 13, CharacterData);
            return CharacterData;
        }

        private static SqlType GetSqlIdentifier()
        {
            QNameManager.QName name = QNameManager.NewInfoSchemaObjectName("SQL_IDENTIFIER", false, 13);
            SqlIdentifier = new CharacterType(12, 0x80L);
            SqlIdentifier.userTypeModifier = new UserTypeModifier(name, 13, SqlIdentifier);
            return SqlIdentifier;
        }

        private static QNameManager.QName GetSystemSubqueryQname()
        {
            QNameManager.QName name1 = QNameManager.NewSystemObjectName("SYSTEM_SUBQUERY", 3);
            name1.SetSchemaIfNull(SystemSchemaQname);
            return name1;
        }

        private static SqlType GetTimeStamp()
        {
            QNameManager.QName name = QNameManager.NewInfoSchemaObjectName("TIME_STAMP", false, 13);
            TimeStamp = new DateTimeType(0x5d, 0x5d, 6);
            TimeStamp.userTypeModifier = new UserTypeModifier(name, 13, TimeStamp);
            return TimeStamp;
        }

        private static SqlType GetYesOrNo()
        {
            QNameManager.QName name = QNameManager.NewInfoSchemaObjectName("YES_OR_NO", false, 13);
            YesOrNo = new CharacterType(12, 3L);
            YesOrNo.userTypeModifier = new UserTypeModifier(name, 13, YesOrNo);
            return YesOrNo;
        }

        public static bool IsLobsSchemaName(string name)
        {
            return "SYSTEM_LOBS".Equals(name);
        }

        public static bool IsSystemSchemaName(QNameManager.QName name)
        {
            if (name.schema != null)
            {
                name = name.schema;
            }
            if (!InformationSchemaQname.Equals(name) && !SystemSchemaQname.Equals(name))
            {
                return SqljSchemaQname.Equals(name);
            }
            return true;
        }

        public static bool IsSystemSchemaName(string name)
        {
            if ((!"DEFINITION_SCHEMA".Equals(name) && !"INFORMATION_SCHEMA".Equals(name)) && !"SYSTEM_SCHEMA".Equals(name))
            {
                return "SQLJ".Equals(name);
            }
            return true;
        }
    }
}

