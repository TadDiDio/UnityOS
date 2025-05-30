namespace DeveloperConsole
{
    public class TokenizationManager : ITokenizationManager
    {
        private ITokenizer Tokenizer = new DefaultTokenizer();
        public void SetTokenizer(ITokenizer tokenizer) => Tokenizer = tokenizer;
        public TokenizationResult Tokenize(string input) => Tokenizer.Tokenize(input);
    }
}