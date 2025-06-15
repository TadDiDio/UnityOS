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

        public ParseResult TryParse(TokenStream tokenStream, ArgumentSpecification argument)
        {
            Type elementType = argument.FieldInfo.FieldType.GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(elementType);
            object listInstance = Activator.CreateInstance(listType);
            MethodInfo addMethod = listType.GetMethod("Add");
            
            while (tokenStream.HasMore())
            {
                var result = ConsoleAPI.Parsing.TryParseType(elementType, tokenStream);
                if (!result.Success)
                {
                    return ParseResult.TypeParsingFailed(result.ErrorMessage, argument);
                }
                
                addMethod!.Invoke(listInstance, new[] { result.Value });
            }

            return ParseResult.Success(listInstance);
        }
    }
}