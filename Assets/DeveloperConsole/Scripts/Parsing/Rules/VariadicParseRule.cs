using System;
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
            if (argument == null) throw new ArgumentNullException(nameof(argument));
            return true;
        }

        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult)
        {
            if (!ConsoleAPI.Parsing.TryTypeParse(argument.FieldInfo.FieldType, tokenStream, out var parsedValue))
            {
                parseResult = ParseResult.TypeParsingFailed();
                return false;
            }
            
            parseResult = ParseResult.Success(parsedValue);
            return true;
        }
    }
}