using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class PositionalParseRule : IParseRule
    {
        public static readonly string PositionalIndexKey = "positional_index";
        
        public int Priority() => 600;

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

        public ParseResult TryParse(TokenStream tokenStream, ArgumentSpecification argument)
        {
            var result = ConsoleAPI.Parsing.TryParseType(argument.FieldInfo.FieldType, tokenStream);
            
            if (!result.Success)
            {
                return ParseResult.TypeParsingFailed(result.ErrorMessage, argument);
            }
            
            return ParseResult.Success(result.Value);
        }
    }
}