using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    /// <summary>
    /// Defines priority based parsing behavior.
    /// </summary>
    public interface IParseRule
    {
        /// <summary>
        /// The priority of this rule, with lower being higher priority.
        /// </summary>
        /// <returns>The priority.</returns>
        public int Priority();


        /// <summary>
        /// Filters which arguments this token can and should set. Normally this is only one, but some rules may
        /// wish to allow a single token to set multiple args, -abc for instance.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="allArgs">All arg specs that can be set.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>True if it can apply.</returns>
        public ArgumentSpecification[] Filter(string token, ArgumentSpecification[] allArgs, ParseContext context);


        /// <summary>
        /// Tries to parse the token stream to the given args.
        /// </summary>
        /// <param name="tokenStream">The stream.</param>
        /// <param name="args">All args to apply to.</param>
        /// <returns>The result.</returns>
        public ParseResult Apply(TokenStream tokenStream, ArgumentSpecification[] args);
    }
}
