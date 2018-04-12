namespace FwNs.Linq.Xprsns
{
    using System;
    using System.Reflection.Emit;

    internal class TypeGeneration
    {
        private const string DynamicAssemblyName = "LiveLinqDynamicCode";
        private static AssemblyBuilder _asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("LiveLinqDynamicCode"), AssemblyBuilderAccess.Run);
        private static ModuleBuilder _module = _asm.DefineDynamicModule("LiveLinqDynamicCode");

        public static ModuleBuilder Module
        {
            get
            {
                return _module;
            }
        }
    }
}

