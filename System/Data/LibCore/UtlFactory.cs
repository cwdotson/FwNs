namespace System.Data.LibCore
{
    using System;
    using System.Data.Common;
    using System.Reflection;
    using System.Security.Permissions;

    public sealed class UtlFactory : DbProviderFactory, IServiceProvider
    {
        public static UtlFactory Instance = new UtlFactory();
        private static Type _dbProviderServicesType = Type.GetType("System.Data.Common.DbProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
        private static object _LibCoreServices;

        public override DbCommand CreateCommand()
        {
            return new UtlCommand();
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new UtlCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return new UtlConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new UtlConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new UtlDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new UtlParameter();
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        private static object GetLibCoreProviderServicesInstance()
        {
            if (_LibCoreServices == null)
            {
                Type type = Type.GetType("System.Data.LibCore.UtlProviderServices, System.Data.LibCore.Linq, Version=1.5.0.0, Culture=neutral, PublicKeyToken=9c147f7358eea142", false);
                if (type != null)
                {
                    _LibCoreServices = type.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);
                }
            }
            return _LibCoreServices;
        }

        [SecurityPermission(SecurityAction.Demand)]
        object IServiceProvider.GetService(Type serviceType)
        {
            if ((_dbProviderServicesType != null) && (serviceType == _dbProviderServicesType))
            {
                return GetLibCoreProviderServicesInstance();
            }
            return null;
        }
    }
}

