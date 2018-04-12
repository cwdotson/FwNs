namespace FwNs.Data.FyleDbNs
{
    using System;

    public enum FileDbExceptionsEnum
    {
        NoOpenDatabase,
        IndexOutOfRange,
        InvalidDatabaseSignature,
        CantOpenNewerDbVersion,
        DatabaseFileNotFound,
        InvalidTypeInSchema,
        InvalidPrimaryKeyType,
        MissingPrimaryKey,
        DuplicatePrimaryKey,
        DatabaseAlreadyHasPrimaryKey,
        PrimaryKeyCannotBeAdded,
        PrimaryKeyValueNotFound,
        InvalidFieldName,
        NeedIntegerKey,
        FieldNameAlreadyExists,
        NonArrayValue,
        InvalidDataType,
        MismatchedKeyFieldTypes,
        InvalidKeyFieldType,
        DatabaseEmpty,
        InvalidFilterConstruct,
        FieldSpecifiedTwice,
        IteratorPastEndOfFile,
        HashSetExpected,
        CantAddOrRemoveFieldWithDeletedRecords,
        DatabaseReadOnlyMode,
        InvalidMetaDataType,
        CantConvertTypeToGuid,
        GuidTypeMustBeGuidOrByteArray
    }
}

