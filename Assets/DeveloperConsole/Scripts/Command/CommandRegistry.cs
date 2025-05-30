using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public static class CommandRegistry
    {
        private static Dictionary<string, Type> _commands = new();
        
        private static void ClearAllCommands()
        {
            _commands.Clear();
        }
        
        public static void Initialize(Dictionary<string, Type> allCommands)
        {
            StaticResetRegistry.Register(ClearAllCommands);
            _commands = allCommands;
        }

        public static List<string> GetBaseCommandNames()
        {
            return _commands
                .Where(kvp =>
                {
                    var attr = kvp.Value.GetCustomAttribute<CommandAttribute>();
                    return attr != null && attr.IsSubcommand == false;
                })
                .Select(kvp => kvp.Key)
                .ToList();
        }  
        
        public static bool TryGetCommand(string fullyQualifiedName, out ICommand command)
        {
            command = null;
            if (!_commands.TryGetValue(fullyQualifiedName, out Type commandType)) return false;
            
            command = (ICommand)Activator.CreateInstance(commandType);
            return true;
        }
        
        public static string GetDescription(string fullyQualifiedName)
        {
            if (!_commands.TryGetValue(fullyQualifiedName, out var commandType)) return "";

            return CommandMetaProcessor.Description(commandType);
        }
    }
}