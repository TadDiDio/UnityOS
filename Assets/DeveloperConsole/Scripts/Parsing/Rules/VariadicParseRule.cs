using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class VariadicParseRule : SingleMatchParseRule
    {
        public override int Priority() => 700;
        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            return allArgs.FirstOrDefault(arg =>
                arg.Attributes.OfType<VariadicAttribute>().Any() &&
                arg.FieldInfo.FieldType.IsGenericType &&
                arg.FieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>));
        }

        protected override ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument)
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

            var value = new Dictionary<ArgumentSpecification, object> {{argument , listInstance}};
            return ParseResult.Success(value);
        }
    }
}