using System;
using System.Collections.Generic;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class TypeAdapterRegistry : ITypeAdapterRegistry
    {
        private Dictionary<Type, ITypeAdapter> _adapters = new();

        public void RegisterAdapter<T>(ITypeAdapter adapter)
        {
            _adapters[typeof(T)] = adapter;
        }

        public TypeAdaptResult AdaptFromStream(Type type, TokenStream stream)
        {
            return GetAdapter(type).AdaptFromTokens(stream);
        }

        public TypeAdaptResult AdaptFromString(Type type, string value, ITokenizer tokenizer = null)
        {
            return GetAdapter(type).ConvertFromString(value, tokenizer);
        }

        private ITypeAdapter GetAdapter(Type type)
        {
            if (!_adapters.TryGetValue(type, out var adapter))
            {
                throw new InvalidOperationException($"No type adapter registered for type {type.Name}");
            }
            return adapter;
        }

        public bool CanAdaptType(Type type)
        {
            return _adapters.ContainsKey(type);
        }
    }
}
