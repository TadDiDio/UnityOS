using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;
using JetBrains.Annotations;

namespace DeveloperConsole.Parsing.Rules
{
    public class VariadicParseRule : IParseRule
    {
        public int Priority() => 700;

        public bool CanMatch(string token, [NotNull] ArgumentSpecification argument, ParseContext context)
        {
            VariadicAttribute attribute = argument.Attributes.OfType<VariadicAttribute>().FirstOrDefault();
            Type type = argument.FieldInfo.FieldType;
            return attribute != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);;
        }

        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult)
        {
            Type elementType = argument.FieldInfo.FieldType.GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(elementType);
            object listInstance = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            
            while (tokenStream.HasMore())
            {
                if (!ConsoleAPI.Parsing.TryParseType(elementType, tokenStream, out var parsedValue))
                {
                    parseResult = ParseResult.TypeParsingFailed();
                    return false;
                }
                
                addMethod!.Invoke(listInstance, new[] { parsedValue });
            }

            parseResult = ParseResult.Success(listInstance);
            return true;
        }
    }
}