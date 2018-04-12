namespace FwNs.Core.LC.cResources
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    public class org_hsqldb_Server_messages
    {
        private static System.Resources.ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("LibCore.Resources.org_hsqldb_Server_messages", typeof(org_hsqldb_Server_messages).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        public static string server_help
        {
            get
            {
                return ResourceManager.GetString("server_help", resourceCulture);
            }
        }

        public static string webserver_help
        {
            get
            {
                return ResourceManager.GetString("webserver_help", resourceCulture);
            }
        }
    }
}

