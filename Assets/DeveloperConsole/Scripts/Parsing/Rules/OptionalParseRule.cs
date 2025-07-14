using System;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    /// <summary>
    /// Matches a single token to an optional argument.
    /// </summary>
    public class OptionalParseRule : SingleMatchParseRule
    {
        public static readonly string OptionalIndexKey = "optional_index";

        public override int Priority() => 650;


        protected override ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {

            if (!context.TryGetData(OptionalIndexKey, out int index))
            {
                index = 0;
                context.SetData(OptionalIndexKey, 0);
            }

            var arg = allArgs
                .FirstOrDefault(arg =>
                    arg.Attributes.OfType<OptionalAttribute>().Any(attr => attr.Index == index));
            OptionalAttribute attribute = arg?
                .Attributes.OfType<OptionalAttribute>()
                .FirstOrDefault(attr => attr.Index == index);

            if (attribute == null) return null;

            context.SetData(OptionalIndexKey, index + 1);
            return arg;
        }

        protected override ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument)
        {
            var result = ConsoleAPI.Parsing.AdaptTypeFromStream(argument.FieldInfo.FieldType, tokenStream);

            // If it can't be parsed, use the default value
            if (!result.Success) return ParseResult.Null();

            var value = new Dictionary<ArgumentSpecification, object> {{argument , result.Value}};

            return ParseResult.Success(value);
        }
    }
}
