namespace DeveloperConsole
{
    public static class TokenizationManager
    {
        private static ITokenizer Tokenizer = new DefaultTokenizer();
        public static void SetTokenizer(ITokenizer tokenizer) => Tokenizer = tokenizer;
        public static TokenizationResult Tokenize(string input) => Tokenizer.Tokenize(input);
    }
}