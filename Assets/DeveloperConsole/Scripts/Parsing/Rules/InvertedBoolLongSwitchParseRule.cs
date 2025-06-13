using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class InvertedBoolLongSwitchParseRule : IParseRule
    {
        public int Priority() => 0;

        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            SwitchAttribute attribute = argument.Attributes.OfType<SwitchAttribute>().FirstOrDefault();
            
            return attribute != null &&
                   token.StartsWith("--no-") && 
                   token.Length > 5 &&
                   token[5..].Equals(argument.Name) && 
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