using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Caches command schemas for faster lookups.
    /// </summary>
    public class CommandRegistry : ICommandRegistry
    {
        /// <summary>
        /// Maps fully qualified command names to their schemas.
        /// </summary>
        public Dictionary<string, CommandSchema> SchemaTable { get; } = new();


        /// <summary>
        /// Creates a new registry.
        /// </summary>
        /// <param name="commandDiscoveryStrategy">The way to discover command types.</param>
        public CommandRegistry(ICommandDiscoveryStrategy commandDiscoveryStrategy)
        {
            var commandTypes = commandDiscoveryStrategy.GetAllCommandTypes();

            var lookup = commandTypes
                .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
                .ToLookup(t => t.GetCustomAttribute<CommandAttribute>().GetType() == typeof(CommandAttribute));

            // Root commands then subcommands
            foreach (var rootCommand in lookup[true]) RegisterType(rootCommand);
            foreach (var subCommand in lookup[false]) RegisterType(subCommand);
        }

        #region SETUP
        private void RegisterType(Type type)
        {
            if (!typeof(ICommand).IsAssignableFrom(type))
            {
                Log.Error($"Type {type.Name} is not an ICommand and cannot be registered.");
                return;
            }

            var attribute = type.GetCustomAttribute<CommandAttribute>();
            var hiddenAttribute = type.GetCustomAttribute<ExcludeFromCmdRegistry>();
            bool hidden = hiddenAttribute is { IncludeButDontList: true };

            var thisSchema = new CommandSchema
            {
                Name = attribute.Name,
                Description = attribute.Description,
                CommandType = type,
                HiddenInRegistry = hidden,
                Subcommands = new HashSet<CommandSchema>()
            };

            if (attribute is SubcommandAttribute subcommand)
            {
                var parentName = DeriveNameFromType(subcommand.ParentCommandType);
                if (!SchemaTable.TryGetValue(parentName, out var parentSchema))
                {
                    Log.Error($"Parent command '{parentName}' not found for subcommand '{subcommand.Name}'!");
                    return;
                }

                parentSchema.Subcommands.Add(thisSchema);
                thisSchema.ParentSchema = parentSchema;
                SchemaTable.Add($"{parentName}.{subcommand.Name}", thisSchema);
            }
            else
            {
                SchemaTable.Add(attribute.Name, thisSchema);
            }

            thisSchema.ArgumentSpecifications = ArgumentSpecification.GetAllFromType(type);
        }

        private string DeriveNameFromType(Type type)
        {
            var names = new List<string>();
            var visited = new HashSet<Type>();

            int depth = 0;
            Type currentType = type;

            while (currentType != null)
            {
                if (!visited.Add(currentType))
                {
                    names.Reverse();
                    Log.Error($"Cycle detected in command hierarchy at '{string.Join(".", names)}'");
                    return null;
                }

                var attr = currentType.GetCustomAttribute<CommandAttribute>();
                if (attr == null)
                {
                    Log.Error($"Parent type '{currentType.Name}' is missing CommandAttribute");
                    return null;
                }

                names.Add(attr.Name);

                // Stop if it's a base command, not a subcommand
                if (attr.GetType() == typeof(CommandAttribute)) break;

                // Safe cast: attr must be SubcommandAttribute at this point
                if (attr is not SubcommandAttribute subAttr)
                {
                    Log.Error($"Unexpected command attribute type on {currentType.Name}");
                    return null;
                }

                currentType = subAttr.ParentCommandType;

                if (++depth > 10)
                {
                    names.Reverse();
                    Log.Error($"Command nesting reached maximum depth at '{string.Join(".", names)}'");
                    return null;
                }
            }

            names.Reverse();
            return string.Join(".", names);
        }
        #endregion

        public List<string> AllCommandNames() => SchemaTable.Keys.Where(k => !SchemaTable[k].HiddenInRegistry).ToList();

        public void RegisterCommand(Type type)
        {
            RegisterType(type);
        }

        public string GetFullyQualifiedCommandName(Type commandType)
        {
            if (!typeof(ICommand).IsAssignableFrom(commandType))
            {
                Log.Error($"Command '{commandType.Name}' is not a command.");
                return null;
            }

            if (commandType.GetCustomAttribute<CommandAttribute>() == null)
            {
                Log.Error($"Command '{commandType.Name}' is missing a command attribute.");
                return null;
            }

            return DeriveNameFromType(commandType);
        }


        public bool TryResolveCommandSchema(string fullyQualifiedName, out CommandSchema schema)
        {
            return SchemaTable.TryGetValue(fullyQualifiedName, out schema);
        }


        public bool TryResolveCommandSchema(List<string> tokens, out CommandSchema schema)
        {
            schema = null;
            if (tokens == null || tokens.Count == 0) return false;
            if (!TryResolveCommandSchema(tokens[0], out schema)) return false;

            // No cycles are possible here because the registry will detect that when constructing
            int index = 1;
            while (index < tokens.Count)
            {
                string currentName = tokens[index];

                var nextSchema = schema.Subcommands.FirstOrDefault(s => s.Name.Equals(currentName));
                if (nextSchema == null) return true;
                schema = nextSchema;
                index++;
            }

            return true;
        }
    }
}
