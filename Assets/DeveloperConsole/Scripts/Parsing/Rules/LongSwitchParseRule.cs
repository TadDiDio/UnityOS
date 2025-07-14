using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class LongSwitchParseRule : SingleMatchParseRule
    {
        public override int Priority() => 200;

        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            if (!token.StartsWith("--") || token.Length <= 2) return null;

            return allArgs
                .Where(arg => arg.Name.Equals(token[2..]))
                .FirstOrDefault(arg => arg.Attributes.OfType<SwitchAttribute>().Any());
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
