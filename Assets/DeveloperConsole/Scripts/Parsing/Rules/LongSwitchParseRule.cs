using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class LongSwitchParseRule : IParseRule
    {
        public int Priority() => 200;

        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            SwitchAttribute attribute = argument.Attributes.OfType<SwitchAttribute>().FirstOrDefault();

            return attribute != null &&
                   token.StartsWith("--") && 
                   token.Length > 2 &&
                   token[2..].Equals(argument.Name);
        }

        public ParseResult TryParse(TokenStream tokenStream, ArgumentSpecification argument)
        {
            // Peel off switch name before parsing
            tokenStream.Next();

            var result = ConsoleAPI.Parsing.TryParseType(argument.FieldInfo.FieldType, tokenStream);
            
            if (!result.Success)
            {
                return ParseResult.TypeParsingFailed(result.ErrorMessage, argument);
            }
            
            return ParseResult.Success(result.Value);
        }
    }
}