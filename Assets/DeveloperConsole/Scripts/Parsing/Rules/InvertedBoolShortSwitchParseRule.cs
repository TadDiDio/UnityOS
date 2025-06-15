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

        public ParseResult TryParse(TokenStream tokenStream, ArgumentSpecification argument)
        {
            // Peel off switch name before parsing
            tokenStream.Next();

            var result = ConsoleAPI.Parsing.TryParseType(typeof(bool), tokenStream);
            
            // If parsing failed, we assume it is an implied false flag with no value
            return ParseResult.Success(result.Success && !(bool)result.Value);
        }
    }
}