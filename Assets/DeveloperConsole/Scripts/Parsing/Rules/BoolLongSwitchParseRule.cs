using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class BoolLongSwitchParseRule : SingleMatchParseRule
    {
        public override int Priority() => 100;
        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            if (!token.StartsWith("--") || token.Length <= 2) return null;

            return allArgs
                .Where(arg => arg.FieldInfo.FieldType == typeof(bool))
                .Where(arg => arg.Name.Equals(token[2..]))
                .FirstOrDefault(arg => arg.Attributes.OfType<SwitchAttribute>().Any());
        }

        protected override ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument)
        {
            // Peel off switch name before parsing
            tokenStream.Next();
            
            var result = ConsoleAPI.Parsing.TryParseType(typeof(bool), tokenStream);

            // If parsing failed, we assume it is an implied true flag with no value
            var value = new Dictionary<ArgumentSpecification, object> {{argument , !result.Success ? true : result.Value}};
            
            return ParseResult.Success(value);
        }
    }
}