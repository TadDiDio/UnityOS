using System;

namespace DeveloperConsole
{
    public interface ITypeParserRegistryProvider
    {
        public void RegisterTypeParser<T>(BaseTypeParser parser);
        public bool TryParse(Type type, TokenStream stream, out object obj);
    }
}