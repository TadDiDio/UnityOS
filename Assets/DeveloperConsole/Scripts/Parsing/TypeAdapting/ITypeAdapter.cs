using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public interface ITypeAdapter
    {
        public TypeAdaptResult AdaptFromTokens(TokenStream stream);
        public TypeAdaptResult ConvertFromString(string input, ITokenizer tokenizer = null);

        // TODO: Add render details
    }

    public struct TypeAdaptResult
    {
        public object Value;
        public bool Success;
        public string ErrorMessage;

        public T As<T>()
        {
            if (!Success)
                throw new InvalidOperationException($"Cannot get value because operation failed: {ErrorMessage}");

            if (Value is T tValue)
                return tValue;

            throw new InvalidCastException($"Stored value is of type {Value?.GetType().Name ?? "null"}, not {typeof(T).Name}");
        }
    }
}
