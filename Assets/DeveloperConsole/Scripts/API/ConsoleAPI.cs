using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    // TODO: probably better to return references to objects rather than just provide services so that you 
    // don't have to predict everything people might want
    public static class ConsoleAPI
    {
        public static List<string> GetBaseCommandNames() => Kernel.Instance.Get<ICommandRegistryProvider>().GetBaseCommandNames();
        public static void RegisterTypeParser<T>(BaseTypeParser parser) => Kernel.Instance.Get<ITypeParserRegistryProvider>().RegisterTypeParser<T>(parser);

        public static bool IsValidCommand(string fullyQualifiedName)
        {
            if (!Kernel.IsInitialized) return false;
            
            var registry = Kernel.Instance.Get<ICommandRegistryProvider>();
            
            try
            {
                return registry.TryGetCommand(fullyQualifiedName, out _);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
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

        public static bool TryGetBinding(Type type, string name, string tag, out Object obj)
        {
            obj = null;
            if (!Kernel.IsInitialized) return false;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsProvider>();
            
            try
            {
                var result = bindingsProvider.TryGetBinding(type, name, tag, out obj);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public static Object ResolveBinding(Type objType, string name, string tag)
        {
            if (!Kernel.IsInitialized) return null;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsProvider>();
            
            try
            {
                var result = bindingsProvider.ResolveBinding(objType, name, tag);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Dictionary<Type, Object> GetAllBindings()
        {
            if (!Kernel.IsInitialized) return null;
            var bindingsProvider = Kernel.Instance.Get<IObjectBindingsProvider>();
            
            try
            {
                return bindingsProvider.GetAllBindings();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}