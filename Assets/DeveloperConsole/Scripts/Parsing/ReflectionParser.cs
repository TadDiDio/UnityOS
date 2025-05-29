using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public class ReflectionParser
    {
        private static BindingFlags AllFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private Type _commandType;
        private FieldInfo[] _fields;

        public ReflectionParser(Type commandType)
        {
            _commandType = commandType;
            _fields = _commandType.GetFields(AllFlags);
        }
        
        public IEnumerable<FieldInfo> GetFieldsWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _fields.Where(field => Attribute.IsDefined(field, typeof(TAttribute)));
        }

        public bool HasSubcommandWithName(string name)
        {
            return _fields
                .Select(field => field.GetCustomAttribute<SubcommandAttribute>())
                .Where(attr => attr != null)
                .Any(attr => attr.Name == name);
        }

        public List<FieldInfo> GetPositionalArgFieldsInOrder()
        {
            return _fields
                .Select(field => (Field: field, Attribute: field.GetCustomAttribute<PositionalArgAttribute>()))
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.Index)
                .Select(x => x.Field)
                .ToList();
        }
        
        public (FieldInfo, SwitchArgAttribute)? GetSwitchField(string name)
        {
            foreach (var field in _fields)
            {
                var attr = field.GetCustomAttribute<SwitchArgAttribute>();
                if (attr == null) continue;
                
                bool match = name.Length == 2 ? attr.ShortName == name : attr.Name == name;
                if (match) return (field, attr);
            }
            return null;
        }

        public List<FieldInfo> GetAllFields()
        {
            return _fields.ToList();
        }
        
        public (FieldInfo field, Type elementType)? GetVariadicArgsField(out bool badContainer)
        {
            badContainer = false;
            foreach (var field in _fields)
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

                badContainer = true;
                return null;
            }
            return null;
        }
    }
}