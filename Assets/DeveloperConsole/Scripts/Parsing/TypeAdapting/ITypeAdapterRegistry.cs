using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public interface ITypeAdapterRegistry
    {
        public TypeAdaptResult AdaptFromStream(Type type, TokenStream stream);
        public TypeAdaptResult AdaptFromString(Type type, string value, ITokenizer tokenizer = null);
        public void RegisterAdapter<T>(ITypeAdapter adapter);
        public bool CanAdaptType(Type type);
    }
}
