using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public interface ITypeParserRegistryProvider
    {
        public void RegisterTypeParser<T>(BaseTypeParser parser);
        public bool TryParse(Type type, TokenStream stream, out object obj);
    }
}