using System;
using System.Linq;
using System.Collections.Generic;
using DeveloperConsole.Parsing;

namespace DeveloperConsole.Command
{
    public class InstantiateVariadic : ICommandValidator
    {
        public bool Validate(CommandParseTarget target, out string errorMessage)
        {
            errorMessage = string.Empty;
            var argument = target.Schema.ArgumentSpecifications.FirstOrDefault(s => s.Attributes.OfType<VariadicAttribute>().Any());
            if (argument == null) return true;

            if (argument.FieldInfo.GetValue(target.Command) != null) return true;

            Type elementType = argument.FieldInfo.FieldType.GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(elementType);
            object listInstance = Activator.CreateInstance(listType);
            argument.FieldInfo.SetValue(target.Command, listInstance);

            return true;
        }
    }
}
