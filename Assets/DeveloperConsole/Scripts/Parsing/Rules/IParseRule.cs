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
        /// Determines if this rule applies to a given token, arg pair.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="argument">The arg specification.</param>
        /// <param name="context">The parse context.</param>
        /// <returns>True if it can apply.</returns>
        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context);
        
        
        /// <summary>
        /// Tries to parse the token stream to the given arg.
        /// </summary>
        /// <param name="tokenStream">The stream.</param>
        /// <param name="argument">The arg.</param>
        /// <param name="parseResult">The result of the parse.</param>
        /// <returns>True if successful.</returns>
        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult);
    }
}