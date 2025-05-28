using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeveloperConsole
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, ICommand> _commandPrefabs = new();

        public static void SetCommands(List<Type> commandTypes)
        {
            foreach (var type in commandTypes) 
            {
                ICommand instance = (ICommand)Activator.CreateInstance(type);
                
                instance.RegisterTypeParsers();
                
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