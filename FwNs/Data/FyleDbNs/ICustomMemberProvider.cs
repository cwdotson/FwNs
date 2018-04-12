namespace FwNs.Data.FyleDbNs
{
    using System.Collections.Generic;

    public interface ICustomMemberProvider
    {
        IEnumerable<string> GetNames();
        IEnumerable<Type> GetTypes();
        IEnumerable<object> GetValues();
    }
}

