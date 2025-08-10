using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Finds all command types using reflection.
    /// </summary>
    public class ReflectionCommandDiscovery : ICommandDiscoveryStrategy
    {
        public List<Type> GetAllValidCommandTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("Unity") &&
                                   !assembly.FullName.StartsWith("System") &&
                                   !assembly.FullName.StartsWith("mscorlib") &&
                                   !assembly.IsDynamic)
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Where(t =>
                {
                    var excludeAttribute = t.GetCustomAttribute<ExcludeFromCmdRegistry>();
                    var commandAttribute = t.GetCustomAttribute<CommandAttribute>();
                    return typeof(ICommand).IsAssignableFrom(t) &&
                           commandAttribute is not null &&
                           !t.IsAbstract &&
                           !t.IsInterface &&
                           (excludeAttribute is null || excludeAttribute.IncludeButDontList);
                })
                .ToList();
        }
    }
}
