using System.Collections.Generic;

namespace DeveloperConsole.Parsing.Tokenizing
{
    public interface ITokenizer
    {
        public TokenizationResult Tokenize(string input);
    }

    public struct TokenizationResult
    {
        public TokenizationStatus Status;
        public List<string> Tokens;

        public static TokenizationResult Empty()
        {
            return new TokenizationResult
            {
                Tokens = null,
                Status = TokenizationStatus.Empty
            };
        }
        
        public static TokenizationResult Success(List<string> tokens)
        {
            return new TokenizationResult
            {
                Tokens = tokens,
                Status = TokenizationStatus.Success
            };
        }
    }

    public enum TokenizationStatus
    {
        Empty,
        Success,
    }
}