using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole
{
    public class AutoRegistration : IAutoRegistrationProvider
    {
        // TODO: Allow parse depth to be configurable
        public Dictionary<string, Type> AllCommands(IAutoRegistrationStrategy strategy, int maxParseDepth = 10)
        {
            var baseCommandInfo = strategy.GetBaseCommandInfo();

            Dictionary<string, Type> results = new();
            foreach (var (type, commandAttribute) in baseCommandInfo)
            {
                results[CommandMetaProcessor.Name(commandAttribute.Name, type)] = type;

                var visited = new HashSet<Type> { type }; // Prevent direct self-referencing

                RecurseSubcommands(type, CommandMetaProcessor.Name(commandAttribute.Name, type), 
                                   results, maxParseDepth, 1, visited);
            }

            return results;
        }

        private void RecurseSubcommands(Type parentType, string parentPath, Dictionary<string, Type> results,
                                               int maxDepth, int currentDepth, HashSet<Type> visited)
        {
            bool depthExceeded = currentDepth > maxDepth;
            var fields = parentType.GetFields(BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<SubcommandAttribute>() == null) continue;

                var subType = field.FieldType;
                if (!typeof(ICommand).IsAssignableFrom(subType)) continue;

                if (visited.Contains(subType))
                {
                    string message = $"Cyclical reference detected in command hierarchy at command " +
                                     $"{parentPath}: {parentType.Name} -> {subType.Name}";
                    Debug.LogWarning(message);
                    continue;
                }

                var subAttr = subType.GetCustomAttribute<CommandAttribute>();
                if (subAttr == null)
                {
                    string message = $"Subcommand field '{field.Name}' on '{parentType.Name}' does not " +
                                     $"reference a type with CommandAttribute.";
                    Debug.LogWarning(message);
                    continue;
                }
                
                var fullPath = $"{parentPath}.{CommandMetaProcessor.Name(subAttr.Name, subType)}";
                
                if (depthExceeded)
                {
                    Debug.LogWarning($"Subcommand recursion exceeded max depth at {fullPath}. " +
                                     $"Possible circular reference or too deep nesting.");
                    return;
                }
                
                results[fullPath] = subType;

                visited.Add(subType);
                RecurseSubcommands(subType, fullPath, results, maxDepth, currentDepth + 1, visited);
                visited.Remove(subType);
            }
        }
    }
}