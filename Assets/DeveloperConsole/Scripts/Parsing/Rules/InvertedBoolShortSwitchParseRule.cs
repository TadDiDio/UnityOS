using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class InvertedBoolShortSwitchParseRule : IParseRule
    {
        public int Priority() => 300;

        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            SwitchAttribute attribute = argument.Attributes.OfType<SwitchAttribute>().FirstOrDefault();
            
            return attribute != null &&
                   token.StartsWith("-no-") &&
                   token.Length > 4 &&
                   token[4..].Equals(attribute.ShortName) && 
                   argument.FieldInfo.FieldType == typeof(bool);
        }

        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult)
        {
            if (!ConsoleAPI.Parsing.TryTypeParse(typeof(bool), tokenStream, out var parsedValue) || parsedValue is not bool typedValue)
            {
                parseResult = ParseResult.TypeParsingFailed();
                return false;
            }

            parseResult = ParseResult.Success(!typedValue);
            return true;
        }
    }
}