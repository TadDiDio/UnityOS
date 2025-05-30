using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole
{
    public class AutoRegistration : IAutoRegistrationProvider
    {
        public Dictionary<string, Type> AllCommands()
        {
            var baseCommandInfo = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("Unity") &&
                                   !assembly.FullName.StartsWith("System") &&
                                   !assembly.FullName.StartsWith("mscorlib") &&
                                   !assembly.IsDynamic)
                .SelectMany(assembly => {
                    try { return assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Select(type => (Type: type, CommandAttr: type.GetCustomAttribute<CommandAttribute>()))
                .Where(t =>
                    t.Type.Namespace != null &&
                    t.Type.Namespace.StartsWith("DeveloperConsole") &&
                    typeof(ICommand).IsAssignableFrom(t.Type) &&
                    !t.Type.IsAbstract &&
                    !t.Type.IsInterface &&
                    t.Type.GetCustomAttribute<ExcludeFromCmdRegistry>() == null &&
                    t.CommandAttr is { IsSubcommand: false })
                .ToList();

            Dictionary<string, Type> results = new();

            foreach (var (type, commandAttribute) in baseCommandInfo)
            {
                results[CommandMetaProcessor.Name(commandAttribute.Name, type)] = type;

                // TODO: 10 is max depth and should be a user set number
                var visited = new HashSet<Type> { type }; // Prevent direct self-referencing
                RecurseSubcommands(type, CommandMetaProcessor.Name(commandAttribute.Name, type), 
                                   results, 10, 1, visited);
            }

            return results;
        }

        private void RecurseSubcommands(Type parentType, string parentPath, Dictionary<string, Type> results,
                                               int maxDepth, int currentDepth, HashSet<Type> visited)
        {
            if (currentDepth > maxDepth)
            {
                string message = $"Subcommand recursion exceeded max depth at {parentPath}. " +
                                 $"Possible circular reference or too deep nesting.";
                Debug.LogWarning(message);
                return;
            }

            var fields = parentType.GetFields(BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<SubcommandAttribute>() == null) continue;

                var subType = field.FieldType;
                if (!typeof(ICommand).IsAssignableFrom(subType)) continue;

                if (visited.Contains(subType))
                {
                    string message = $"Cyclical reference detected in command hierarchy at {parentPath} -> " +
                                     $"{subType.Name}";
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
                results[fullPath] = subType;

                visited.Add(subType);
                RecurseSubcommands(subType, fullPath, results, maxDepth, currentDepth + 1, visited);
                visited.Remove(subType);
            }
        }
    }
}