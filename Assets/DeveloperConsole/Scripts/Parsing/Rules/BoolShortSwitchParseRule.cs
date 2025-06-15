using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class BoolShortSwitchParseRule : IParseRule
    {
        public int Priority() => 400;

        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            SwitchAttribute attribute = argument.Attributes.OfType<SwitchAttribute>().FirstOrDefault();

            return attribute != null &&
                   token.StartsWith("-") &&
                   token.Length > 1 &&
                   token[1..].Equals(attribute.ShortName) &&
                   argument.FieldInfo.FieldType == typeof(bool);
        }

        public ParseResult TryParse(TokenStream tokenStream, ArgumentSpecification argument)
        {
            // Peel off switch name before parsing
            tokenStream.Next();

            var result = ConsoleAPI.Parsing.TryParseType(typeof(bool), tokenStream);
            
            // If parsing failed, we assume it is an implied true flag with no value
            return ParseResult.Success(!result.Success ? true : result.Value);
        }
    }
}