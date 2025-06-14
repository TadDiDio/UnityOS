using System.Reflection;
using System.Reflection.Emit;

namespace DeveloperConsole.Tests
{
    public static class DynamicAssemblyCache
    {
        private static uint _id = 0;
        public static uint NextId
        {
            get => ++_id;
            private set => NextId = value;
        }
        public static ModuleBuilder ModuleBuilder { get; }
        
        static DynamicAssemblyCache()
        {
            // Create an assembly
            AssemblyName assemName = new AssemblyName("TestDynamicAssembly");
            AssemblyBuilder assemBuilder = AssemblyBuilder.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);
            
            // Create a dynamic module in Dynamic Assembly.
            ModuleBuilder = assemBuilder.DefineDynamicModule("TestDynamicModule");
        }
    }
}