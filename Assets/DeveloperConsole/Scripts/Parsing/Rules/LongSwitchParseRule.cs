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