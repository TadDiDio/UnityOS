using System.Linq;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public abstract class TypeAdapter<T> : ITypeAdapter
    {
        public TypeAdaptResult ConvertFromString(string input, ITokenizer tokenizer = null)
        {
            ITokenizer t = tokenizer ?? new DefaultTokenizer();
            var tokenResult = t.Tokenize(input);

            if (tokenResult.Status is TokenizationStatus.Empty)
            {
                return new TypeAdaptResult
                {
                    Success = false,
                    Value = null,
                    ErrorMessage = $"Cannot convert an empty token stream to {TypeFriendlyNames.TypeToName(typeof(T))}"
                };
            }

            return AdaptFromTokens(new TokenStream(tokenResult.Tokens));
        }

        public TypeAdaptResult AdaptFromTokens(TokenStream stream)
        {
            TokenStream copy = new TokenStream(stream.Remaining().ToList());

            bool success = TryConsumeAndConvert(copy, out T convertResult);
            int numConsumed = stream.Count() - copy.Count();

            if (success)
            {
                stream.Read(numConsumed);
            }

            return new TypeAdaptResult
            {
                Success = success,
                Value = success ? convertResult : default,
                ErrorMessage = success ? null : $"Could not parse '{string.Join(", ", stream.Remaining().Take(numConsumed + 1).ToList())}' as {TypeFriendlyNames.TypeToName(typeof(T))}."
            };
        }

        protected abstract bool TryConsumeAndConvert(TokenStream stream, out T result);
    }
}
