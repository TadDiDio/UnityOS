using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeveloperConsole
{
    // TODO: probably better to return references to objects rather than just provide services so that you 
    // don't have to predict everything people might want
    public static class ConsoleAPI
    {
        public static List<string> GetBaseCommandNames()
        {
            if (!Kernel.IsInitialized) return null;
            
            var registry = Kernel.Instance.Get<ICommandRegistryProvider>();

            try
            {
                return registry.GetBaseCommandNames();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static void RegisterTypeParser<T>(BaseTypeParser parser)
        {
            if (!Kernel.IsInitialized) return;
            
            var registry = Kernel.Instance.Get<ITypeParserRegistryProvider>();

            try
            {
                registry.RegisterTypeParser<T>(parser);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static string GetCommandDescription(string name)
        {
            if (!Kernel.IsInitialized) return "";
            
            var registry = Kernel.Instance.Get<ICommandRegistryProvider>();
            
            try
            {
                return registry.GetDescription(name);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "";
            }
        }
        
        public static bool IsValidCommand(string fullyQualifiedName)
        {
            if (!Kernel.IsInitialized) return false;
            
            var registry = Kernel.Instance.Get<ICommandRegistryProvider>();
            
            try
            {
                return registry.TryGetCommand(fullyQualifiedName, out _);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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
            catch (Exception e)
            {
                Debug.LogException(e);
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
            catch (Exception e)
            {
                Debug.LogException(e);
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
            catch (Exception e)
            {
                Debug.LogException(e);
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
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}