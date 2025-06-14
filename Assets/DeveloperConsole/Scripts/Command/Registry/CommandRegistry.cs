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
        
        public Dictionary<string, CommandSchema> SchemaTable { get; }

        
        /// <summary>
        /// Creates a new registry.
        /// </summary>
        /// <param name="commandDiscoveryStrategy">The way to discover command types.</param>
        public CommandRegistry(ICommandDiscoveryStrategy commandDiscoveryStrategy)
        {
            var commandTypes = commandDiscoveryStrategy.GetAllCommandTypes();
            SchemaTable = BuildFullCommandTable(commandTypes);
        }
        
        #region SETUP
        private Dictionary<string, CommandSchema> BuildFullCommandTable(List<Type> commandTypes)
        {
            var result = AddRootCommands(commandTypes);
            
            var subcommands = commandTypes
                .Select(t => (Type: t, SubcommandAttribute: t.GetCustomAttribute<SubcommandAttribute>()))
                .Where(t => t.SubcommandAttribute != null)
                .ToList();
            
            foreach (var subcommand in subcommands)
            {
                var thisSchema = new CommandSchema
                {
                    Name = subcommand.SubcommandAttribute.Name,
                    Description = subcommand.SubcommandAttribute.Description,
                    CommandType = subcommand.Type,
                    Subcommands = new HashSet<CommandSchema>()
                };

                var parentName = DeriveParentNameFromParentType(subcommand.SubcommandAttribute.ParentCommandType);
                if (parentName == null) continue;
                
                if (!result.TryGetValue(parentName, out var parentSchema))
                {
                    Log.Error($"Parent command '{parentName}' not found for subcommand '{subcommand.SubcommandAttribute.Name}'!");
                    continue;
                }
                
                parentSchema.Subcommands.Add(thisSchema);
                thisSchema.ParentSchema = parentSchema;
                
                var fullName = $"{parentName}.{subcommand.SubcommandAttribute.Name}";
                result.Add(fullName, thisSchema);
            }

            BuildSchemas(result);
            
            return result;
        }

        private static Dictionary<string, CommandSchema> AddRootCommands(List<Type> commandTypes)
        {
            var rootCommands = commandTypes
                .Select(t => (Type: t, CommandAttribute: t.GetCustomAttributes(typeof(CommandAttribute))
                .FirstOrDefault(attr => attr.GetType() == typeof(CommandAttribute)) as CommandAttribute))
                .Where(t => t.CommandAttribute != null)
                .ToList();

            return rootCommands.Select(command => new CommandSchema
            {
                Name = command.CommandAttribute.Name,
                Description = command.CommandAttribute.Description,
                CommandType = command.Type,
                ParentSchema = null,
                Subcommands = new HashSet<CommandSchema>()
            })
            .ToDictionary(schema => schema.Name);
        }
        
        private static string DeriveParentNameFromParentType(Type parentType)
        {
            var names = new List<string>();
            var visited = new HashSet<Type>();

            int depth = 0;
            Type currentType = parentType;

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

        private static void BuildSchemas(Dictionary<string, CommandSchema> commandTable)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            foreach (var commandSchema in commandTable.Select(kvp => kvp.Value))
            {
                commandSchema.ArgumentSpecifications = new HashSet<ArgumentSpecification>();

                var allFields = commandSchema.CommandType.GetFields(flags);

                foreach (var field in allFields)
                {
                    var attributes = field.GetCustomAttributes<ArgumentAttribute>().ToList();
                    if (attributes.Count == 0) continue;

                    commandSchema.ArgumentSpecifications.Add(new ArgumentSpecification(field));
                }
            }
        }
        #endregion
        
        public List<string> AllCommandNames() => SchemaTable.Keys.ToList();
        
        
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