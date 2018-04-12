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
    public class data_type_remarks
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
                    resourceMan = new System.Resources.ResourceManager("LibCore.Resources.data-type-remarks", typeof(data_type_remarks).Assembly);
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

        public static string BINARY
        {
            get
            {
                return ResourceManager.GetString("BINARY", resourceCulture);
            }
        }

        public static string DATALINK
        {
            get
            {
                return ResourceManager.GetString("DATALINK", resourceCulture);
            }
        }

        public static string DATE
        {
            get
            {
                return ResourceManager.GetString("DATE", resourceCulture);
            }
        }

        public static string OTHER
        {
            get
            {
                return ResourceManager.GetString("OTHER", resourceCulture);
            }
        }

        public static string TIME
        {
            get
            {
                return ResourceManager.GetString("TIME", resourceCulture);
            }
        }

        public static string TIMESTAMP
        {
            get
            {
                return ResourceManager.GetString("TIMESTAMP", resourceCulture);
            }
        }

        public static string XML
        {
            get
            {
                return ResourceManager.GetString("XML", resourceCulture);
            }
        }
    }
}

