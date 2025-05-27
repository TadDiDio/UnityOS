using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public static class ReflectionParsing
    {
        private static BindingFlags AllFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetFields(AllFlags).Where(field => Attribute.IsDefined(field, typeof(TAttribute)));
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this IEnumerable<FieldInfo> fields) where TAttribute : Attribute
        {
            return fields.Select(field => field.GetCustomAttribute<TAttribute>()).Where(attr => attr != null);
        }

        public static bool HasSubcommandWithName(this IEnumerable<FieldInfo> fields, string name)
        {
            return fields
                .GetAttributes<SubcommandAttribute>()
                .Any(attr => attr.Name == name);
        }
    }
}