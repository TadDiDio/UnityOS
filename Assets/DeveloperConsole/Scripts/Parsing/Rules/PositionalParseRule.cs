using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class PositionalParseRule : IParseRule
    {
        public static readonly string PositionalIndexKey = "positional_index";
        
        public int Priority() => 600;

        // TODO: May need to track a max positional index but may also not have to since
        // matching is done via name and index. Variadic args just take over once all positionals fail
        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context)
        {
            if (!context.TryGetData(PositionalIndexKey, out int index))
            {
                index = 0;
                context.SetData(PositionalIndexKey, 0);
            }

            PositionalAttribute attribute = argument.Attributes.OfType<PositionalAttribute>().FirstOrDefault();
            bool canMatch = attribute != null && attribute.Index == index;

            if (canMatch)
            {
                context.SetData(PositionalIndexKey, index + 1);
            }
            
            return canMatch;
        }

        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult)
        {
            if (!ConsoleAPI.Parsing.TryParseType(argument.FieldInfo.FieldType, tokenStream, out var parsedValue))
            {
                parseResult = ParseResult.TypeParsingFailed();
                return false;
            }
            
            parseResult = ParseResult.Success(parsedValue);
            return true;
        }
    }
}