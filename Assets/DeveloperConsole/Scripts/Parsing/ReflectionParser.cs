using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeveloperConsole
{
    public class ReflectionParser
    {
        private ICommand _command;
        private FieldInfo[] _fields;
        private const BindingFlags AllFlags = BindingFlags.Instance | BindingFlags.Public |
                                              BindingFlags.NonPublic | BindingFlags.Static;

        public ReflectionParser(ICommand command)
        {
            _command = command;
            _fields = command.GetType().GetFields(AllFlags);
        }
        
        public bool HasSubcommandWithSimpleName(string name)
        {
            name = name.Trim().ToLower();
            return _fields
                .Where(field =>
                    field.GetCustomAttribute<SubcommandAttribute>() != null &&
                    typeof(CommandBase).IsAssignableFrom(field.FieldType))
                .Any(field =>
                {
                    var attribute = field.FieldType.GetCustomAttribute<CommandAttribute>();
                    return attribute != null && attribute.Name == name && !attribute.IsRoot;
                });
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
            string switchName = name.TrimStart('-');
            switchName = switchName.Length == 1 ? $"-{switchName}" : $"--{switchName}";
            
            foreach (var field in _fields)
            {
                var attr = field.GetCustomAttribute<SwitchArgAttribute>();
                if (attr == null) continue;
                
                bool match = switchName.Length == 2 ? attr.ShortName == switchName : attr.Name == switchName;
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