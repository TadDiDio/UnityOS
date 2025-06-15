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