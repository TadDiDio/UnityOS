using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public static class AutoRegistration
    {
        public static List<Type> CommandTypes()
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
                .Where(type =>
                    type.Namespace != null &&
                    type.Namespace.StartsWith("DeveloperConsole") &&
                    typeof(ICommand).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !type.IsInterface)
                .ToList();
        }

        public static List<(Type parsedType, Type parserType)> TypeParsersTypes()
        {
            var baseGenericType = typeof(BaseTypeParser<>);
            var results = new List<(Type, Type)>();

            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm =>
                {
                    try { return asm.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
                });

            foreach (var type in allTypes)
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    continue;

                var baseType = type.BaseType;

                while (baseType != null)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == baseGenericType)
                    {
                        var parsedType = baseType.GetGenericArguments()[0];
                        results.Add((parsedType, type));
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return results;
        }
    }
}