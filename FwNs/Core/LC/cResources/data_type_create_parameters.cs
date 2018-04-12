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
    public class data_type_create_parameters
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
                    resourceMan = new System.Resources.ResourceManager("LibCore.Resources.data-type-create-parameters", typeof(data_type_create_parameters).Assembly);
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

        public static string DECIMAL
        {
            get
            {
                return ResourceManager.GetString("DECIMAL", resourceCulture);
            }
        }

        public static string SIZED
        {
            get
            {
                return ResourceManager.GetString("SIZED", resourceCulture);
            }
        }
    }
}

