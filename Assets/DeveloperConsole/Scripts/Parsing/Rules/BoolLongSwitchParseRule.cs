using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class BoolLongSwitchParseRule : IParseRule
    {
        public int Priority() => 100;

        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            SwitchAttribute attribute = argument.Attributes.OfType<SwitchAttribute>().FirstOrDefault();
            
            return attribute != null &&
                   token.StartsWith("--") && 
                   token.Length > 2 &&
                   token[2..].Equals(argument.Name) && 
                   argument.FieldInfo.FieldType == typeof(bool);
        }

        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult)
        {
            if (!ConsoleAPI.Parsing.TryParseType(typeof(bool), tokenStream, out var parsedValue))
            {
                parseResult = ParseResult.TypeParsingFailed();
                return false;
            }

            parseResult = ParseResult.Success(parsedValue);
            return true;
        }
    }
}