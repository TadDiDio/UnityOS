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
                if (string.IsNullOrEmpty(commandAttribute.Name))
                {
                    Debug.LogError($"Command name for {type.Name} is null or empty. This is not allowed " +
                                   $"and the command will not be available.");
                    continue;
                }
                
                results[commandAttribute.Name] = type;

                var visited = new HashSet<Type> { type }; // Prevent direct self-referencing
                RecurseSubcommands(type, commandAttribute.Name, results, maxParseDepth, 1, visited);
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

                if (string.IsNullOrEmpty(subAttr.Name))
                {
                    Debug.LogError($"Command name for {subType.Name} is null or empty. This is not allowed " +
                                   $"and the command will not be available.");
                    continue;
                }
                var fullPath = $"{parentPath}.{subAttr.Name}";
                
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