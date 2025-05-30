using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface ITokenizer
    {
        public TokenizationResult Tokenize(string input);
    }

    public struct TokenizationResult
    {
        public bool Success;
        public List<string> Tokens;
        public TokenStream TokenStream;
    }
}