namespace FwNs.Core.LC.cSchemas
{
    using FwNs.Core.LC.cEngine;
    using FwNs.Core.LC.cLib;
    using FwNs.Core.LC.cRights;
    using System;

    public interface ISchemaObject
    {
        void Compile(Session session, ISchemaObject parentObject);
        QNameManager.QName GetCatalogName();
        long GetChangeTimestamp();
        OrderedHashSet<ISchemaObject> GetComponents();
        QNameManager.QName GetName();
        Grantee GetOwner();
        OrderedHashSet<QNameManager.QName> GetReferences();
        QNameManager.QName GetSchemaName();
        int GetSchemaObjectType();
        string GetSql();
    }
}

