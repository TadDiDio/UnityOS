using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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

        public static List<FieldInfo> GetPositionalArgFieldsInOrder(Type type)
        {
            return type.GetFields(AllFlags)
                .Select(field => (Field: field, Attribute: field.GetCustomAttribute<PositionalArgAttribute>()))
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.Index)
                .Select(x => x.Field)
                .ToList();
        }
        
        public static (FieldInfo, SwitchArgAttribute)? GetSwitchField(Type type, string name)
        {
            foreach (var field in type.GetFields(AllFlags))
            {
                var attr = field.GetCustomAttribute<SwitchArgAttribute>();
                if (attr == null) continue;
                
                bool match = name.Length == 2 ? attr.ShortName == name : attr.Name == name;
                if (match) return (field, attr);
            }
            return null;
        }

        public static List<FieldInfo> GetAllFields(Type type)
        {
            return type.GetFields(AllFlags).ToList();
        }
        
        public static (FieldInfo field, Type elementType)? GetVariadicArgsField(Type targetType)
        {
            foreach (var field in targetType.GetFields(AllFlags))
            {
                var attr = field.GetCustomAttribute<VariadicArgsAttribute>();
                if (attr == null)
                    continue;

                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = field.FieldType.GetGenericArguments()[0];
                    return (field, elementType);
                }
            }
            return null;
        }
    }
}