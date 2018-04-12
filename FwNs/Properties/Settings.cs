namespace FwNs.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.3.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [ApplicationScopedSetting, DebuggerNonUserCode, SpecialSetting(SpecialSetting.ConnectionString), DefaultSettingValue(@"Data Source=XPS8300\SQL2K12;Initial Catalog=DbRootsV0IV;Integrated Security=True")]
        public string DbRootsV0IVConnectionString
        {
            get
            {
                return (string) this["DbRootsV0IVConnectionString"];
            }
        }

        [ApplicationScopedSetting, DebuggerNonUserCode, SpecialSetting(SpecialSetting.ConnectionString), DefaultSettingValue(@"Data Source=NP4QGG04\SQLEXPRESS;Initial Catalog=DbRootsVII0004;Integrated Security=True")]
        public string DbRootsVII0004ConnectionString
        {
            get
            {
                return (string) this["DbRootsVII0004ConnectionString"];
            }
        }
    }
}

