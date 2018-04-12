namespace FwNs.Core.LC.cParsing
{
    using System;
    using System.Collections.Generic;

    public class Tokens
    {
        public const string T_ABS = "ABS";
        public const string T_AGGREGATE = "AGGREGATE";
        public const string T_ALL = "ALL";
        public const string T_ALLOCATE = "ALLOCATE";
        public const string T_ALTER = "ALTER";
        public const string T_AND = "AND";
        public const string T_ANY = "ANY";
        public const string T_ARE = "ARE";
        public const string T_ARRAY = "ARRAY";
        public const string T_AS = "AS";
        public const string T_ASENSITIVE = "ASENSITIVE";
        public const string T_ASYMMETRIC = "ASYMMETRIC";
        public const string T_AT = "AT";
        public const string T_ATOMIC = "ATOMIC";
        public const string T_AUTHORIZATION = "AUTHORIZATION";
        public const string T_AVG = "AVG";
        public const string T_BEGIN = "BEGIN";
        public const string T_BETWEEN = "BETWEEN";
        public const string T_BIGINT = "BIGINT";
        public const string T_BINARY = "BINARY";
        public const string T_BIT_LENGTH = "BIT_LENGTH";
        public const string T_BLOB = "BLOB";
        public const string T_BODY = "BODY";
        public const string T_BOOLEAN = "BOOLEAN";
        public const string T_BOTH = "BOTH";
        public const string T_BY = "BY";
        public const string T_BYTE = "BYTE";
        public const string T_CALL = "CALL";
        public const string T_CALLED = "CALLED";
        public const string T_CARDINALITY = "CARDINALITY";
        public const string T_CASCADED = "CASCADED";
        public const string T_CASE = "CASE";
        public const string T_CAST = "CAST";
        public const string T_CEIL = "CEIL";
        public const string T_CEILING = "CEILING";
        public const string T_CHR = "CHR";
        public const string T_CHAR = "CHAR";
        public const string T_CHAR_LENGTH = "CHAR_LENGTH";
        public const string T_CHARACTER = "CHARACTER";
        public const string T_CHARACTER_LENGTH = "CHARACTER_LENGTH";
        public const string T_CHECK = "CHECK";
        public const string T_CLOB = "CLOB";
        public const string T_CLOSE = "CLOSE";
        public const string T_COALESCE = "COALESCE";
        public const string T_COLLATE = "COLLATE";
        public const string T_COLLECT = "COLLECT";
        public const string T_COLUMN = "COLUMN";
        public const string T_COMMIT = "COMMIT";
        public const string T_COMMENT = "COMMENT";
        public const string T_CONDITION = "CONDITION";
        public const string T_CONNECT = "CONNECT";
        public const string T_CONSTRAINT = "CONSTRAINT";
        public const string T_CONVERT = "CONVERT";
        public const string T_CORR = "CORR";
        public const string T_CORRESPONDING = "CORRESPONDING";
        public const string T_COUNT = "COUNT";
        public const string T_COVAR_POP = "COVAR_POP";
        public const string T_COVAR_SAMP = "COVAR_SAMP";
        public const string T_CREATE = "CREATE";
        public const string T_CROSS = "CROSS";
        public const string T_CUBE = "CUBE";
        public const string T_CUME_DIST = "CUME_DIST";
        public const string T_CURRENT = "CURRENT";
        public const string T_CURRENT_CATALOG = "CURRENT_CATALOG";
        public const string T_CURRENT_DATE = "CURRENT_DATE";
        public const string T_CURRENT_DEFAULT_TRANSFORM_GROUP = "CURRENT_DEFAULT_TRANSFORM_GROUP";
        public const string T_CURRENT_PATH = "CURRENT_PATH";
        public const string T_CURRENT_ROLE = "CURRENT_ROLE";
        public const string T_CURRENT_SCHEMA = "CURRENT_SCHEMA";
        public const string T_CURRENT_TIME = "CURRENT_TIME";
        public const string T_CURRENT_TIMESTAMP = "CURRENT_TIMESTAMP";
        public const string T_CURRENT_TRANSFORM_GROUP_FOR_TYPE = "CURRENT_TRANSFORM_GROUP_FOR_TYPE";
        public const string T_CURRENT_USER = "CURRENT_USER";
        public const string T_CURSOR = "CURSOR";
        public const string T_CURRVAL = "CURRVAL";
        public const string T_CYCLE = "CYCLE";
        public const string T_DATE = "DATE";
        public const string T_DAY = "DAY";
        public const string T_DEALLOCATE = "DEALLOCATE";
        public const string T_DEC = "DEC";
        public const string T_DECIMAL = "DECIMAL";
        public const string T_DECLARE = "DECLARE";
        public const string T_DEFAULT = "DEFAULT";
        public const string T_DELETE = "DELETE";
        public const string T_DENSE_RANK = "DENSE_RANK";
        public const string T_DEREF = "DEREF";
        public const string T_DESCRIBE = "DESCRIBE";
        public const string T_DETERMINISTIC = "DETERMINISTIC";
        public const string T_DISCONNECT = "DISCONNECT";
        public const string T_DISTINCT = "DISTINCT";
        public const string T_DO = "DO";
        public const string T_DOUBLE = "DOUBLE";
        public const string T_DROP = "DROP";
        public const string T_DUAL = "DUAL";
        public const string T_DYNAMIC = "DYNAMIC";
        public const string T_EACH = "EACH";
        public const string T_LibCore_ERRNO = "LibCore_ERRNO";
        public const string T_ELEMENT = "ELEMENT";
        public const string T_ELSE = "ELSE";
        public const string T_ELSIF = "ELSIF";
        public const string T_ELSEIF = "ELSEIF";
        public const string T_END = "END";
        public const string T_END_EXEC = "END_EXEC";
        public const string T_ESCAPE = "ESCAPE";
        public const string T_EVERY = "EVERY";
        public const string T_EXCEPT = "EXCEPT";
        public const string T_EXEC = "EXEC";
        public const string T_EXECUTE = "EXECUTE";
        public const string T_EXISTS = "EXISTS";
        public const string T_EXP = "EXP";
        public const string T_EXTERNAL = "EXTERNAL";
        public const string T_EXTRACT = "EXTRACT";
        public const string T_FALSE = "FALSE";
        public const string T_FETCH = "FETCH";
        public const string T_FILTER = "FILTER";
        public const string T_FIRST_VALUE = "FIRST_VALUE";
        public const string T_FLOAT = "FLOAT";
        public const string T_FLOOR = "FLOOR";
        public const string T_FOR = "FOR";
        public const string T_FOREIGN = "FOREIGN";
        public const string T_FREE = "FREE";
        public const string T_FROM = "FROM";
        public const string T_FULL = "FULL";
        public const string T_FUNCTION = "FUNCTION";
        public const string T_FUSION = "FUSION";
        public const string T_GET = "GET";
        public const string T_GLOBAL = "GLOBAL";
        public const string T_GRANT = "GRANT";
        public const string T_GROUP = "GROUP";
        public const string T_GROUPING = "GROUPING";
        public const string T_HANDLER = "HANDLER";
        public const string T_HAVING = "HAVING";
        public const string T_HOLD = "HOLD";
        public const string T_HOUR = "HOUR";
        public const string T_IDENTITY = "IDENTITY";
        public const string T_IF = "IF";
        public const string T_IMPORT = "IMPORT";
        public const string T_IN = "IN";
        public const string T_INDICATOR = "INDICATOR";
        public const string T_INNER = "INNER";
        public const string T_INOUT = "INOUT";
        public const string T_INSENSITIVE = "INSENSITIVE";
        public const string T_INSERT = "INSERT";
        public const string T_INT = "INT";
        public const string T_INTEGER = "INTEGER";
        public const string T_INTERSECT = "INTERSECT";
        public const string T_INTERSECTION = "INTERSECTION";
        public const string T_INTERVAL = "INTERVAL";
        public const string T_INTO = "INTO";
        public const string T_ITERATE = "ITERATE";
        public const string T_IS = "IS";
        public const string T_JAR = "JAR";
        public const string T_JOIN = "JOIN";
        public const string T_LAG = "LAG";
        public const string T_LANGUAGE = "LANGUAGE";
        public const string T_LARGE = "LARGE";
        public const string T_LAST_VALUE = "LAST_VALUE";
        public const string T_LATERAL = "LATERAL";
        public const string T_LEAD = "LEAD";
        public const string T_LEADING = "LEADING";
        public const string T_LEAVE = "LEAVE";
        public const string T_LEFT = "LEFT";
        public const string T_LIKE = "LIKE";
        public const string T_REGEXP_LIKE = "REGEXP_LIKE";
        public const string T_LN = "LN";
        public const string T_LOCAL = "LOCAL";
        public const string T_LOCALTIME = "LOCALTIME";
        public const string T_LOCALTIMESTAMP = "LOCALTIMESTAMP";
        public const string T_LOOP = "LOOP";
        public const string T_LOWER = "LOWER";
        public const string T_LPAD = "LPAD";
        public const string T_MATCH = "MATCH";
        public const string T_MAX = "MAX";
        public const string T_MAX_CARDINALITY = "MAX_CARDINALITY";
        public const string T_MEMBER = "MEMBER";
        public const string T_MERGE = "MERGE";
        public const string T_METHOD = "METHOD";
        public const string T_NEXTVAL = "NEXTVAL";
        public const string T_MIN = "MIN";
        public const string T_MINUTE = "MINUTE";
        public const string T_MOD = "MOD";
        public const string T_MODIFIES = "MODIFIES";
        public const string T_MODULE = "MODULE";
        public const string T_MONTH = "MONTH";
        public const string T_MULTISET = "MULTISET";
        public const string T_NATIONAL = "NATIONAL";
        public const string T_NATURAL = "NATURAL";
        public const string T_NCHAR = "NCHAR";
        public const string T_NCLOB = "NCLOB";
        public const string T_NVARCHAR = "NVARCHAR";
        public const string T_NVARCHAR2 = "NVARCHAR2";
        public const string T_NEW = "NEW";
        public const string T_NEWID = "NEWID";
        public const string T_NO = "NO";
        public const string T_NONE = "NONE";
        public const string T_NORMALIZE = "NORMALIZE";
        public const string T_NOT = "NOT";
        public const string T_NTH_VALUE = "NTH_VALUE";
        public const string T_NTILE = "NTILE";
        public const string T_NULL = "NULL";
        public const string T_NULLIF = "NULLIF";
        public const string T_NUMERIC = "NUMERIC";
        public const string T_OCCURRENCES_REGEX = "OCCURRENCES_REGEX";
        public const string T_OCTET_LENGTH = "OCTET_LENGTH";
        public const string T_OF = "OF";
        public const string T_OFFSET = "OFFSET";
        public const string T_OLD = "OLD";
        public const string T_ON = "ON";
        public const string T_ONLY = "ONLY";
        public const string T_OPEN = "OPEN";
        public const string T_OR = "OR";
        public const string T_ORDER = "ORDER";
        public const string T_OUT = "OUT";
        public const string T_OUTER = "OUTER";
        public const string T_OVER = "OVER";
        public const string T_OVERLAPS = "OVERLAPS";
        public const string T_OVERLAY = "OVERLAY";
        public const string T_PARAMETER = "PARAMETER";
        public const string T_PARTITION = "PARTITION";
        public const string T_PERCENT_RANK = "PERCENT_RANK";
        public const string T_PERCENTILE_CONT = "PERCENTILE_CONT";
        public const string T_PERCENTILE_DISC = "PERCENTILE_DISC";
        public const string T_POSITION = "POSITION";
        public const string T_REGEXP_REPLACE = "REGEXP_REPLACE";
        public const string T_POW = "POW";
        public const string T_POWER = "POWER";
        public const string T_PRECISION = "PRECISION";
        public const string T_PREPARE = "PREPARE";
        public const string T_PRIMARY = "PRIMARY";
        public const string T_PROCEDURE = "PROCEDURE";
        public const string T_RANGE = "RANGE";
        public const string T_RANK = "RANK";
        public const string T_READS = "READS";
        public const string T_REAL = "REAL";
        public const string T_RECURSIVE = "RECURSIVE";
        public const string T_REF = "REF";
        public const string T_REFERENCES = "REFERENCES";
        public const string T_REFERENCING = "REFERENCING";
        public const string T_REGR_AVGX = "REGR_AVGX";
        public const string T_REGR_AVGY = "REGR_AVGY";
        public const string T_REGR_COUNT = "REGR_COUNT";
        public const string T_REGR_INTERCEPT = "REGR_INTERCEPT";
        public const string T_REGR_R2 = "REGR_R2";
        public const string T_REGR_SLOPE = "REGR_SLOPE";
        public const string T_REGR_SXX = "REGR_SXX";
        public const string T_REGR_SXY = "REGR_SXY";
        public const string T_REGR_SYY = "REGR_SYY";
        public const string T_RELEASE = "RELEASE";
        public const string T_REPEAT = "REPEAT";
        public const string T_RESIGNAL = "RESIGNAL";
        public const string T_RESULT = "RESULT";
        public const string T_RETURN = "RETURN";
        public const string T_RETURNS = "RETURNS";
        public const string T_REVOKE = "REVOKE";
        public const string T_RIGHT = "RIGHT";
        public const string T_ROLLBACK = "ROLLBACK";
        public const string T_ROLLUP = "ROLLUP";
        public const string T_ROW = "ROW";
        public const string T_ROW_NUMBER = "ROW_NUMBER";
        public const string T_ROWS = "ROWS";
        public const string T_RPAD = "RPAD";
        public const string T_SAVEPOINT = "SAVEPOINT";
        public const string T_SCOPE = "SCOPE";
        public const string T_SCROLL = "SCROLL";
        public const string T_SEARCH = "SEARCH";
        public const string T_SECOND = "SECOND";
        public const string T_SELECT = "SELECT";
        public const string T_SELECT_TYPE = "SELECT_TYPE";
        public const string T_SENSITIVE = "SENSITIVE";
        public const string T_SESSION_USER = "SESSION_USER";
        public const string T_SET = "SET";
        public const string T_SIGNAL = "SIGNAL";
        public const string T_SIMILAR = "SIMILAR";
        public const string T_SMALLINT = "SMALLINT";
        public const string T_SOME = "SOME";
        public const string T_SPECIFIC = "SPECIFIC";
        public const string T_SPECIFICTYPE = "SPECIFICTYPE";
        public const string T_SQL = "SQL";
        public const string T_SQLEXCEPTION = "SQLEXCEPTION";
        public const string T_SQLSTATE = "SQLSTATE";
        public const string T_SQLWARNING = "SQLWARNING";
        public const string T_SQRT = "SQRT";
        public const string T_START = "START";
        public const string T_STATIC = "STATIC";
        public const string T_STDDEV_POP = "STDDEV_POP";
        public const string T_STDDEV_SAMP = "STDDEV_SAMP";
        public const string T_STDDEV = "STDDEV";
        public const string T_SUBMULTISET = "SUBMULTISET";
        public const string T_SUBSTRING = "SUBSTRING";
        public const string T_REGEXP_SUBSTR = "REGEXP_SUBSTR";
        public const string T_SUM = "SUM";
        public const string T_SYMMETRIC = "SYMMETRIC";
        public const string T_SYSTEM = "SYSTEM";
        public const string T_SYSTEM_USER = "SYSTEM_USER";
        public const string T_TABLE = "TABLE";
        public const string T_TABLESAMPLE = "TABLESAMPLE";
        public const string T_THEN = "THEN";
        public const string T_TIME = "TIME";
        public const string T_TIMESTAMP = "TIMESTAMP";
        public const string T_TIMEZONE_HOUR = "TIMEZONE_HOUR";
        public const string T_TIMEZONE_MINUTE = "TIMEZONE_MINUTE";
        public const string T_TO = "TO";
        public const string T_TRAILING = "TRAILING";
        public const string T_TRANSLATE = "TRANSLATE";
        public const string T_REGEXP_INSTR = "REGEXP_INSTR";
        public const string T_TRANSLATION = "TRANSLATION";
        public const string T_TREAT = "TREAT";
        public const string T_TRIGGER = "TRIGGER";
        public const string T_TRIM = "TRIM";
        public const string T_TRIM_ARRAY = "TRIM_ARRAY";
        public const string T_TRUE = "TRUE";
        public const string T_TRUNCATE = "TRUNCATE";
        public const string T_UESCAPE = "UESCAPE";
        public const string T_UNION = "UNION";
        public const string T_UNIQUE = "UNIQUE";
        public const string T_UNKNOWN = "UNKNOWN";
        public const string T_UNNEST = "UNNEST";
        public const string T_UNTIL = "UNTIL";
        public const string T_UPDATE = "UPDATE";
        public const string T_UPPER = "UPPER";
        public const string T_USER = "USER";
        public const string T_USING = "USING";
        public const string T_VALUE = "VALUE";
        public const string T_VALUES = "VALUES";
        public const string T_VAR_POP = "VAR_POP";
        public const string T_VAR_SAMP = "VAR_SAMP";
        public const string T_VARIANCE = "VARIANCE";
        public const string T_VARBINARY = "VARBINARY";
        public const string T_VARCHAR = "VARCHAR";
        public const string T_VARYING = "VARYING";
        public const string T_WHEN = "WHEN";
        public const string T_WHENEVER = "WHENEVER";
        public const string T_WHERE = "WHERE";
        public const string T_WHILE = "WHILE";
        public const string T_WIDTH_BUCKET = "WIDTH_BUCKET";
        public const string T_WINDOW = "WINDOW";
        public const string T_WITH = "WITH";
        public const string T_WITHIN = "WITHIN";
        public const string T_WITHOUT = "WITHOUT";
        public const string T_YEAR = "YEAR";
        public const string T_VARCHAR2 = "VARCHAR2";
        public const string T_ASTERISK = "*";
        public const string T_COMMA = ",";
        public const string T_CIRCUMFLEX = "^";
        public const string T_CLOSEBRACKET = ")";
        public const string T_COLON = ":";
        public const string T_CONCAT = "||";
        public const string T_DIVIDE = "/";
        public const string T_EQUALS = "=";
        public const string T_GREATER = ">";
        public const string T_GREATER_EQUALS = ">=";
        public const string T_LESS = "<";
        public const string T_LESS_EQUALS = "<=";
        public const string T_PERCENT = "%";
        public const string T_PLUS = "+";
        public const string T_MINUS = "-";
        public const string T_NOT_EQUALS = "<>";
        public const string T_NOT_EQUALS_ALT = "!=";
        public const string T_OPENBRACKET = "(";
        public const string T_QUESTION = "?";
        public const string T_SEMICOLON = ";";
        public const string T_DOUBLE_COLON = "::";
        public const string T_ATSYM = "@";
        public const string T_ASSIGN = ":=";
        public const string T_DOUBLE_GREATER = ">>";
        public const string T_DOUBLE_LESS = "<<";
        public const string T_DOT_DOT = "..";
        public const string T_LEFTBRACKET = "[";
        public const string T_RIGHTBRACKET = "]";
        public const string T_A = "A";
        public const string T_ABSOLUTE = "ABSOLUTE";
        public const string T_ACTION = "ACTION";
        public const string T_ADA = "ADA";
        public const string T_ADMIN = "ADMIN";
        public const string T_AFTER = "AFTER";
        public const string T_ALWAYS = "ALWAYS";
        public const string T_ASC = "ASC";
        public const string T_ASSERTION = "ASSERTION";
        public const string T_ASSIGNMENT = "ASSIGNMENT";
        public const string T_ATTRIBUTE = "ATTRIBUTE";
        public const string T_ATTRIBUTES = "ATTRIBUTES";
        public const string T_BEFORE = "BEFORE";
        public const string T_BERNOULLI = "BERNOULLI";
        public const string T_BREADTH = "BREADTH";
        public const string T_C = "C";
        public const string T_CASCADE = "CASCADE";
        public const string T_CATALOG = "CATALOG";
        public const string T_CATALOG_NAME = "CATALOG_NAME";
        public const string T_CHAIN = "CHAIN";
        public const string T_CHARACTER_SET_CATALOG = "CHARACTER_SET_CATALOG";
        public const string T_CHARACTER_SET_NAME = "CHARACTER_SET_NAME";
        public const string T_CHARACTER_SET_SCHEMA = "CHARACTER_SET_SCHEMA";
        public const string T_CHARACTERISTICS = "CHARACTERISTICS";
        public const string T_CHARACTERS = "CHARACTERS";
        public const string T_CLASS_ORIGIN = "CLASS_ORIGIN";
        public const string T_COBOL = "COBOL";
        public const string T_COLLATION = "COLLATION";
        public const string T_COLLATION_CATALOG = "COLLATION_CATALOG";
        public const string T_COLLATION_NAME = "COLLATION_NAME";
        public const string T_COLLATION_SCHEMA = "COLLATION_SCHEMA";
        public const string T_COLUMN_NAME = "COLUMN_NAME";
        public const string T_COMMAND_FUNCTION = "COMMAND_FUNCTION";
        public const string T_COMMAND_FUNCTION_CODE = "COMMAND_FUNCTION_CODE";
        public const string T_COMMITTED = "COMMITTED";
        public const string T_COMPARABLE = "COMPARABLE";
        public const string T_CONDITION_IDENTIFIER = "CONDIITON_IDENTIFIER";
        public const string T_CONDITION_NUMBER = "CONDITION_NUMBER";
        public const string T_CONNECTION_NAME = "CONNECTION_NAME";
        public const string T_CONSTRAINT_CATALOG = "CONSTRAINT_CATALOG";
        public const string T_CONSTRAINT_NAME = "CONSTRAINT_NAME";
        public const string T_CONSTRAINT_SCHEMA = "CONSTRAINT_SCHEMA";
        public const string T_CONSTRAINTS = "CONSTRAINTS";
        public const string T_CONSTRUCTOR = "CONSTRUCTOR";
        public const string T_CONTAINS = "CONTAINS";
        public const string T_CONTINUE = "CONTINUE";
        public const string T_CURRENT_COLLATION = "CURRENT_COLLATION";
        public const string T_CURSOR_NAME = "CURSOR_NAME";
        public const string T_DATA = "DATA";
        public const string T_DATETIME_INTERVAL_CODE = "DATETIME_INTERVAL_CODE";
        public const string T_DATETIME_INTERVAL_PRECISION = "DATETIME_INTERVAL_PRECISION";
        public const string T_DEFAULTS = "DEFAULTS";
        public const string T_DEFERRABLE = "DEFERRABLE";
        public const string T_DEFERRED = "DEFERRED";
        public const string T_DEFINED = "DEFINED";
        public const string T_DEFINER = "DEFINER";
        public const string T_DEGREE = "DEGREE";
        public const string T_DEPTH = "DEPTH";
        public const string T_DERIVED = "DERIVED";
        public const string T_DESC = "DESC";
        public const string T_DESCRIPTOR = "DESCRIPTOR";
        public const string T_DIAGNOSTICS = "DIAGNOSTICS";
        public const string T_DISPATCH = "DISPATCH";
        public const string T_DOMAIN = "DOMAIN";
        public const string T_DYNAMIC_FUNCTION = "DYNAMIC_FUNCTION";
        public const string T_DYNAMIC_FUNCTION_CODE = "DYNAMIC_FUNCTION_CODE";
        public const string T_EXCEPTION = "EXCEPTION";
        public const string T_EXCLUDE = "EXCLUDE";
        public const string T_EXCLUDING = "EXCLUDING";
        public const string T_EXIT = "EXIT";
        public const string T_FINAL = "FINAL";
        public const string T_FIRST = "FIRST";
        public const string T_FOLLOWING = "FOLLOWING";
        public const string T_FORTRAN = "FORTRAN";
        public const string T_FOUND = "FOUND";
        public const string T_G_FACTOR = "G";
        public const string T_GENERAL = "GENERAL";
        public const string T_GO = "GO";
        public const string T_GOTO = "GOTO";
        public const string T_GRANTED = "GRANTED";
        public const string T_HIERARCHY = "HIERARCHY";
        public const string T_IMPLEMENTATION = "IMPLEMENTATION";
        public const string T_INCLUDING = "INCLUDING";
        public const string T_INCREMENT = "INCREMENT";
        public const string T_INITIALLY = "INITIALLY";
        public const string T_INPUT = "INPUT";
        public const string T_INSTANCE = "INSTANCE";
        public const string T_INSTANTIABLE = "INSTANTIABLE";
        public const string T_INSTEAD = "INSTEAD";
        public const string T_INTERFACE = "INTERFACE";
        public const string T_INVOKER = "INVOKER";
        public const string T_ISOLATION = "ISOLATION";
        public const string T_DOTNET = "DOTNET";
        public const string T_K_FACTOR = "K";
        public const string T_KEY = "KEY";
        public const string T_KEY_MEMBER = "KEY_MEMBER";
        public const string T_KEY_TYPE = "KEY_TYPE";
        public const string T_LAST = "LAST";
        public const string T_LENGTH = "LENGTH";
        public const string T_LEVEL = "LEVEL";
        public const string T_LIBRARY = "LIBRARY";
        public const string T_LOCATOR = "LOCATOR";
        public const string T_M_FACTOR = "M";
        public const string T_MAP = "MAP";
        public const string T_MATCHED = "MATCHED";
        public const string T_MAXVALUE = "MAXVALUE";
        public const string T_MESSAGE_LENGTH = "MESSAGE_LENGTH";
        public const string T_MESSAGE_OCTET_LENGTH = "MESSAGE_OCTET_LENGTH";
        public const string T_MESSAGE_TEXT = "MESSAGE_TEXT";
        public const string T_MINVALUE = "MINVALUE";
        public const string T_MORE = "MORE";
        public const string T_MUMPS = "MUMPS";
        public const string T_NAME = "NAME";
        public const string T_NAMES = "NAMES";
        public const string T_NESTING = "NESTING";
        public const string T_NEXT = "NEXT";
        public const string T_NORMALIZED = "NORMALIZED";
        public const string T_NULLABLE = "NULLABLE";
        public const string T_NULLS = "NULLS";
        public const string T_NUMBER = "NUMBER";
        public const string T_OBJECT = "OBJECT";
        public const string T_OCTETS = "OCTETS";
        public const string T_OPTION = "OPTION";
        public const string T_OPTIONS = "OPTIONS";
        public const string T_ORDERING = "ORDERING";
        public const string T_ORDINALITY = "ORDINALITY";
        public const string T_OTHERS = "OTHERS";
        public const string T_OVERRIDING = "OVERRIDING";
        public const string T_P_FACTOR = "P";
        public const string T_PAD = "PAD";
        public const string T_PARAMETER_MODE = "PARAMETER_MODE";
        public const string T_PARAMETER_NAME = "PARAMETER_NAME";
        public const string T_PARAMETER_ORDINAL_POSITION = "PARAMETER_ORDINAL_POSITION";
        public const string T_PARAMETER_SPECIFIC_CATALOG = "PARAMETER_SPECIFIC_CATALOG";
        public const string T_PARAMETER_SPEC_NAME = "PARAMETER_SPECIFIC_NAME";
        public const string T_PARAMETER_SPEC_SCHEMA = "PARAMETER_SPECIFIC_SCHEMA";
        public const string T_PARTIAL = "PARTIAL";
        public const string T_PASCAL = "PASCAL";
        public const string T_PATH = "PATH";
        public const string T_PLACING = "PLACING";
        public const string T_PLI = "PLI";
        public const string T_PRECEDING = "PRECEDING";
        public const string T_PRESERVE = "PRESERVE";
        public const string T_PRIOR = "PRIOR";
        public const string T_PRIVILEGES = "PRIVILEGES";
        public const string T_PUBLIC = "PUBLIC";
        public const string T_READ = "READ";
        public const string T_RELATIVE = "RELATIVE";
        public const string T_REPEATABLE = "REPEATABLE";
        public const string T_RESTART = "RESTART";
        public const string T_RETURNED_CARDINALITY = "RETURNED_CARDINALITY";
        public const string T_RETURNED_LENGTH = "RETURNED_LENGTH";
        public const string T_RETURNED_OCTET_LENGTH = "RETURNED_OCTET_LENGTH";
        public const string T_RETURNED_SQLSTATE = "RETURNED_SQLSTATE";
        public const string T_ROLE = "ROLE";
        public const string T_ROUTINE = "ROUTINE";
        public const string T_ROUTINE_CATALOG = "ROUTINE_CATALOG";
        public const string T_ROUTINE_NAME = "ROUTINE_NAME";
        public const string T_ROUTINE_SCHEMA = "ROUTINE_SCHEMA";
        public const string T_ROW_COUNT = "ROW_COUNT";
        public const string T_SCALE = "SCALE";
        public const string T_SCHEMA = "SCHEMA";
        public const string T_SCHEMA_NAME = "SCHEMA_NAME";
        public const string T_SCOPE_CATALOG = "SCOPE_CATALOG";
        public const string T_SCOPE_NAME = "SCOPE_NAME";
        public const string T_SCOPE_SCHEMA = "SCOPE_SCHEMA";
        public const string T_SECTION = "SECTION";
        public const string T_SECURITY = "SECURITY";
        public const string T_SELF = "SELF";
        public const string T_SEQUENCE = "SEQUENCE";
        public const string T_SERIALIZABLE = "SERIALIZABLE";
        public const string T_SERVER = "SERVER";
        public const string T_SERVER_NAME = "SERVER_NAME";
        public const string T_SESSION = "SESSION";
        public const string T_SETS = "SETS";
        public const string T_SIMPLE = "SIMPLE";
        public const string T_SIZE = "SIZE";
        public const string T_SOURCE = "SOURCE";
        public const string T_SPACE = "SPACE";
        public const string T_SPECIFIC_NAME = "SPECIFIC_NAME";
        public const string T_SQLDATA = "SQLDATA";
        public const string T_STACKED = "STACKED";
        public const string T_STATE = "STATE";
        public const string T_STATEMENT = "STATEMENT";
        public const string T_STRUCTURE = "STRUCTURE";
        public const string T_STYLE = "STYLE";
        public const string T_SUBCLASS_ORIGIN = "SUBCLASS_ORIGIN";
        public const string T_T_FACTOR = "T";
        public const string T_TABLE_NAME = "TABLE_NAME";
        public const string T_TEMPORARY = "TEMPORARY";
        public const string T_TIES = "TIES";
        public const string T_TOP_LEVEL_COUNT = "TOP_LEVEL_COUNT";
        public const string T_TRANSACTION = "TRANSACTION";
        public const string T_TRANSACTION_ID = "TRANSACTION_ID";
        public const string T_TRANSACT_COMMITTED = "TRANSACTIONS_COMMITTED";
        public const string T_TRANSACTION_ROLLED_BACK = "TRANSACTIONS_ROLLED_BACK";
        public const string T_TRANSACT_ACTIVE = "TRANSACTION_ACTIVE";
        public const string T_TRANSFORM = "TRANSFORM";
        public const string T_TRANSFORMS = "TRANSFORMS";
        public const string T_TRIGGER_CATALOG = "TRIGGER_CATALOG";
        public const string T_TRIGGER_NAME = "TRIGGER_NAME";
        public const string T_TRIGGER_SCHEMA = "TRIGGER_SCHEMA";
        public const string T_TYPE = "TYPE";
        public const string T_UNBOUNDED = "UNBOUNDED";
        public const string T_UNCOMMITTED = "UNCOMMITTED";
        public const string T_UNDER = "UNDER";
        public const string T_UNDO = "UNDO";
        public const string T_UNIQUEIDENTIFIER = "UNIQUEIDENTIFIER";
        public const string T_UNNAMED = "UNNAMED";
        public const string T_USAGE = "USAGE";
        public const string T_USER_DEFINED_TYPE_CATALOG = "USER_DEFINED_TYPE_CATALOG";
        public const string T_USER_DEFINED_TYPE_CODE = "USER_DEFINED_TYPE_CODE";
        public const string T_USER_DEFINED_TYPE_NAME = "USER_DEFINED_TYPE_NAME";
        public const string T_USER_DEFINED_TYPE_SCHEMA = "USER_DEFINED_TYPE_SCHEMA";
        public const string T_VIEW = "VIEW";
        public const string T_WORK = "WORK";
        public const string T_WRAPPER = "WRAPPER";
        public const string T_WRITE = "WRITE";
        public const string T_ZONE = "ZONE";
        public const string T_ADD = "ADD";
        public const string T_ALIAS = "ALIAS";
        public const string T_AUTOCOMMIT = "AUTOCOMMIT";
        public const string T_BACKUP = "BACKUP";
        public const string T_BIT = "BIT";
        public const string T_BITLENGTH = "BITLENGTH";
        public const string T_CACHE = "CACHE";
        public const string T_CACHED = "CACHED";
        public const string T_CASEWHEN = "CASEWHEN";
        public const string T_CHECKPOINT = "CHECKPOINT";
        public const string T_CLASS = "CLASS";
        public const string T_COMPACT = "COMPACT";
        public const string T_COMPRESSED = "COMPRESSED";
        public const string T_CONTROL = "CONTROL";
        public const string T_CURDATE = "CURDATE";
        public const string T_CURTIME = "CURTIME";
        public const string T_DATABASE = "DATABASE";
        public const string T_DEFRAG = "DEFRAG";
        public const string T_DELAY = "DELAY";
        public const string T_EXPLAIN = "EXPLAIN";
        public const string T_EVENT = "EVENT";
        public const string T_FILE = "FILE";
        public const string T_FILES = "FILES";
        public const string T_FOLD = "FOLD";
        public const string T_GENERATED = "GENERATED";
        public const string T_HEADER = "HEADER";
        public const string T_ID = "ID";
        public const string T_IDENTIFIED = "IDENTIFIED";
        public const string T_IFNULL = "IFNULL";
        public const string T_IGNORECASE = "IGNORECASE";
        public const string T_IMMEDIATELY = "IMMEDIATELY";
        public const string T_IMMEDIATE = "IMMEDIATE";
        public const string T_INDEX = "INDEX";
        public const string T_INITIAL = "INITIAL";
        public const string T_INTEGRITY = "INTEGRITY";
        public const string T_ISAUTOCOMMIT = "ISAUTOCOMMIT";
        public const string T_ISREADONLYDATABASE = "ISREADONLYDATABASE";
        public const string T_ISREADONLYDATABASEFILES = "ISREADONLYDATABASEFILES";
        public const string T_ISREADONLYSESSION = "ISREADONLYSESSION";
        public const string T_LIMIT = "LIMIT";
        public const string T_LOB = "LOB";
        public const string T_LOCK = "LOCK";
        public const string T_LOCKS = "LOCKS";
        public const string T_MAXROWS = "MAXROWS";
        public const string T_MEMORY = "MEMORY";
        public const string T_MILLIS = "MILLIS";
        public const string T_MINUS_EXCEPT = "MINUS";
        public const string T_MVCC = "MVCC";
        public const string T_MVLOCKS = "MVLOCKS";
        public const string T_NIO = "NIO";
        public const string T_NOCACHE = "NOCACHE";
        public const string T_NOWAIT = "NOWAIT";
        public const string T_NVL = "NVL";
        public const string T_NVL2 = "NVL2";
        public const string T_OCTETLENGTH = "OCTETLENGTH";
        public const string T_OFF = "OFF";
        public const string T_OTHER = "OTHER";
        public const string T_PASSWORD = "PASSWORD";
        public const string T_PLAN = "PLAN";
        public const string T_PROPERTY = "PROPERTY";
        public const string T_QUEUE = "QUEUE";
        public const string T_READONLY = "READONLY";
        public const string T_REFERENTIAL = "REFERENTIAL";
        public const string T_RENAME = "RENAME";
        public const string T_RESTRICT = "RESTRICT";
        public const string T_SCRIPT = "SCRIPT";
        public const string T_SCRIPTFORMAT = "SCRIPTFORMAT";
        public const string T_BLOCKING = "BLOCKING";
        public const string T_SHUTDOWN = "SHUTDOWN";
        public const string T_SQL_TSI_DAY = "SQL_TSI_DAY";
        public const string T_SQL_TSI_FRAC_SECOND = "SQL_TSI_FRAC_SECOND";
        public const string T_SQL_TSI_HOUR = "SQL_TSI_HOUR";
        public const string T_SQL_TSI_MINUTE = "SQL_TSI_MINUTE";
        public const string T_SQL_TSI_MONTH = "SQL_TSI_MONTH";
        public const string T_SQL_TSI_QUARTER = "SQL_TSI_QUARTER";
        public const string T_SQL_TSI_SECOND = "SQL_TSI_SECOND";
        public const string T_SQL_TSI_WEEK = "SQL_TSI_WEEK";
        public const string T_SQL_TSI_YEAR = "SQL_TSI_YEAR";
        public const string T_SQL_BIGINT = "SQL_BIGINT";
        public const string T_SQL_BINARY = "SQL_BINARY";
        public const string T_SQL_BIT = "SQL_BIT";
        public const string T_SQL_BLOB = "SQL_BLOB";
        public const string T_SQL_BOOLEAN = "SQL_BOOLEAN";
        public const string T_SQL_CHAR = "SQL_CHAR";
        public const string T_SQL_CLOB = "SQL_CLOB";
        public const string T_SQL_DATE = "SQL_DATE";
        public const string T_SQL_DECIMAL = "SQL_DECIMAL";
        public const string T_SQL_DATALINK = "SQL_DATALINK";
        public const string T_SQL_DOUBLE = "SQL_DOUBLE";
        public const string T_SQL_FLOAT = "SQL_FLOAT";
        public const string T_SQL_INTEGER = "SQL_INTEGER";
        public const string T_SQL_LONGVARBINARY = "SQL_LONGVARBINARY";
        public const string T_SQL_LONGNVARCHAR = "SQL_LONGNVARCHAR";
        public const string T_SQL_LONGVARCHAR = "SQL_LONGVARCHAR";
        public const string T_SQL_NCHAR = "SQL_NCHAR";
        public const string T_SQL_NCLOB = "SQL_NCLOB";
        public const string T_SQL_NUMERIC = "SQL_NUMERIC";
        public const string T_SQL_NVARCHAR = "SQL_NVARCHAR";
        public const string T_SQL_REAL = "SQL_REAL";
        public const string T_SQL_ROWID = "SQL_ROWID";
        public const string T_SQL_SQLXML = "SQL_SQLXML";
        public const string T_SQL_SMALLINT = "SQL_SMALLINT";
        public const string T_SQL_TIME = "SQL_TIME";
        public const string T_SQL_TIMESTAMP = "SQL_TIMESTAMP";
        public const string T_SQL_TINYINT = "SQL_TINYINT";
        public const string T_SQL_VARBINARY = "SQL_VARBINARY";
        public const string T_SQL_VARCHAR = "SQL_VARCHAR";
        public const string T_TEMP = "TEMP";
        public const string T_TEXT = "TEXT";
        public const string T_TIMESTAMPADD = "TIMESTAMPADD";
        public const string T_TIMESTAMPDIFF = "TIMESTAMPDIFF";
        public const string T_TINYINT = "TINYINT";
        public const string T_TO_CHAR = "TO_CHAR";
        public const string T_TO_DATE = "TO_DATE";
        public const string T_TODAY = "TODAY";
        public const string T_TOP = "TOP";
        public const string T_VARCHAR_IGNORECASE = "VARCHAR_IGNORECASE";
        public const string T_WRITE_DELAY = "WRITE_DELAY";
        public const string T_YES = "YES";
        public const string T_DAY_NAME = "DAY_NAME";
        public const string T_MONTH_NAME = "MONTH_NAME";
        public const string T_QUARTER = "QUARTER";
        public const string T_DAY_OF_WEEK = "DAY_OF_WEEK";
        public const string T_DAY_OF_MONTH = "DAY_OF_MONTH";
        public const string T_DAY_OF_YEAR = "DAY_OF_YEAR";
        public const string T_WEEK_OF_YEAR = "WEEK_OF_YEAR";
        public const string T_DAYNAME = "DAYNAME";
        public const string T_MONTHNAME = "MONTHNAME";
        public const string T_DAYOFMONTH = "DAYOFMONTH";
        public const string T_DAYOFWEEK = "DAYOFWEEK";
        public const string T_DAYOFYEAR = "DAYOFYEAR";
        public const string T_WEEK = "WEEK";
        public const string T_ACOS = "ACOS";
        public const string T_ASCII = "ASCII";
        public const string T_ASIN = "ASIN";
        public const string T_ATAN = "ATAN";
        public const string T_ATAN2 = "ATAN2";
        public const string T_BITAND = "BITAND";
        public const string T_BITOR = "BITOR";
        public const string T_BITXOR = "BITXOR";
        public const string T_CONCAT_WORD = "CONCAT";
        public const string T_COS = "COS";
        public const string T_COT = "COT";
        public const string T_CRYPT_KEY = "CRYPT_KEY";
        public const string T_CRYPT_IV = "CRYPT_IV";
        public const string T_DATEADD = "DATEADD";
        public const string T_DATEDIFF = "DATEDIFF";
        public const string T_DEGREES = "DEGREES";
        public const string T_DIFFERENCE = "DIFFERENCE";
        public const string T_DMOD = "DMOD";
        public const string T_GC = "GC";
        public const string T_HEXTORAW = "HEXTORAW";
        public const string T_LCASE = "LCASE";
        public const string T_INSTR = "INSTR";
        public const string T_LOG = "LOG";
        public const string T_LOG10 = "LOG10";
        public const string T_LTRIM = "LTRIM";
        public const string T_NOW = "NOW";
        public const string T_PI = "PI";
        public const string T_RADIANS = "RADIANS";
        public const string T_RAND = "RAND";
        public const string T_RAWTOHEX = "RAWTOHEX";
        public const string T_REPLACE = "REPLACE";
        public const string T_REVERSE = "REVERSE";
        public const string T_ROUND = "ROUND";
        public const string T_RTRIM = "RTRIM";
        public const string T_SECONDS_MIDNIGHT = "SECONDS_SINCE_MIDNIGHT";
        public const string T_SIGN = "SIGN";
        public const string T_SIN = "SIN";
        public const string T_SOUNDEX = "SOUNDEX";
        public const string T_SPACE_WORD = "SPACE_WORD";
        public const string T_SUBSTR = "SUBSTR";
        public const string T_SYSDATE = "SYSDATE";
        public const string T_SYSTIMESTAMP = "SYSTIMESTAMP";
        public const string T_TAN = "TAN";
        public const string T_UCASE = "UCASE";
        public const string T_INITCAP = "INITCAP";
        public const string T_TRUNC = "TRUNC";
        public const string T_LEAST = "LEAST";
        public const string T_GREATEST = "GREATEST";
        public const string T_ADD_MONTHS = "ADD_MONTHS";
        public const string T_NEXT_DAY = "NEXT_DAY";
        public const string T_LAST_DAY = "LAST_DAY";
        public const string T_MONTHS_BETWEEN = "MONTHS_BETWEEN";
        public const string T_NEW_TIME = "NEW_TIME";
        public const string T_DECODE = "DECODE";
        public const string T_NUMTOYMINTERVAL = "NUMTOYMINTERVAL";
        public const string T_TO_YMINTERVAL = "TO_YMINTERVAL";
        public const string T_NUMTODSINTERVAL = "NUMTODSINTERVAL";
        public const string T_TO_DSINTERVAL = "TO_DSINTERVAL";
        public const int ABS = 1;
        public const int ALL = 2;
        public const int ALLOCATE = 3;
        public const int ALTER = 4;
        public const int AND = 5;
        public const int ANY = 6;
        public const int ARE = 7;
        public const int ARRAY = 8;
        public const int AS = 9;
        public const int ASENSITIVE = 10;
        public const int ASYMMETRIC = 11;
        public const int AT = 12;
        public const int ATOMIC = 13;
        public const int AUTHORIZATION = 14;
        public const int AVG = 15;
        public const int BEGIN = 0x10;
        public const int BETWEEN = 0x11;
        public const int BIGINT = 0x12;
        public const int BINARY = 0x13;
        public const int BLOB = 20;
        public const int BOOLEAN = 0x15;
        public const int BOTH = 0x16;
        public const int BY = 0x17;
        public const int CALL = 0x18;
        public const int CALLED = 0x19;
        public const int CARDINALITY = 0x1a;
        public const int CASCADED = 0x1b;
        public const int CASE = 0x1c;
        public const int CAST = 0x1d;
        public const int CEIL = 30;
        public const int CEILING = 0x1f;
        public const int CHR = 0x20;
        public const int CHAR_LENGTH = 0x21;
        public const int CHARACTER = 0x22;
        public const int CHARACTER_LENGTH = 0x23;
        public const int CHECK = 0x24;
        public const int CLOB = 0x25;
        public const int CLOSE = 0x26;
        public const int COALESCE = 0x27;
        public const int COLLATE = 40;
        public const int COLLECT = 0x29;
        public const int COLUMN = 0x2a;
        public const int COMMIT = 0x2b;
        public const int COMPARABLE = 0x2c;
        public const int CONDITION = 0x2d;
        public const int CONNECT = 0x2e;
        public const int CONSTRAINT = 0x2f;
        public const int CONVERT = 0x30;
        public const int CORR = 0x31;
        public const int CORRESPONDING = 50;
        public const int COUNT = 0x33;
        public const int COVAR_POP = 0x34;
        public const int COVAR_SAMP = 0x35;
        public const int CREATE = 0x36;
        public const int CROSS = 0x37;
        public const int CUBE = 0x38;
        public const int CUME_DIST = 0x39;
        public const int CURRENT = 0x3a;
        public const int CURRENT_CATALOG = 0x3b;
        public const int CURRENT_DATE = 60;
        public const int CURRENT_DEFAULT_TRANSFORM_GROUP = 0x3d;
        public const int CURRENT_PATH = 0x3e;
        public const int CURRENT_ROLE = 0x3f;
        public const int CURRENT_SCHEMA = 0x40;
        public const int CURRENT_TIME = 0x41;
        public const int CURRENT_TIMESTAMP = 0x42;
        public const int CURRENT_TRANSFORM_GROUP_FOR_TYPE = 0x43;
        public const int CURRENT_USER = 0x44;
        public const int CURSOR = 0x45;
        public const int CYCLE = 70;
        public const int DATE = 0x47;
        public const int DAY = 0x48;
        public const int DEALLOCATE = 0x49;
        public const int DEC = 0x4a;
        public const int DECIMAL = 0x4b;
        public const int DECLARE = 0x4c;
        public const int DEFAULT = 0x4d;
        public const int DELETE = 0x4e;
        public const int DENSE_RANK = 0x4f;
        public const int DEREF = 80;
        public const int DESCRIBE = 0x51;
        public const int DETERMINISTIC = 0x52;
        public const int DISCONNECT = 0x53;
        public const int DISTINCT = 0x54;
        public const int DO = 0x55;
        public const int DOUBLE = 0x56;
        public const int DROP = 0x57;
        public const int DYNAMIC = 0x58;
        public const int EACH = 0x59;
        public const int ELEMENT = 90;
        public const int ELSE = 0x5b;
        public const int ELSEIF = 0x5c;
        public const int END = 0x5d;
        public const int END_EXEC = 0x5e;
        public const int ESCAPE = 0x5f;
        public const int EVERY = 0x60;
        public const int EXCEPT = 0x61;
        public const int EXEC = 0x62;
        public const int EXECUTE = 0x63;
        public const int EXISTS = 100;
        public const int EXIT = 0x65;
        public const int EXP = 0x66;
        public const int EXTERNAL = 0x67;
        public const int EXTRACT = 0x68;
        public const int FALSE = 0x69;
        public const int FETCH = 0x6a;
        public const int FILTER = 0x6b;
        public const int FIRST_VALUE = 0x6c;
        public const int FLOAT = 0x6d;
        public const int FLOOR = 110;
        public const int FOR = 0x6f;
        public const int FOREIGN = 0x70;
        public const int FREE = 0x71;
        public const int FROM = 0x72;
        public const int FULL = 0x73;
        public const int FUNCTION = 0x74;
        public const int FUSION = 0x75;
        public const int GET = 0x76;
        public const int GLOBAL = 0x77;
        public const int GRANT = 120;
        public const int GROUP = 0x79;
        public const int GROUPING = 0x7a;
        public const int HANDLER = 0x7b;
        public const int HAVING = 0x7c;
        public const int HOLD = 0x7d;
        public const int HOUR = 0x7e;
        public const int IDENTITY = 0x7f;
        public const int IN = 0x80;
        public const int INDICATOR = 0x81;
        public const int INNER = 130;
        public const int INOUT = 0x83;
        public const int INSENSITIVE = 0x84;
        public const int INSERT = 0x85;
        public const int INT = 0x86;
        public const int INTEGER = 0x87;
        public const int INTERSECT = 0x88;
        public const int INTERSECTION = 0x89;
        public const int INTERVAL = 0x8a;
        public const int INTO = 0x8b;
        public const int IS = 140;
        public const int ITERATE = 0x8d;
        public const int JOIN = 0x8e;
        public const int LAG = 0x8f;
        public const int LANGUAGE = 0x90;
        public const int LARGE = 0x91;
        public const int LAST_VALUE = 0x92;
        public const int LATERAL = 0x93;
        public const int LEAD = 0x94;
        public const int LEADING = 0x95;
        public const int LEAVE = 150;
        public const int LEFT = 0x97;
        public const int LIKE = 0x98;
        public const int REGEXP_LIKE = 0x99;
        public const int LN = 0x9a;
        public const int LOCAL = 0x9b;
        public const int LOCALTIME = 0x9c;
        public const int LOCALTIMESTAMP = 0x9d;
        public const int LOOP = 0x9e;
        public const int LOWER = 0x9f;
        public const int MATCH = 160;
        public const int MAX = 0xa1;
        public const int MAX_CARDINALITY = 0xa2;
        public const int MEMBER = 0xa3;
        public const int MERGE = 0xa4;
        public const int METHOD = 0xa5;
        public const int MIN = 0xa6;
        public const int MINUTE = 0xa7;
        public const int MOD = 0xa8;
        public const int MODIFIES = 0xa9;
        public const int MODULE = 170;
        public const int MONTH = 0xab;
        public const int MULTISET = 0xac;
        public const int NATIONAL = 0xad;
        public const int NATURAL = 0xae;
        public const int NCHAR = 0xaf;
        public const int NCLOB = 0xb0;
        public const int NEW = 0xb1;
        public const int NO = 0xb2;
        public const int NONE = 0xb3;
        public const int NORMALIZE = 180;
        public const int NOT = 0xb5;
        public const int NTH_VALUE = 0xb6;
        public const int NTILE = 0xb7;
        public const int NULL = 0xb8;
        public const int NULLIF = 0xb9;
        public const int NUMERIC = 0xba;
        public const int OCCURRENCES_REGEX = 0xbb;
        public const int OCTET_LENGTH = 0xbc;
        public const int OF = 0xbd;
        public const int OFFSET = 190;
        public const int OLD = 0xbf;
        public const int ON = 0xc0;
        public const int ONLY = 0xc1;
        public const int OPEN = 0xc2;
        public const int OR = 0xc3;
        public const int ORDER = 0xc4;
        public const int OUT = 0xc5;
        public const int OUTER = 0xc6;
        public const int OVER = 0xc7;
        public const int OVERLAPS = 200;
        public const int OVERLAY = 0xc9;
        public const int PARAMETER = 0xca;
        public const int PARTITION = 0xcb;
        public const int PERCENT_RANK = 0xcc;
        public const int PERCENTILE_CONT = 0xcd;
        public const int PERCENTILE_DISC = 0xce;
        public const int POSITION = 0xcf;
        public const int REGEXP_REPLACE = 0xd0;
        public const int POWER = 0xd1;
        public const int PRECISION = 210;
        public const int PREPARE = 0xd3;
        public const int PRIMARY = 0xd4;
        public const int PROCEDURE = 0xd5;
        public const int RANGE = 0xd6;
        public const int RANK = 0xd7;
        public const int READS = 0xd8;
        public const int REAL = 0xd9;
        public const int RECURSIVE = 0xda;
        public const int REF = 0xdb;
        public const int REFERENCES = 220;
        public const int REFERENCING = 0xdd;
        public const int REGR_AVGX = 0xde;
        public const int REGR_AVGY = 0xdf;
        public const int REGR_COUNT = 0xe0;
        public const int REGR_INTERCEPT = 0xe1;
        public const int REGR_R2 = 0xe2;
        public const int REGR_SLOPE = 0xe3;
        public const int REGR_SXX = 0xe4;
        public const int REGR_SXY = 0xe5;
        public const int REGR_SYY = 230;
        public const int RELEASE = 0xe7;
        public const int REPEAT = 0xe8;
        public const int RESIGNAL = 0xe9;
        public const int RESULT = 0xea;
        public const int RETURN = 0xeb;
        public const int RETURNS = 0xec;
        public const int REVOKE = 0xed;
        public const int RIGHT = 0xee;
        public const int ROLLBACK = 0xef;
        public const int ROLLUP = 240;
        public const int ROW = 0xf1;
        public const int ROW_NUMBER = 0xf2;
        public const int ROWS = 0xf3;
        public const int SAVEPOINT = 0xf4;
        public const int SCOPE = 0xf5;
        public const int SCROLL = 0xf6;
        public const int SEARCH = 0xf7;
        public const int SECOND = 0xf8;
        public const int SELECT = 0xf9;
        public const int SENSITIVE = 250;
        public const int SESSION_USER = 0xfb;
        public const int SET = 0xfc;
        public const int SIGNAL = 0xfd;
        public const int SIMILAR = 0xfe;
        public const int SMALLINT = 0xff;
        public const int SOME = 0x100;
        public const int SPECIFIC = 0x101;
        public const int SPECIFICTYPE = 0x102;
        public const int SQL = 0x103;
        public const int SQLEXCEPTION = 260;
        public const int SQLSTATE = 0x105;
        public const int SQLWARNING = 0x106;
        public const int SQRT = 0x107;
        public const int STACKED = 0x108;
        public const int START = 0x109;
        public const int STATIC = 0x10a;
        public const int STDDEV_POP = 0x10b;
        public const int STDDEV_SAMP = 0x10c;
        public const int SUBMULTISET = 0x10d;
        public const int SUBSTRING = 270;
        public const int REGEXP_SUBSTR = 0x10f;
        public const int SUM = 0x110;
        public const int SYMMETRIC = 0x111;
        public const int SYSTEM = 0x112;
        public const int SYSTEM_USER = 0x113;
        public const int TABLE = 0x114;
        public const int TABLESAMPLE = 0x115;
        public const int THEN = 0x116;
        public const int TIME = 0x117;
        public const int TIMESTAMP = 280;
        public const int TIMEZONE_HOUR = 0x119;
        public const int TIMEZONE_MINUTE = 0x11a;
        public const int TO = 0x11b;
        public const int TRAILING = 0x11c;
        public const int TRANSLATE = 0x11d;
        public const int REGEXP_INSTR = 0x11e;
        public const int TRANSLATION = 0x11f;
        public const int TREAT = 0x120;
        public const int TRIGGER = 0x121;
        public const int TRIM = 290;
        public const int TRIM_ARRAY = 0x123;
        public const int TRUE = 0x124;
        public const int TRUNCATE = 0x125;
        public const int UESCAPE = 0x126;
        public const int UNDO = 0x127;
        public const int UNION = 0x128;
        public const int UNIQUE = 0x129;
        public const int UNKNOWN = 0x12a;
        public const int UNNEST = 0x12b;
        public const int UNTIL = 300;
        public const int UPDATE = 0x12d;
        public const int UPPER = 0x12e;
        public const int USER = 0x12f;
        public const int USING = 0x130;
        public const int VALUE = 0x131;
        public const int VALUES = 0x132;
        public const int VAR_POP = 0x133;
        public const int VAR_SAMP = 0x134;
        public const int VARBINARY = 0x135;
        public const int VARCHAR = 310;
        public const int VARYING = 0x137;
        public const int WHEN = 0x138;
        public const int WHENEVER = 0x139;
        public const int WHERE = 0x13a;
        public const int WIDTH_BUCKET = 0x13b;
        public const int WINDOW = 0x13c;
        public const int WITH = 0x13d;
        public const int WITHIN = 0x13e;
        public const int WITHOUT = 0x13f;
        public const int WHILE = 320;
        public const int YEAR = 0x141;
        public const int LPAD = 0x142;
        public const int RPAD = 0x143;
        public const int A = 330;
        public const int ABSOLUTE = 0x14b;
        public const int ACTION = 0x14c;
        public const int ADA = 0x14d;
        public const int ADD = 0x14e;
        public const int ADMIN = 0x14f;
        public const int AFTER = 0x150;
        public const int ALWAYS = 0x151;
        public const int ASC = 0x152;
        public const int ASSERTION = 0x153;
        public const int ASSIGNMENT = 340;
        public const int ATTRIBUTE = 0x155;
        public const int ATTRIBUTES = 0x156;
        public const int BEFORE = 0x157;
        public const int BERNOULLI = 0x158;
        public const int BREADTH = 0x159;
        public const int C = 0x15a;
        public const int CASCADE = 0x15b;
        public const int CATALOG = 0x15c;
        public const int CATALOG_NAME = 0x15d;
        public const int CHAIN = 350;
        public const int CHARACTER_SET_CATALOG = 0x15f;
        public const int CHARACTER_SET_NAME = 0x160;
        public const int CHARACTER_SET_SCHEMA = 0x161;
        public const int CHARACTERISTICS = 0x162;
        public const int CHARACTERS = 0x163;
        public const int CLASS_ORIGIN = 0x164;
        public const int COBOL = 0x165;
        public const int COLLATION = 0x166;
        public const int COLLATION_CATALOG = 0x167;
        public const int COLLATION_NAME = 360;
        public const int COLLATION_SCHEMA = 0x169;
        public const int COLUMN_NAME = 0x16a;
        public const int COMMAND_FUNCTION = 0x16b;
        public const int COMMAND_FUNCTION_CODE = 0x16c;
        public const int COMMITTED = 0x16d;
        public const int CONDITION_IDENTIFIER = 0x16e;
        public const int CONDITION_NUMBER = 0x16f;
        public const int CONNECTION = 0x170;
        public const int CONNECTION_NAME = 0x171;
        public const int CONSTRAINT_CATALOG = 370;
        public const int CONSTRAINT_NAME = 0x173;
        public const int CONSTRAINT_SCHEMA = 0x174;
        public const int CONSTRAINTS = 0x175;
        public const int CONSTRUCTOR = 0x176;
        public const int CONTAINS = 0x177;
        public const int CONTINUE = 0x178;
        public const int CURSOR_NAME = 0x179;
        public const int DATA = 0x17a;
        public const int DATETIME_INTERVAL_CODE = 0x17b;
        public const int DATETIME_INTERVAL_PRECISION = 380;
        public const int DEFAULTS = 0x17d;
        public const int DEFERRABLE = 0x17e;
        public const int DEFERRED = 0x17f;
        public const int DEFINED = 0x180;
        public const int DEFINER = 0x181;
        public const int DEGREE = 0x182;
        public const int DEPTH = 0x183;
        public const int DERIVED = 0x184;
        public const int DESC = 0x185;
        public const int DESCRIPTOR = 390;
        public const int DIAGNOSTICS = 0x187;
        public const int DISPATCH = 0x188;
        public const int DOMAIN = 0x189;
        public const int DYNAMIC_FUNCTION = 0x18a;
        public const int DYNAMIC_FUNCTION_CODE = 0x18b;
        public const int EQUALS = 0x18c;
        public const int EXCEPTION = 0x18d;
        public const int EXCLUDE = 0x18e;
        public const int EXCLUDING = 0x18f;
        public const int FINAL = 400;
        public const int FIRST = 0x191;
        public const int FOLLOWING = 0x192;
        public const int FORTRAN = 0x193;
        public const int FOUND = 0x194;
        public const int G = 0x195;
        public const int GENERAL = 0x196;
        public const int GENERATED = 0x197;
        public const int GO = 0x198;
        public const int GOTO = 0x199;
        public const int GRANTED = 410;
        public const int HIERARCHY = 0x19b;
        public const int IF = 0x19c;
        public const int IGNORE = 0x19d;
        public const int IMMEDIATE = 0x19e;
        public const int IMPLEMENTATION = 0x19f;
        public const int INCLUDING = 0x1a0;
        public const int INCREMENT = 0x1a1;
        public const int INITIALLY = 0x1a2;
        public const int INPUT = 0x1a3;
        public const int INSTANCE = 420;
        public const int INSTANTIABLE = 0x1a5;
        public const int INSTEAD = 0x1a6;
        public const int INVOKER = 0x1a7;
        public const int ISOLATION = 0x1a8;
        public const int DOTNET = 0x1a9;
        public const int K = 0x1aa;
        public const int KEY = 0x1ab;
        public const int KEY_MEMBER = 0x1ac;
        public const int KEY_TYPE = 0x1ad;
        public const int LAST = 430;
        public const int LENGTH = 0x1af;
        public const int LEVEL = 0x1b0;
        public const int LOCATOR = 0x1b1;
        public const int M = 0x1b2;
        public const int MAP = 0x1b3;
        public const int MATCHED = 0x1b4;
        public const int MAXVALUE = 0x1b5;
        public const int MESSAGE_LENGTH = 0x1b6;
        public const int MESSAGE_OCTET_LENGTH = 0x1b7;
        public const int MESSAGE_TEXT = 440;
        public const int MINVALUE = 0x1b9;
        public const int MORE = 0x1ba;
        public const int MUMPS = 0x1bb;
        public const int NAME = 0x1bc;
        public const int NAMES = 0x1bd;
        public const int NESTING = 0x1be;
        public const int NEXT = 0x1bf;
        public const int NORMALIZED = 0x1c0;
        public const int NULLABLE = 0x1c1;
        public const int NULLS = 450;
        public const int NUMBER = 0x1c3;
        public const int OBJECT = 0x1c4;
        public const int OCTETS = 0x1c5;
        public const int OPTION = 0x1c6;
        public const int OPTIONS = 0x1c7;
        public const int ORDERING = 0x1c8;
        public const int ORDINALITY = 0x1c9;
        public const int OTHERS = 0x1ca;
        public const int OUTPUT = 0x1cb;
        public const int OVERRIDING = 460;
        public const int P = 0x1cd;
        public const int PAD = 0x1ce;
        public const int PARAMETER_MODE = 0x1cf;
        public const int PARAMETER_NAME = 0x1d0;
        public const int PARAMETER_ORDINAL_POSITION = 0x1d1;
        public const int PARAMETER_SPECIFIC_CATALOG = 0x1d2;
        public const int PARAMETER_SPECIFIC_NAME = 0x1d3;
        public const int PARAMETER_SPECIFIC_SCHEMA = 0x1d4;
        public const int PARTIAL = 0x1d5;
        public const int PASCAL = 470;
        public const int PATH = 0x1d7;
        public const int PLACING = 0x1d8;
        public const int PLI = 0x1d9;
        public const int PRECEDING = 0x1da;
        public const int PRESERVE = 0x1db;
        public const int PRIOR = 0x1dc;
        public const int PRIVILEGES = 0x1dd;
        public const int PUBLIC = 0x1de;
        public const int READ = 0x1df;
        public const int RELATIVE = 480;
        public const int REPEATABLE = 0x1e1;
        public const int RESPECT = 0x1e2;
        public const int RESTART = 0x1e3;
        public const int RESTRICT = 0x1e4;
        public const int RETURNED_CARDINALITY = 0x1e5;
        public const int RETURNED_LENGTH = 0x1e6;
        public const int RETURNED_OCTET_LENGTH = 0x1e7;
        public const int RETURNED_SQLSTATE = 0x1e8;
        public const int ROLE = 0x1e9;
        public const int ROUTINE = 490;
        public const int ROUTINE_CATALOG = 0x1eb;
        public const int ROUTINE_NAME = 0x1ec;
        public const int ROUTINE_SCHEMA = 0x1ed;
        public const int ROW_COUNT = 0x1ee;
        public const int SCALE = 0x1ef;
        public const int SCHEMA = 0x1f0;
        public const int SCHEMA_NAME = 0x1f1;
        public const int SCOPE_CATALOG = 0x1f2;
        public const int SCOPE_NAME = 0x1f3;
        public const int SCOPE_SCHEMA = 500;
        public const int SECTION = 0x1f5;
        public const int SECURITY = 0x1f6;
        public const int SELF = 0x1f7;
        public const int SEQUENCE = 0x1f8;
        public const int SERIALIZABLE = 0x1f9;
        public const int SERVER_NAME = 0x1fa;
        public const int SESSION = 0x1fb;
        public const int SETS = 0x1fc;
        public const int SIMPLE = 0x1fd;
        public const int SIZE = 510;
        public const int SOURCE = 0x1ff;
        public const int SPACE = 0x200;
        public const int SPECIFIC_NAME = 0x201;
        public const int STATE = 0x202;
        public const int STATEMENT = 0x203;
        public const int STRUCTURE = 0x204;
        public const int STYLE = 0x205;
        public const int SUBCLASS_ORIGIN = 0x206;
        public const int T = 0x207;
        public const int TABLE_NAME = 520;
        public const int TEMPORARY = 0x209;
        public const int TIES = 0x20a;
        public const int TOP_LEVEL_COUNT = 0x20b;
        public const int TRANSACTION = 0x20c;
        public const int TRANSACTION_ACTIVE = 0x20d;
        public const int TRANSACTIONS_COMMITTED = 0x20e;
        public const int TRANSACTIONS_ROLLED_BACK = 0x20f;
        public const int TRANSFORM = 0x210;
        public const int TRANSFORMS = 0x211;
        public const int TRIGGER_CATALOG = 530;
        public const int TRIGGER_NAME = 0x213;
        public const int TRIGGER_SCHEMA = 0x214;
        public const int TYPE = 0x215;
        public const int UNBOUNDED = 0x216;
        public const int UNCOMMITTED = 0x217;
        public const int UNDER = 0x218;
        public const int UNNAMED = 0x219;
        public const int USAGE = 0x21a;
        public const int USER_DEFINED_TYPE_CATALOG = 0x21b;
        public const int USER_DEFINED_TYPE_CODE = 540;
        public const int USER_DEFINED_TYPE_NAME = 0x21d;
        public const int USER_DEFINED_TYPE_SCHEMA = 0x21e;
        public const int VIEW = 0x21f;
        public const int WORK = 0x220;
        public const int WRITE = 0x221;
        public const int ZONE = 0x222;
        public const int ASSIGN = 0x223;
        public const int ALIAS = 0x227;
        public const int AUTOCOMMIT = 0x228;
        public const int BACKUP = 0x229;
        public const int BIT = 0x22a;
        public const int BLOCKING = 0x22b;
        public const int CACHE = 0x22c;
        public const int CACHED = 0x22d;
        public const int CASEWHEN = 0x22e;
        public const int CHECKPOINT = 0x22f;
        public const int CLASS = 560;
        public const int COMPACT = 0x231;
        public const int COMPRESSED = 0x232;
        public const int CONTROL = 0x233;
        public const int DATABASE = 0x234;
        public const int DEFRAG = 0x235;
        public const int DELAY = 0x236;
        public const int EVENT = 0x237;
        public const int EXPLAIN = 0x238;
        public const int FILE = 0x239;
        public const int FILES = 570;
        public const int GC = 0x23b;
        public const int HEADER = 0x23c;
        public const int IGNORECASE = 0x23d;
        public const int IMMEDIATELY = 0x23e;
        public const int INTEGRITY = 0x23f;
        public const int INDEX = 0x240;
        public const int INITIAL = 0x241;
        public const int LIMIT = 0x242;
        public const int LOCK = 0x243;
        public const int LOCKS = 580;
        public const int MAXROWS = 0x245;
        public const int MEMORY = 0x246;
        public const int MILLIS = 0x247;
        public const int MINUS_EXCEPT = 0x248;
        public const int OFF = 0x249;
        public const int PASSWORD = 0x24a;
        public const int PLAN = 0x24b;
        public const int PROPERTY = 0x24c;
        public const int READONLY = 0x24d;
        public const int REFERENTIAL = 590;
        public const int RENAME = 0x24f;
        public const int SCRIPT = 0x250;
        public const int SCRIPTFORMAT = 0x251;
        public const int SHUTDOWN = 0x252;
        public const int TEMP = 0x253;
        public const int TEXT = 0x254;
        public const int WRITE_DELAY = 0x255;
        public const int IDENTIFIED = 0x256;
        public const int ACOS = 0x259;
        public const int ASCII = 0x25a;
        public const int ASIN = 0x25b;
        public const int ATAN = 0x25c;
        public const int ATAN2 = 0x25d;
        public const int BIT_LENGTH = 0x25e;
        public const int BITAND = 0x25f;
        public const int BITLENGTH = 0x260;
        public const int BITOR = 0x261;
        public const int BITXOR = 610;
        public const int CONCAT_WORD = 0x263;
        public const int COS = 0x264;
        public const int COT = 0x265;
        public const int CRYPT_KEY = 0x266;
        public const int CURDATE = 0x267;
        public const int CURTIME = 0x268;
        public const int DATEADD = 0x269;
        public const int DATEDIFF = 0x26a;
        public const int DAY_NAME = 0x26b;
        public const int DAY_OF_MONTH = 620;
        public const int DAY_OF_WEEK = 0x26d;
        public const int DAY_OF_YEAR = 0x26e;
        public const int DAYNAME = 0x26f;
        public const int DAYOFMONTH = 0x270;
        public const int DAYOFWEEK = 0x271;
        public const int DAYOFYEAR = 0x272;
        public const int DEGREES = 0x273;
        public const int DIFFERENCE = 0x274;
        public const int DMOD = 0x275;
        public const int HEXTORAW = 630;
        public const int IFNULL = 0x277;
        public const int ISAUTOCOMMIT = 0x278;
        public const int ISREADONLYDATABASE = 0x279;
        public const int ISREADONLYDATABASEFILES = 0x27a;
        public const int ISREADONLYSESSION = 0x27b;
        public const int LCASE = 0x27c;
        public const int INSTR = 0x27d;
        public const int LOG = 0x27e;
        public const int LOG10 = 0x27f;
        public const int LTRIM = 640;
        public const int MONTH_NAME = 0x281;
        public const int MONTHNAME = 0x282;
        public const int MVCC = 0x283;
        public const int MVLOCKS = 0x284;
        public const int NIO = 0x285;
        public const int NOW = 0x286;
        public const int OCTETLENGTH = 0x287;
        public const int PI = 0x288;
        public const int QUARTER = 0x289;
        public const int RADIANS = 650;
        public const int RAND = 0x28b;
        public const int RAWTOHEX = 0x28c;
        public const int REPLACE = 0x28d;
        public const int REVERSE = 0x28e;
        public const int ROUND = 0x28f;
        public const int RTRIM = 0x291;
        public const int SECONDS_MIDNIGHT = 0x292;
        public const int SIGN = 0x293;
        public const int SIN = 660;
        public const int SOUNDEX = 0x295;
        public const int SPACE_WORD = 0x296;
        public const int SUBSTR = 0x297;
        public const int SYSDATE = 0x298;
        public const int TAN = 0x299;
        public const int TIMESTAMPADD = 0x29a;
        public const int TIMESTAMPDIFF = 0x29b;
        public const int TO_CHAR = 0x29c;
        public const int TODAY = 0x29d;
        public const int TOP = 670;
        public const int UCASE = 0x29f;
        public const int WEEK = 0x2a0;
        public const int WEEK_OF_YEAR = 0x2a1;
        public const int TO_DATE = 0x2a2;
        public const int INITCAP = 0x2a3;
        public const int TRUNC = 0x2a4;
        public const int GREATEST = 0x2a5;
        public const int LEAST = 0x2a6;
        public const int ADD_MONTHS = 0x2a7;
        public const int NEXT_DAY = 0x325;
        public const int LAST_DAY = 0x326;
        public const int MONTHS_BETWEEN = 0x327;
        public const int NEW_TIME = 0x328;
        public const int DECODE = 0x329;
        public const int NUMTOYMINTERVAL = 810;
        public const int TO_YMINTERVAL = 0x32b;
        public const int NUMTODSINTERVAL = 0x32c;
        public const int TO_DSINTERVAL = 0x32d;
        public const int LOB = 0x32e;
        public const int CRYPT_IV = 0x32f;
        public const int PERCENT = 0x330;
        public const int LibCore_ERRNO = 0x331;
        public const int AGGREGATE = 0x332;
        public const int RIGHTBRACKET = 0x333;
        public const int LEFTBRACKET = 820;
        public const int COMMENT = 0x335;
        public const int SYSTIMESTAMP = 0x336;
        public const int CHAR = 0x337;
        public const int BYTE = 0x338;
        public const int NVL2 = 0x339;
        public const int NEWID = 0x33a;
        public const int TRANSACTION_ID = 0x33b;
        public const int BODY = 0x33c;
        public const int ASTERISK = 0x2a9;
        public const int CLOSEBRACKET = 0x2aa;
        public const int COLON = 0x2ab;
        public const int COMMA = 0x2ac;
        public const int CONCAT = 0x2ad;
        public const int DIVIDE = 0x2ae;
        public const int DOUBLE_COLON_OP = 0x2af;
        public const int DOUBLE_PERIOD_OP = 0x2b0;
        public const int GREATER = 680;
        public const int GREATER_EQUALS = 690;
        public const int LESS = 0x2b3;
        public const int LESS_EQUALS = 0x2b4;
        public const int MINUS = 0x2b5;
        public const int NOT_EQUALS = 0x2b6;
        public const int OPENBRACKET = 0x2b7;
        public const int PLUS = 0x2b8;
        public const int QUESTION = 0x2b9;
        public const int RIGHT_ARROW_OP = 0x2ba;
        public const int SEMICOLON = 0x2bb;
        public const int ATSYM = 700;
        public const int DOUBLE_GREATER = 800;
        public const int DOUBLE_LESS = 0x321;
        public const int DOT_DOT = 0x322;
        public const int UNIQUEIDENTIFIER = 0x323;
        public const int NOCACHE = 0x324;
        public const int SQL_BIGINT = 0x2bd;
        public const int SQL_BINARY = 0x2be;
        public const int SQL_BIT = 0x2bf;
        public const int SQL_BLOB = 0x2c0;
        public const int SQL_BOOLEAN = 0x2c1;
        public const int SQL_CHAR = 0x2c2;
        public const int SQL_CLOB = 0x2c3;
        public const int SQL_DATE = 0x2c4;
        public const int SQL_DECIMAL = 0x2c5;
        public const int SQL_DATALINK = 710;
        public const int SQL_DOUBLE = 0x2c7;
        public const int SQL_FLOAT = 0x2c8;
        public const int SQL_INTEGER = 0x2c9;
        public const int SQL_LONGVARBINARY = 0x2ca;
        public const int SQL_LONGNVARCHAR = 0x2cb;
        public const int SQL_LONGVARCHAR = 0x2cc;
        public const int SQL_NCHAR = 0x2cd;
        public const int SQL_NCLOB = 0x2ce;
        public const int SQL_NUMERIC = 0x2cf;
        public const int SQL_NVARCHAR = 720;
        public const int SQL_REAL = 0x2d1;
        public const int SQL_ROWID = 0x2d2;
        public const int SQL_SQLXML = 0x2d3;
        public const int SQL_SMALLINT = 0x2d4;
        public const int SQL_TIME = 0x2d5;
        public const int SQL_TIMESTAMP = 0x2d6;
        public const int SQL_TINYINT = 0x2d7;
        public const int SQL_VARBINARY = 0x2d8;
        public const int SQL_VARCHAR = 0x2d9;
        public const int SQL_TSI_FRAC_SECOND = 0x2db;
        public const int SQL_TSI_SECOND = 0x2dc;
        public const int SQL_TSI_MINUTE = 0x2dd;
        public const int SQL_TSI_HOUR = 0x2de;
        public const int SQL_TSI_DAY = 0x2df;
        public const int SQL_TSI_WEEK = 0x2e0;
        public const int SQL_TSI_MONTH = 0x2e1;
        public const int SQL_TSI_QUARTER = 0x2e2;
        public const int SQL_TSI_YEAR = 0x2e3;
        public const int X_MANY = 740;
        public const int X_KEYSET = 0x2e5;
        public const int X_OPTION = 0x2e6;
        public const int X_REPEAT = 0x2e7;
        public const int X_POS_INTEGER = 0x2e8;
        public const int X_VALUE = 0x2e9;
        public const int X_IDENTIFIER = 0x2ea;
        public const int X_DELIMITED_IDENTIFIER = 0x2eb;
        public const int X_ENDPARSE = 0x2ec;
        public const int X_STARTPARSE = 0x2ed;
        public const int X_REMARK = 750;
        public const int X_NULL = 0x2ef;
        public const int X_LOB_SIZE = 0x2f0;
        public const int X_MALFORMED_STRING = 0x2f1;
        public const int X_MALFORMED_NUMERIC = 0x2f2;
        public const int X_MALFORMED_BIT_STRING = 0x2f3;
        public const int X_MALFORMED_BINARY_STRING = 0x2f4;
        public const int X_MALFORMED_UNICODE_STRING = 0x2f5;
        public const int X_MALFORMED_COMMENT = 0x2f6;
        public const int X_MALFORMED_IDENTIFIER = 0x2f7;
        public const int X_MALFORMED_UNICODE_ESCAPE = 760;
        public const int X_UNKNOWN_TOKEN = -1;
        private static Dictionary<string, int> reservedKeys = GetReservedKeys();
        private static Dictionary<string, int> commandSet = GetCommandSet();
        private static HashSet<short> coreReservedWords = GetCoreReservedWords();
        public static readonly short[] SQL_INTERVAL_FIELD_CODES = new short[] { 0x141, 0xab, 0x48, 0x7e, 0xa7, 0xf8 };
        public static readonly string[] SQL_INTERVAL_FIELD_NAMES = new string[] { "YEAR", "MONTH", "DAY", "HOUR", "MINUTE", "SECOND" };

        private static Dictionary<string, int> GetCommandSet()
        {
            return new Dictionary<string, int>(0xfb) { 
                { 
                    "ACTION",
                    0x14c
                },
                { 
                    "ADD",
                    0x14e
                },
                { 
                    "ADMIN",
                    0x14f
                },
                { 
                    "AFTER",
                    0x150
                },
                { 
                    "ALIAS",
                    0x227
                },
                { 
                    "ALWAYS",
                    0x151
                },
                { 
                    "ASC",
                    0x152
                },
                { 
                    "AUTOCOMMIT",
                    0x228
                },
                { 
                    "BACKUP",
                    0x229
                },
                { 
                    "BEFORE",
                    0x157
                },
                { 
                    "BIT",
                    0x15
                },
                { 
                    "BLOCKING",
                    0x22b
                },
                { 
                    "CACHE",
                    0x22c
                },
                { 
                    "CACHED",
                    0x22d
                },
                { 
                    "CHAIN",
                    350
                },
                { 
                    "CASCADE",
                    0x15b
                },
                { 
                    "CATALOG",
                    0x15c
                },
                { 
                    "CATALOG_NAME",
                    0x15d
                },
                { 
                    "CHARACTERISTICS",
                    0x162
                },
                { 
                    "CHECKPOINT",
                    0x22f
                },
                { 
                    "CLASS_ORIGIN",
                    0x164
                },
                { 
                    "CRYPT_KEY",
                    0x266
                },
                { 
                    "CRYPT_IV",
                    0x32f
                },
                { 
                    "CLASS",
                    560
                },
                { 
                    "COLLATE",
                    40
                },
                { 
                    "COLLATION",
                    0x166
                },
                { 
                    "COMMENT",
                    0x335
                },
                { 
                    "COLUMN_NAME",
                    0x16a
                },
                { 
                    "COMMITTED",
                    0x16d
                },
                { 
                    "COMPACT",
                    0x231
                },
                { 
                    "COMPRESSED",
                    0x232
                },
                { 
                    "CONDIITON_IDENTIFIER",
                    0x16e
                },
                { 
                    "CONSTRAINT_CATALOG",
                    370
                },
                { 
                    "CONSTRAINT_SCHEMA",
                    0x174
                },
                { 
                    "CONSTRAINT_NAME",
                    0x173
                },
                { 
                    "CONTAINS",
                    0x177
                },
                { 
                    "CONTINUE",
                    0x178
                },
                { 
                    "CONTROL",
                    0x233
                },
                { 
                    "CURDATE",
                    0x267
                },
                { 
                    "CURTIME",
                    0x268
                },
                { 
                    "CURSOR_NAME",
                    0x179
                },
                { 
                    "DATA",
                    0x17a
                },
                { 
                    "DATABASE",
                    0x234
                },
                { 
                    "DEFAULTS",
                    0x17d
                },
                { 
                    "DEFRAG",
                    0x235
                },
                { 
                    "DELAY",
                    0x236
                },
                { 
                    "DESC",
                    0x185
                },
                { 
                    "DOMAIN",
                    0x189
                },
                { 
                    "LibCore_ERRNO",
                    0x331
                },
                { 
                    "EVENT",
                    0x237
                },
                { 
                    "EXCLUDING",
                    0x18f
                },
                { 
                    "EXPLAIN",
                    0x238
                },
                { 
                    "FILE",
                    0x239
                },
                { 
                    "FILES",
                    570
                },
                { 
                    "FINAL",
                    400
                },
                { 
                    "FIRST",
                    0x191
                },
                { 
                    "FOUND",
                    0x194
                },
                { 
                    "G",
                    0x195
                },
                { 
                    "GC",
                    0x23b
                },
                { 
                    "GENERATED",
                    0x197
                },
                { 
                    "GRANTED",
                    410
                },
                { 
                    "HEADER",
                    0x23c
                },
                { 
                    "IF",
                    0x19c
                },
                { 
                    "IGNORECASE",
                    0x23d
                },
                { 
                    "IMMEDIATELY",
                    0x23e
                },
                { 
                    "IMMEDIATE",
                    0x19e
                },
                { 
                    "INCLUDING",
                    0x1a0
                },
                { 
                    "INCREMENT",
                    0x1a1
                },
                { 
                    "INDEX",
                    0x240
                },
                { 
                    "INITIAL",
                    0x241
                },
                { 
                    "INPUT",
                    0x1a3
                },
                { 
                    "INSTEAD",
                    0x1a6
                },
                { 
                    "INTEGRITY",
                    0x23f
                },
                { 
                    "ISAUTOCOMMIT",
                    0x278
                },
                { 
                    "ISOLATION",
                    0x1a8
                },
                { 
                    "ISREADONLYDATABASE",
                    0x279
                },
                { 
                    "ISREADONLYDATABASEFILES",
                    0x27a
                },
                { 
                    "ISREADONLYSESSION",
                    0x27b
                },
                { 
                    "DOTNET",
                    0x1a9
                },
                { 
                    "K",
                    0x1aa
                },
                { 
                    "KEY",
                    0x1ab
                },
                { 
                    "LAST",
                    430
                },
                { 
                    "LENGTH",
                    0x1af
                },
                { 
                    "LEVEL",
                    0x1b0
                },
                { 
                    "LIMIT",
                    0x242
                },
                { 
                    "LOB",
                    0x32e
                },
                { 
                    "LOCKS",
                    580
                },
                { 
                    "M",
                    0x1b2
                },
                { 
                    "MATCHED",
                    0x1b4
                },
                { 
                    "MAXROWS",
                    0x245
                },
                { 
                    "MAXVALUE",
                    0x1b5
                },
                { 
                    "MESSAGE_TEXT",
                    440
                },
                { 
                    "MEMORY",
                    0x246
                },
                { 
                    "MILLIS",
                    0x247
                },
                { 
                    "MINVALUE",
                    0x1b9
                },
                { 
                    "MVCC",
                    0x283
                },
                { 
                    "MVLOCKS",
                    0x284
                },
                { 
                    "NAME",
                    0x1bc
                },
                { 
                    "NEXT",
                    0x1bf
                },
                { 
                    "NIO",
                    0x285
                },
                { 
                    "NOCACHE",
                    0x324
                },
                { 
                    "NOW",
                    0x286
                },
                { 
                    "NULLS",
                    450
                },
                { 
                    "OFF",
                    0x249
                },
                { 
                    "OPTION",
                    0x1c6
                },
                { 
                    "ORDINALITY",
                    0x1c9
                },
                { 
                    "OVERRIDING",
                    460
                },
                { 
                    "P",
                    0x1cd
                },
                { 
                    "PARTIAL",
                    0x1d5
                },
                { 
                    "PASSWORD",
                    0x24a
                },
                { 
                    "IDENTIFIED",
                    0x256
                },
                { 
                    "PLACING",
                    0x1d8
                },
                { 
                    "PLAN",
                    0x24b
                },
                { 
                    "PRESERVE",
                    0x1db
                },
                { 
                    "PRIVILEGES",
                    0x1dd
                },
                { 
                    "PROPERTY",
                    0x24c
                },
                { 
                    "READ",
                    0x1df
                },
                { 
                    "READONLY",
                    0x24d
                },
                { 
                    "REFERENTIAL",
                    590
                },
                { 
                    "RENAME",
                    0x24f
                },
                { 
                    "REPEATABLE",
                    0x1e1
                },
                { 
                    "RESTART",
                    0x1e3
                },
                { 
                    "RESTRICT",
                    0x1e4
                },
                { 
                    "RESULT",
                    0xea
                },
                { 
                    "ROLE",
                    0x1e9
                },
                { 
                    "ROUTINE",
                    490
                },
                { 
                    "SCALE",
                    0x1ef
                },
                { 
                    "SCHEMA",
                    0x1f0
                },
                { 
                    "SCRIPT",
                    0x250
                },
                { 
                    "SCRIPTFORMAT",
                    0x251
                },
                { 
                    "SEQUENCE",
                    0x1f8
                },
                { 
                    "SERIALIZABLE",
                    0x1f9
                },
                { 
                    "SESSION",
                    0x1fb
                },
                { 
                    "SHUTDOWN",
                    0x252
                },
                { 
                    "SIMPLE",
                    0x1fd
                },
                { 
                    "SIZE",
                    510
                },
                { 
                    "SOURCE",
                    0x1ff
                },
                { 
                    "SPACE",
                    0x200
                },
                { 
                    "SQL_BIGINT",
                    0x2bd
                },
                { 
                    "SQL_BINARY",
                    0x2be
                },
                { 
                    "SQL_BIT",
                    0x2bf
                },
                { 
                    "SQL_BLOB",
                    0x2c0
                },
                { 
                    "SQL_BOOLEAN",
                    0x2c1
                },
                { 
                    "SQL_CHAR",
                    0x2c2
                },
                { 
                    "SQL_CLOB",
                    0x2c3
                },
                { 
                    "SQL_DATALINK",
                    710
                },
                { 
                    "SQL_DATE",
                    0x2c4
                },
                { 
                    "SQL_DECIMAL",
                    0x2c5
                },
                { 
                    "SQL_DOUBLE",
                    0x2c7
                },
                { 
                    "SQL_FLOAT",
                    0x2c8
                },
                { 
                    "SQL_INTEGER",
                    0x2c9
                },
                { 
                    "SQL_LONGNVARCHAR",
                    0x2cb
                },
                { 
                    "SQL_LONGVARBINARY",
                    0x2ca
                },
                { 
                    "SQL_LONGVARCHAR",
                    0x2cc
                },
                { 
                    "SQL_NCHAR",
                    0x2cd
                },
                { 
                    "SQL_NCLOB",
                    0x2ce
                },
                { 
                    "SQL_NUMERIC",
                    0x2cf
                },
                { 
                    "SQL_NVARCHAR",
                    720
                },
                { 
                    "SQL_REAL",
                    0x2d1
                },
                { 
                    "SQL_ROWID",
                    0x2d2
                },
                { 
                    "SQL_SMALLINT",
                    0x2d4
                },
                { 
                    "SQL_SQLXML",
                    0x2d3
                },
                { 
                    "SQL_TIME",
                    0x2d5
                },
                { 
                    "SQL_TIMESTAMP",
                    0x2d6
                },
                { 
                    "SQL_TINYINT",
                    0x2d7
                },
                { 
                    "SQL_VARBINARY",
                    0x2d8
                },
                { 
                    "SQL_VARCHAR",
                    0x2d9
                },
                { 
                    "SQL_TSI_DAY",
                    0x2df
                },
                { 
                    "SQL_TSI_FRAC_SECOND",
                    0x2db
                },
                { 
                    "SQL_TSI_HOUR",
                    0x2de
                },
                { 
                    "SQL_TSI_MINUTE",
                    0x2dd
                },
                { 
                    "SQL_TSI_MONTH",
                    0x2e1
                },
                { 
                    "SQL_TSI_QUARTER",
                    0x2e2
                },
                { 
                    "SQL_TSI_SECOND",
                    0x2dc
                },
                { 
                    "SQL_TSI_WEEK",
                    0x2e0
                },
                { 
                    "SQL_TSI_YEAR",
                    0x2e3
                },
                { 
                    "STATEMENT",
                    0x203
                },
                { 
                    "STYLE",
                    0x205
                },
                { 
                    "SUBCLASS_ORIGIN",
                    0x206
                },
                { 
                    "T",
                    0x207
                },
                { 
                    "TABLE_NAME",
                    520
                },
                { 
                    "TEMP",
                    0x253
                },
                { 
                    "TEMPORARY",
                    0x209
                },
                { 
                    "TEXT",
                    0x254
                },
                { 
                    "TIMESTAMPADD",
                    0x29a
                },
                { 
                    "TIMESTAMPDIFF",
                    0x29b
                },
                { 
                    "TO_CHAR",
                    0x29c
                },
                { 
                    "TO_DATE",
                    0x2a2
                },
                { 
                    "TODAY",
                    0x29d
                },
                { 
                    "TOP",
                    670
                },
                { 
                    "TRANSACTION",
                    0x20c
                },
                { 
                    "TYPE",
                    0x215
                },
                { 
                    "UNCOMMITTED",
                    0x217
                },
                { 
                    "USAGE",
                    0x21a
                },
                { 
                    "VIEW",
                    0x21f
                },
                { 
                    "WORK",
                    0x220
                },
                { 
                    "WRITE",
                    0x221
                },
                { 
                    "WRITE_DELAY",
                    0x255
                },
                { 
                    "ZONE",
                    0x222
                },
                { 
                    "ACOS",
                    0x259
                },
                { 
                    "ASCII",
                    0x25a
                },
                { 
                    "ASIN",
                    0x25b
                },
                { 
                    "ATAN",
                    0x25c
                },
                { 
                    "ATAN2",
                    0x25d
                },
                { 
                    "BITAND",
                    0x25f
                },
                { 
                    "BITLENGTH",
                    0x260
                },
                { 
                    "BITOR",
                    0x261
                },
                { 
                    "BITXOR",
                    610
                },
                { 
                    "BODY",
                    0x33c
                },
                { 
                    "CASEWHEN",
                    0x22e
                },
                { 
                    "CONCAT",
                    0x263
                },
                { 
                    "COS",
                    0x264
                },
                { 
                    "COT",
                    0x265
                },
                { 
                    "DATEADD",
                    0x269
                },
                { 
                    "DATEDIFF",
                    0x26a
                },
                { 
                    "DAY_NAME",
                    0x26b
                },
                { 
                    "DAY_OF_MONTH",
                    620
                },
                { 
                    "DAY_OF_WEEK",
                    0x26d
                },
                { 
                    "DAY_OF_YEAR",
                    0x26e
                },
                { 
                    "DAYNAME",
                    0x26f
                },
                { 
                    "DAYOFMONTH",
                    0x270
                },
                { 
                    "DAYOFWEEK",
                    0x271
                },
                { 
                    "DAYOFYEAR",
                    0x272
                },
                { 
                    "MONTHNAME",
                    0x282
                },
                { 
                    "WEEK",
                    0x2a0
                },
                { 
                    "DEGREES",
                    0x273
                },
                { 
                    "DIFFERENCE",
                    0x274
                },
                { 
                    "DMOD",
                    0x275
                },
                { 
                    "GREATEST",
                    0x2a5
                },
                { 
                    "HEXTORAW",
                    630
                },
                { 
                    "IFNULL",
                    0x277
                },
                { 
                    "LEAST",
                    0x2a6
                },
                { 
                    "LCASE",
                    0x27c
                },
                { 
                    "INSTR",
                    0x27d
                },
                { 
                    "LOG",
                    0x27e
                },
                { 
                    "LOG10",
                    0x27f
                },
                { 
                    "LTRIM",
                    640
                },
                { 
                    "MONTH_NAME",
                    0x281
                },
                { 
                    "NAMES",
                    0x1bd
                },
                { 
                    "NVL",
                    0x277
                },
                { 
                    "OCTETLENGTH",
                    0x287
                },
                { 
                    "PI",
                    0x288
                },
                { 
                    "QUARTER",
                    0x289
                },
                { 
                    "RADIANS",
                    650
                },
                { 
                    "RAND",
                    0x28b
                },
                { 
                    "RAWTOHEX",
                    0x28c
                },
                { 
                    "REPLACE",
                    0x28d
                },
                { 
                    "REVERSE",
                    0x28e
                },
                { 
                    "ROUND",
                    0x28f
                },
                { 
                    "RTRIM",
                    0x291
                },
                { 
                    "SECONDS_SINCE_MIDNIGHT",
                    0x292
                },
                { 
                    "SIGN",
                    0x293
                },
                { 
                    "SIN",
                    660
                },
                { 
                    "SOUNDEX",
                    0x295
                },
                { 
                    "SPACE_WORD",
                    0x296
                },
                { 
                    "SUBSTR",
                    0x297
                },
                { 
                    "SYSDATE",
                    0x298
                },
                { 
                    "SYSTIMESTAMP",
                    0x336
                },
                { 
                    "TAN",
                    0x299
                },
                { 
                    "UCASE",
                    0x29f
                },
                { 
                    "INITCAP",
                    0x2a3
                },
                { 
                    "RPAD",
                    0x143
                },
                { 
                    "LPAD",
                    0x142
                },
                { 
                    "TRUNC",
                    0x2a4
                },
                { 
                    "ADD_MONTHS",
                    0x2a7
                },
                { 
                    "NEXT_DAY",
                    0x325
                },
                { 
                    "LAST_DAY",
                    0x326
                },
                { 
                    "MONTHS_BETWEEN",
                    0x327
                },
                { 
                    "NEW_TIME",
                    0x328
                },
                { 
                    "*",
                    0x2a9
                },
                { 
                    ")",
                    0x2aa
                },
                { 
                    ":",
                    0x2ab
                },
                { 
                    ",",
                    0x2ac
                },
                { 
                    "||",
                    0x2ad
                },
                { 
                    "/",
                    0x2ae
                },
                { 
                    "=",
                    0x18c
                },
                { 
                    ":=",
                    0x223
                },
                { 
                    ">",
                    680
                },
                { 
                    ">=",
                    690
                },
                { 
                    "<",
                    0x2b3
                },
                { 
                    "<=",
                    0x2b4
                },
                { 
                    "-",
                    0x2b5
                },
                { 
                    "<>",
                    0x2b6
                },
                { 
                    "!=",
                    0x2b6
                },
                { 
                    "(",
                    0x2b7
                },
                { 
                    "+",
                    0x2b8
                },
                { 
                    "%",
                    0x330
                },
                { 
                    "?",
                    0x2b9
                },
                { 
                    ";",
                    0x2bb
                },
                { 
                    "@",
                    700
                },
                { 
                    "OBJECT",
                    0x1c4
                },
                { 
                    "..",
                    0x322
                }
            };
        }

        private static HashSet<short> GetCoreReservedWords()
        {
            coreReservedWords = new HashSet<short>();
            short[] numArray = new short[] { 
                0x14f, 9, 5, 2, 6, 12, 15, 0x17, 0x11, 0x16, 0x18, 0x1c, 0x1d, 50, 0x30, 0x33,
                0x27, 0x36, 0x37, 0x54, 0x57, 0x5b, 0x5d, 0x60, 100, 0x61, 0x6f, 0x72, 0x73, 120, 0x79, 0x7c,
                0x8b, 140, 0x80, 0x88, 0x8e, 130, 0x97, 0x95, 0x98, 0xa1, 0xa6, 0x248, 0xae, 0xb9, 0xb5, 0xc0,
                0xc4, 0xc3, 0xc6, 0xd4, 220, 0xee, 0xf9, 0xfc, 0x100, 0x10b, 0x10c, 0x110, 0x114, 0x116, 0x11b, 0x11c,
                0x121, 0x128, 0x129, 0x130, 0x132, 0x133, 0x134, 0x138, 0x13a, 0x13d
            };
            for (int i = 0; i < numArray.Length; i++)
            {
                coreReservedWords.Add(numArray[i]);
            }
            return coreReservedWords;
        }

        public static string GetKeyword(int token)
        {
            foreach (KeyValuePair<string, int> pair in reservedKeys)
            {
                if (pair.Value == token)
                {
                    return pair.Key;
                }
            }
            foreach (KeyValuePair<string, int> pair2 in commandSet)
            {
                if (pair2.Value == token)
                {
                    return pair2.Key;
                }
            }
            return string.Empty;
        }

        public static int GetKeywordID(string token, int defaultValue)
        {
            int num;
            if (reservedKeys.TryGetValue(token, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int GetNonKeywordID(string token, int defaultValue)
        {
            int num;
            if (commandSet.TryGetValue(token, out num))
            {
                return num;
            }
            return defaultValue;
        }

        private static Dictionary<string, int> GetReservedKeys()
        {
            return new Dictionary<string, int>(0x15f) { 
                { 
                    "ABS",
                    1
                },
                { 
                    "AGGREGATE",
                    0x332
                },
                { 
                    "ALL",
                    2
                },
                { 
                    "ALLOCATE",
                    3
                },
                { 
                    "ALTER",
                    4
                },
                { 
                    "AND",
                    5
                },
                { 
                    "ANY",
                    6
                },
                { 
                    "ARE",
                    7
                },
                { 
                    "ARRAY",
                    8
                },
                { 
                    "AS",
                    9
                },
                { 
                    "ASENSITIVE",
                    10
                },
                { 
                    "ASYMMETRIC",
                    11
                },
                { 
                    "AT",
                    12
                },
                { 
                    "ATOMIC",
                    13
                },
                { 
                    "AUTHORIZATION",
                    14
                },
                { 
                    "AVG",
                    15
                },
                { 
                    "EXCEPTION",
                    0x18d
                },
                { 
                    "BEGIN",
                    0x10
                },
                { 
                    "BETWEEN",
                    0x11
                },
                { 
                    "BIGINT",
                    0x12
                },
                { 
                    "BINARY",
                    0x13
                },
                { 
                    "BIT_LENGTH",
                    0x25e
                },
                { 
                    "BLOB",
                    20
                },
                { 
                    "BOOLEAN",
                    0x15
                },
                { 
                    "BOTH",
                    0x16
                },
                { 
                    "BY",
                    0x17
                },
                { 
                    "BYTE",
                    0x338
                },
                { 
                    "CALL",
                    0x18
                },
                { 
                    "CALLED",
                    0x19
                },
                { 
                    "CARDINALITY",
                    0x1a
                },
                { 
                    "CASCADED",
                    0x1b
                },
                { 
                    "CASE",
                    0x1c
                },
                { 
                    "CAST",
                    0x1d
                },
                { 
                    "CEIL",
                    30
                },
                { 
                    "CEILING",
                    0x1f
                },
                { 
                    "CHAR",
                    0x337
                },
                { 
                    "CHR",
                    0x20
                },
                { 
                    "CHAR_LENGTH",
                    0x21
                },
                { 
                    "CHARACTER",
                    0x22
                },
                { 
                    "CHARACTER_LENGTH",
                    0x23
                },
                { 
                    "CHECK",
                    0x24
                },
                { 
                    "CLOB",
                    0x25
                },
                { 
                    "CLOSE",
                    0x26
                },
                { 
                    "COALESCE",
                    0x27
                },
                { 
                    "COLLATE",
                    40
                },
                { 
                    "COLLECT",
                    0x29
                },
                { 
                    "COLUMN",
                    0x2a
                },
                { 
                    "COMMIT",
                    0x2b
                },
                { 
                    "COMPARABLE",
                    0x2c
                },
                { 
                    "CONDITION",
                    0x2d
                },
                { 
                    "CONNECT",
                    0x2e
                },
                { 
                    "CONSTRAINT",
                    0x2f
                },
                { 
                    "CONSTRAINTS",
                    0x175
                },
                { 
                    "CONVERT",
                    0x30
                },
                { 
                    "CORR",
                    0x31
                },
                { 
                    "CORRESPONDING",
                    50
                },
                { 
                    "COUNT",
                    0x33
                },
                { 
                    "COVAR_POP",
                    0x34
                },
                { 
                    "COVAR_SAMP",
                    0x35
                },
                { 
                    "CREATE",
                    0x36
                },
                { 
                    "CROSS",
                    0x37
                },
                { 
                    "CUBE",
                    0x38
                },
                { 
                    "CUME_DIST",
                    0x39
                },
                { 
                    "CURRENT",
                    0x3a
                },
                { 
                    "CURRENT_CATALOG",
                    0x3b
                },
                { 
                    "CURRENT_DATE",
                    60
                },
                { 
                    "CURRENT_DEFAULT_TRANSFORM_GROUP",
                    0x3d
                },
                { 
                    "CURRENT_PATH",
                    0x3e
                },
                { 
                    "CURRENT_ROLE",
                    0x3f
                },
                { 
                    "CURRENT_SCHEMA",
                    0x40
                },
                { 
                    "CURRENT_TIME",
                    0x41
                },
                { 
                    "CURRENT_TIMESTAMP",
                    0x42
                },
                { 
                    "DO",
                    0x55
                },
                { 
                    "CURRENT_TRANSFORM_GROUP_FOR_TYPE",
                    0x43
                },
                { 
                    "CURRENT_USER",
                    0x44
                },
                { 
                    "CURSOR",
                    0x45
                },
                { 
                    "CYCLE",
                    70
                },
                { 
                    "DATE",
                    0x47
                },
                { 
                    "DAY",
                    0x48
                },
                { 
                    "DEALLOCATE",
                    0x49
                },
                { 
                    "DEC",
                    0x4a
                },
                { 
                    "DECIMAL",
                    0x4b
                },
                { 
                    "DECLARE",
                    0x4c
                },
                { 
                    "DEFAULT",
                    0x4d
                },
                { 
                    "DELETE",
                    0x4e
                },
                { 
                    "DENSE_RANK",
                    0x4f
                },
                { 
                    "DEREF",
                    80
                },
                { 
                    "DESCRIBE",
                    0x51
                },
                { 
                    "DETERMINISTIC",
                    0x52
                },
                { 
                    "DISCONNECT",
                    0x53
                },
                { 
                    "DISTINCT",
                    0x54
                },
                { 
                    "DOUBLE",
                    0x56
                },
                { 
                    "DROP",
                    0x57
                },
                { 
                    "DYNAMIC",
                    0x58
                },
                { 
                    "EACH",
                    0x59
                },
                { 
                    "ELEMENT",
                    90
                },
                { 
                    "ELSE",
                    0x5b
                },
                { 
                    "ELSIF",
                    0x5c
                },
                { 
                    "ELSEIF",
                    0x5c
                },
                { 
                    "END",
                    0x5d
                },
                { 
                    "END_EXEC",
                    0x5e
                },
                { 
                    "ESCAPE",
                    0x5f
                },
                { 
                    "EVERY",
                    0x60
                },
                { 
                    "EXCEPT",
                    0x61
                },
                { 
                    "MINUS",
                    0x248
                },
                { 
                    "EXEC",
                    0x62
                },
                { 
                    "EXECUTE",
                    0x63
                },
                { 
                    "EXISTS",
                    100
                },
                { 
                    "EXIT",
                    0x65
                },
                { 
                    "EXP",
                    0x66
                },
                { 
                    "EXTERNAL",
                    0x67
                },
                { 
                    "EXTRACT",
                    0x68
                },
                { 
                    "FALSE",
                    0x69
                },
                { 
                    "FETCH",
                    0x6a
                },
                { 
                    "FILTER",
                    0x6b
                },
                { 
                    "FIRST_VALUE",
                    0x6c
                },
                { 
                    "FLOAT",
                    0x6d
                },
                { 
                    "FLOOR",
                    110
                },
                { 
                    "FOR",
                    0x6f
                },
                { 
                    "FOREIGN",
                    0x70
                },
                { 
                    "FREE",
                    0x71
                },
                { 
                    "FROM",
                    0x72
                },
                { 
                    "FULL",
                    0x73
                },
                { 
                    "FUNCTION",
                    0x74
                },
                { 
                    "FUSION",
                    0x75
                },
                { 
                    "GET",
                    0x76
                },
                { 
                    "GLOBAL",
                    0x77
                },
                { 
                    "GOTO",
                    0x199
                },
                { 
                    "GRANT",
                    120
                },
                { 
                    "GROUP",
                    0x79
                },
                { 
                    "GROUPING",
                    0x7a
                },
                { 
                    "HANDLER",
                    0x7b
                },
                { 
                    "HAVING",
                    0x7c
                },
                { 
                    "HOLD",
                    0x7d
                },
                { 
                    "HOUR",
                    0x7e
                },
                { 
                    "IDENTITY",
                    0x7f
                },
                { 
                    "IF",
                    0x19c
                },
                { 
                    "IN",
                    0x80
                },
                { 
                    "INDICATOR",
                    0x81
                },
                { 
                    "INNER",
                    130
                },
                { 
                    "INOUT",
                    0x83
                },
                { 
                    "INSENSITIVE",
                    0x84
                },
                { 
                    "INSERT",
                    0x85
                },
                { 
                    "INT",
                    0x86
                },
                { 
                    "INTEGER",
                    0x87
                },
                { 
                    "INTERSECT",
                    0x88
                },
                { 
                    "INTERSECTION",
                    0x89
                },
                { 
                    "INTERVAL",
                    0x8a
                },
                { 
                    "INTO",
                    0x8b
                },
                { 
                    "IS",
                    140
                },
                { 
                    "ITERATE",
                    0x8d
                },
                { 
                    "JOIN",
                    0x8e
                },
                { 
                    "LAG",
                    0x8f
                },
                { 
                    "LANGUAGE",
                    0x90
                },
                { 
                    "LARGE",
                    0x91
                },
                { 
                    "LAST_VALUE",
                    0x92
                },
                { 
                    "LATERAL",
                    0x93
                },
                { 
                    "LEAD",
                    0x94
                },
                { 
                    "LEADING",
                    0x95
                },
                { 
                    "LEAVE",
                    150
                },
                { 
                    "LEFT",
                    0x97
                },
                { 
                    "LIKE",
                    0x98
                },
                { 
                    "REGEXP_LIKE",
                    0x99
                },
                { 
                    "LN",
                    0x9a
                },
                { 
                    "LOCAL",
                    0x9b
                },
                { 
                    "LOCALTIME",
                    0x9c
                },
                { 
                    "LOCALTIMESTAMP",
                    0x9d
                },
                { 
                    "LOOP",
                    0x9e
                },
                { 
                    "LOWER",
                    0x9f
                },
                { 
                    "MATCH",
                    160
                },
                { 
                    "MAX",
                    0xa1
                },
                { 
                    "MAX_CARDINALITY",
                    0xa2
                },
                { 
                    "MEMBER",
                    0xa3
                },
                { 
                    "MERGE",
                    0xa4
                },
                { 
                    "METHOD",
                    0xa5
                },
                { 
                    "MIN",
                    0xa6
                },
                { 
                    "MINUTE",
                    0xa7
                },
                { 
                    "MOD",
                    0xa8
                },
                { 
                    "MODIFIES",
                    0xa9
                },
                { 
                    "MODULE",
                    170
                },
                { 
                    "MONTH",
                    0xab
                },
                { 
                    "MULTISET",
                    0xac
                },
                { 
                    "NATIONAL",
                    0xad
                },
                { 
                    "NATURAL",
                    0xae
                },
                { 
                    "NCHAR",
                    0xaf
                },
                { 
                    "NCLOB",
                    0xb0
                },
                { 
                    "NEW",
                    0xb1
                },
                { 
                    "NEWID",
                    0x33a
                },
                { 
                    "NO",
                    0xb2
                },
                { 
                    "NONE",
                    0xb3
                },
                { 
                    "NORMALIZE",
                    180
                },
                { 
                    "NOT",
                    0xb5
                },
                { 
                    "NTH_VALUE",
                    0xb6
                },
                { 
                    "NTILE",
                    0xb7
                },
                { 
                    "NULL",
                    0xb8
                },
                { 
                    "NULLIF",
                    0xb9
                },
                { 
                    "NUMERIC",
                    0xba
                },
                { 
                    "NUMBER",
                    0x1c3
                },
                { 
                    "OCCURRENCES_REGEX",
                    0xbb
                },
                { 
                    "OCTET_LENGTH",
                    0xbc
                },
                { 
                    "OF",
                    0xbd
                },
                { 
                    "OFFSET",
                    190
                },
                { 
                    "OLD",
                    0xbf
                },
                { 
                    "ON",
                    0xc0
                },
                { 
                    "ONLY",
                    0xc1
                },
                { 
                    "OPEN",
                    0xc2
                },
                { 
                    "OR",
                    0xc3
                },
                { 
                    "ORDER",
                    0xc4
                },
                { 
                    "OUT",
                    0xc5
                },
                { 
                    "OUTER",
                    0xc6
                },
                { 
                    "OVER",
                    0xc7
                },
                { 
                    "OVERLAPS",
                    200
                },
                { 
                    "OVERLAY",
                    0xc9
                },
                { 
                    "PARAMETER",
                    0xca
                },
                { 
                    "PARTITION",
                    0xcb
                },
                { 
                    "PERCENT_RANK",
                    0xcc
                },
                { 
                    "PERCENTILE_CONT",
                    0xcd
                },
                { 
                    "PERCENTILE_DISC",
                    0xce
                },
                { 
                    "POSITION",
                    0xcf
                },
                { 
                    "REGEXP_REPLACE",
                    0xd0
                },
                { 
                    "POW",
                    0xd1
                },
                { 
                    "POWER",
                    0xd1
                },
                { 
                    "PRECISION",
                    210
                },
                { 
                    "PREPARE",
                    0xd3
                },
                { 
                    "PRIMARY",
                    0xd4
                },
                { 
                    "PROCEDURE",
                    0xd5
                },
                { 
                    "RANGE",
                    0xd6
                },
                { 
                    "RANK",
                    0xd7
                },
                { 
                    "READS",
                    0xd8
                },
                { 
                    "REAL",
                    0xd9
                },
                { 
                    "RECURSIVE",
                    0xda
                },
                { 
                    "REF",
                    0xdb
                },
                { 
                    "REFERENCES",
                    220
                },
                { 
                    "REFERENCING",
                    0xdd
                },
                { 
                    "REGR_AVGX",
                    0xde
                },
                { 
                    "REGR_AVGY",
                    0xdf
                },
                { 
                    "REGR_COUNT",
                    0xe0
                },
                { 
                    "REGR_INTERCEPT",
                    0xe1
                },
                { 
                    "REGR_R2",
                    0xe2
                },
                { 
                    "REGR_SLOPE",
                    0xe3
                },
                { 
                    "REGR_SXX",
                    0xe4
                },
                { 
                    "REGR_SXY",
                    0xe5
                },
                { 
                    "REGR_SYY",
                    230
                },
                { 
                    "RELEASE",
                    0xe7
                },
                { 
                    "REPEAT",
                    0xe8
                },
                { 
                    "RESIGNAL",
                    0xe9
                },
                { 
                    "RETURN",
                    0xeb
                },
                { 
                    "RETURNS",
                    0xec
                },
                { 
                    "REVOKE",
                    0xed
                },
                { 
                    "RIGHT",
                    0xee
                },
                { 
                    "ROLLBACK",
                    0xef
                },
                { 
                    "ROLLUP",
                    240
                },
                { 
                    "ROW",
                    0xf1
                },
                { 
                    "ROW_NUMBER",
                    0xf2
                },
                { 
                    "ROWS",
                    0xf3
                },
                { 
                    "SAVEPOINT",
                    0xf4
                },
                { 
                    "SCOPE",
                    0xf5
                },
                { 
                    "SCROLL",
                    0xf6
                },
                { 
                    "SEARCH",
                    0xf7
                },
                { 
                    "SECOND",
                    0xf8
                },
                { 
                    "SELECT",
                    0xf9
                },
                { 
                    "SENSITIVE",
                    250
                },
                { 
                    "SESSION_USER",
                    0xfb
                },
                { 
                    "SET",
                    0xfc
                },
                { 
                    "SIGNAL",
                    0xfd
                },
                { 
                    "SIMILAR",
                    0xfe
                },
                { 
                    "SMALLINT",
                    0xff
                },
                { 
                    "SOME",
                    0x100
                },
                { 
                    "SPECIFIC",
                    0x101
                },
                { 
                    "SPECIFICTYPE",
                    0x102
                },
                { 
                    "SQL",
                    0x103
                },
                { 
                    "SQLEXCEPTION",
                    260
                },
                { 
                    "SQLSTATE",
                    0x105
                },
                { 
                    "SQLWARNING",
                    0x106
                },
                { 
                    "SQRT",
                    0x107
                },
                { 
                    "STACKED",
                    0x108
                },
                { 
                    "START",
                    0x109
                },
                { 
                    "STATIC",
                    0x10a
                },
                { 
                    "STDDEV_POP",
                    0x10b
                },
                { 
                    "STDDEV_SAMP",
                    0x10c
                },
                { 
                    "STDDEV",
                    0x10c
                },
                { 
                    "SUBMULTISET",
                    0x10d
                },
                { 
                    "SUBSTRING",
                    270
                },
                { 
                    "REGEXP_SUBSTR",
                    0x10f
                },
                { 
                    "SUM",
                    0x110
                },
                { 
                    "SYMMETRIC",
                    0x111
                },
                { 
                    "SYSTEM",
                    0x112
                },
                { 
                    "SYSTEM_USER",
                    0x113
                },
                { 
                    "TABLE",
                    0x114
                },
                { 
                    "TABLESAMPLE",
                    0x115
                },
                { 
                    "THEN",
                    0x116
                },
                { 
                    "TIME",
                    0x117
                },
                { 
                    "TIMESTAMP",
                    280
                },
                { 
                    "TIMEZONE_HOUR",
                    0x119
                },
                { 
                    "TIMEZONE_MINUTE",
                    0x11a
                },
                { 
                    "TO",
                    0x11b
                },
                { 
                    "TRAILING",
                    0x11c
                },
                { 
                    "TRANSLATE",
                    0x11d
                },
                { 
                    "REGEXP_INSTR",
                    0x11e
                },
                { 
                    "TRANSLATION",
                    0x11f
                },
                { 
                    "TREAT",
                    0x120
                },
                { 
                    "TRIGGER",
                    0x121
                },
                { 
                    "TRIM",
                    290
                },
                { 
                    "TRIM_ARRAY",
                    0x123
                },
                { 
                    "TRUE",
                    0x124
                },
                { 
                    "TRUNCATE",
                    0x125
                },
                { 
                    "UESCAPE",
                    0x126
                },
                { 
                    "UNDO",
                    0x127
                },
                { 
                    "UNION",
                    0x128
                },
                { 
                    "UNIQUE",
                    0x129
                },
                { 
                    "UNIQUEIDENTIFIER",
                    0x323
                },
                { 
                    "UNKNOWN",
                    0x12a
                },
                { 
                    "UNNEST",
                    0x12b
                },
                { 
                    "UNTIL",
                    300
                },
                { 
                    "UPDATE",
                    0x12d
                },
                { 
                    "UPPER",
                    0x12e
                },
                { 
                    "USER",
                    0x12f
                },
                { 
                    "USING",
                    0x130
                },
                { 
                    "VALUE",
                    0x131
                },
                { 
                    "VALUES",
                    0x132
                },
                { 
                    "VAR_POP",
                    0x133
                },
                { 
                    "VAR_SAMP",
                    0x134
                },
                { 
                    "VARIANCE",
                    0x134
                },
                { 
                    "VARBINARY",
                    0x135
                },
                { 
                    "VARCHAR",
                    310
                },
                { 
                    "VARCHAR2",
                    310
                },
                { 
                    "NVARCHAR",
                    310
                },
                { 
                    "NVARCHAR2",
                    310
                },
                { 
                    "VARYING",
                    0x137
                },
                { 
                    "WHEN",
                    0x138
                },
                { 
                    "WHENEVER",
                    0x139
                },
                { 
                    "WHERE",
                    0x13a
                },
                { 
                    "WIDTH_BUCKET",
                    0x13b
                },
                { 
                    "WINDOW",
                    0x13c
                },
                { 
                    "WITH",
                    0x13d
                },
                { 
                    "WITHIN",
                    0x13e
                },
                { 
                    "WITHOUT",
                    0x13f
                },
                { 
                    "WHILE",
                    320
                },
                { 
                    "YEAR",
                    0x141
                },
                { 
                    "DECODE",
                    0x329
                },
                { 
                    "NUMTOYMINTERVAL",
                    810
                },
                { 
                    "TO_YMINTERVAL",
                    0x32b
                },
                { 
                    "NUMTODSINTERVAL",
                    0x32c
                },
                { 
                    "TO_DSINTERVAL",
                    0x32d
                },
                { 
                    "NVL2",
                    0x339
                }
            };
        }

        public static IList<string> GetReservedWords()
        {
            List<string> list = new List<string>();
            foreach (string str in reservedKeys.Keys)
            {
                list.Add(str);
            }
            return list;
        }

        public static bool IsCoreKeyword(int token)
        {
            return coreReservedWords.Contains((short) token);
        }

        public static bool IsKeyword(string token)
        {
            return reservedKeys.ContainsKey(token);
        }
    }
}

