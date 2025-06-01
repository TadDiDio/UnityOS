using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleAPI
    {
        public static List<string> GetBaseCommandNames() => Kernel.Instance.Get<ICommandRegistryProvider>().GetBaseCommandNames();
        public static void RegisterTypeParser<T>(BaseTypeParser parser) => Kernel.Instance.Get<ITypeParserRegistryProvider>().RegisterTypeParser<T>(parser);

        public static bool TryGetCommand(string fullyQualifiedName, out ICommand command)
        {
            command = null;
            if (!Kernel.IsInitialized) return false;
            
            var registry = Kernel.Instance.Get<ICommandRegistryProvider>();

            try
            {
                return registry.TryGetCommand(fullyQualifiedName, out command);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}