using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Parses a particular type.
    /// </summary>
    public abstract class BaseTypeParser
    {
        /// <summary>
        /// Parses a sub token stream into a type.
        /// </summary>
        /// <param name="tokenSubSteam">A token stream with only the number of requested tokens.</param>
        /// <param name="obj">The resulting value.</param>
        /// <returns>True if successful.</returns>
        public abstract bool TryParse(TokenStream tokenSubSteam, out object obj);
    }
}