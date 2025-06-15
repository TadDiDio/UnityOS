using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// Provides a registry of all type parsers.
    /// </summary>
    public interface ITypeParserRegistryProvider
    {
        /// <summary>
        /// Registers a new type parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <typeparam name="T">The type it parses to.</typeparam>
        public void RegisterTypeParser<T>(BaseTypeParser parser);
        
        
        /// <summary>
        /// Tries to parse a stream to a given type.
        /// </summary>
        /// <param name="type">The type to parse to.</param>
        /// <param name="stream">The token stream.</param>
        /// <returns>The result.</returns>
        public TypeParseResult TryParse(Type type, TokenStream stream);
    }
}