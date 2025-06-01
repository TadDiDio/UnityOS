using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public interface IAutoRegistrationStrategy
    {
        public List<(Type, CommandAttribute)> GetBaseCommandInfo();
    }
    
    public class ReflectionAutoRegistration : IAutoRegistrationStrategy
    {
        public List<(Type, CommandAttribute)> GetBaseCommandInfo()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
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
                    t.CommandAttr is { IsRoot: true })
                .ToList();
        }
    }
    
    public class MockAutoRegistration : IAutoRegistrationStrategy
    {
        private List<(Type, CommandAttribute)> _info;
        public MockAutoRegistration(List<(Type, CommandAttribute)> info)
        {
            _info = info;
        }
        public List<(Type, CommandAttribute)> GetBaseCommandInfo() => _info;
    }
}