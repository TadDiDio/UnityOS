using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public class PositionalParseRule : SingleMatchParseRule
    {
        public static readonly string PositionalIndexKey = "positional_index";

        public override int Priority() => 600;

        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            if (!context.TryGetData(PositionalIndexKey, out int index))
            {
                index = 0;
                context.SetData(PositionalIndexKey, 0);
            }

            var arg = allArgs
                .FirstOrDefault(arg =>
                    arg.Attributes.OfType<PositionalAttribute>().Any(attr => attr.Index == index));
            PositionalAttribute attribute = arg?
                .Attributes.OfType<PositionalAttribute>()
                .FirstOrDefault(attr => attr.Index == index);

            if (attribute == null) return null;

            context.SetData(PositionalIndexKey, index + 1);
            return arg;
        }

        protected override ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument)
        {
            var result = ConsoleAPI.Parsing.AdaptTypeFromStream(argument.FieldInfo.FieldType, tokenStream);

            if (!result.Success)
            {
                return ParseResult.TypeParsingFailed(result.ErrorMessage, argument);
            }

            var value = new Dictionary<ArgumentSpecification, object> {{argument , result.Value}};

            return ParseResult.Success(value);
        }
    }
}
