using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public class CommandRegistry : ICommandRegistryProvider
    {
        private Dictionary<string, Type> _commands = new();
        
        public CommandRegistry(Dictionary<string, Type> allCommands)
        {
            _commands = allCommands;
        }

        public List<string> GetBaseCommandNames()
        {
            return _commands
                .Where(kvp =>
                {
                    var attr = kvp.Value.GetCustomAttribute<CommandAttribute>();
                    return attr != null && attr.IsRoot;
                })
                .Select(kvp => kvp.Key)
                .ToList();
        }  
        
        public bool TryGetCommand(string fullyQualifiedName, out ICommand command)
        {
            command = null;
            if (!_commands.TryGetValue(fullyQualifiedName, out Type commandType)) return false;
            
            command = (ICommand)Activator.CreateInstance(commandType);
            return true;
        }
        
        public string GetDescription(string fullyQualifiedName)
        {
            if (!_commands.TryGetValue(fullyQualifiedName, out var commandType)) return "";
            var attribute = commandType.GetCustomAttribute<CommandAttribute>();
            return attribute == null ? "" : attribute.Description;
        }
    }
}