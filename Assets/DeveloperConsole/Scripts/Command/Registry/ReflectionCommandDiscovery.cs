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
        public List<Type> GetAllCommandTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("Unity") &&
                                   !assembly.FullName.StartsWith("System") &&
                                   !assembly.FullName.StartsWith("mscorlib") &&
                                   !assembly.IsDynamic)
                .SelectMany(assembly =>
                {
                    try { return assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t =>
                    typeof(ICommand).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t.GetCustomAttribute<ExcludeFromCmdRegistry>() == null)
                .ToList();
        }
    }
}