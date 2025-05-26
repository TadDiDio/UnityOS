using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, ICommand> _commandPrefabs = new();
        
        static CommandRegistry()
        {
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("Unity") &&
                                   !assembly.FullName.StartsWith("System") &&
                                   !assembly.FullName.StartsWith("mscorlib") &&
                                   !assembly.IsDynamic)
                .SelectMany(assembly => {
                    try { return assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(type =>
                    type.Namespace != null &&
                    type.Namespace.StartsWith("DeveloperConsole") &&
                    typeof(ICommand).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !type.IsInterface)
                .ToList();
            
            foreach (var type in commandTypes) 
            {
                ICommand instance = (ICommand)Activator.CreateInstance(type);
                
                string commandName = instance.GetName();
                if (!_commandPrefabs.TryAdd(commandName, instance))
                {
                    Debug.LogWarning($"Two or more commands share the name {commandName}!");
                }
            }
        }
        
        public static List<string> GetAllCommandNames() => _commandPrefabs.Keys.ToList();
        
        public static bool TryGetCommand(string name, out ICommand command)
        {
            command = null;
            if (!_commandPrefabs.TryGetValue(name, out ICommand prefab)) return false;
            
            command = (ICommand)Activator.CreateInstance(prefab.GetType());
            return true;
        }
        
        public static string GetDescription(string name)
        {
            if (!_commandPrefabs.TryGetValue(name, out var prefab)) return "";
            return prefab.GetDescription();
        }
    }
}