using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class ShortSwitchParseRule : SingleMatchParseRule
    {
        public override int Priority() => 500;

        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            if (!token.StartsWith("-") || token.Length != 2) return null;

            return allArgs.FirstOrDefault(arg => arg.Attributes.OfType<SwitchAttribute>().Any(attr => attr.Alias.Equals(token[1])));
        }

        protected override ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument)
        {
            // Peel off switch name before parsing
            tokenStream.Next();

            var result = ConsoleAPI.Parsing.AdaptTypeFromStream(argument.FieldInfo.FieldType, tokenStream);

            if (!result.Success)
            {
                return ParseResult.TypeParsingFailed(result.ErrorMessage, argument);
            }

            var value = new Dictionary<ArgumentSpecification, object> {{argument , result.Value}};

            return ParseResult.Success(value);
        }
    }
}
