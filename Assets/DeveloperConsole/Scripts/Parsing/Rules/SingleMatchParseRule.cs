using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public abstract class SingleMatchParseRule : IParseRule
    {
        public abstract int Priority();

        /// <summary>
        /// Filters the incoming arg list to a single match.
        /// </summary>
        /// <param name="token">The input token.</param>
        /// <param name="arguments">All candidate arguments.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>The single argument to set.</returns>
        protected abstract ArgumentSpecification FindMatchingArg(string token, ArgumentSpecification[] allArgs, ParseContext context);

        public ArgumentSpecification[] Filter(string token, ArgumentSpecification[] allArgs, ParseContext context)
        {
            var withAttributes = allArgs.Where(arg => arg.Attributes.Count > 0);

            var filtered = withAttributes.ToArray();
            if (!filtered.Any()) return null;

            var arg = FindMatchingArg(token, filtered.ToArray(), context);
            return arg == null ? null : new[] { arg };
        }


        /// <summary>
        /// Applies this rule to the given argument.
        /// </summary>
        /// <param name="tokenStream">The input token stream.</param>
        /// <param name="argument">The argument being applied.</param>
        /// <returns>The result.</returns>
        protected abstract ParseResult ApplyToArg(TokenStream tokenStream, ArgumentSpecification argument);

        public ParseResult Apply(TokenStream tokenStream, ArgumentSpecification[] args)
        {
            if (args.Length != 1)
            {
                Log.Error("Too many arguments attempting to set while applying long bool switch rule.");
                return ParseResult.TooManyArgs();
            }

            return ApplyToArg(tokenStream, args[0]);
        }
    }
}
