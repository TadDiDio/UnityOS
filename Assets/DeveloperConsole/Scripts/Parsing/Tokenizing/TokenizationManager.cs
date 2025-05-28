namespace DeveloperConsole
{
    public static class TokenizationManager
    {
        public static ITokenizer Tokenizer { get; private set; } = new DefaultTokenizer();
        public static void SetTokenizer(ITokenizer tokenizer) => Tokenizer = tokenizer;
        
        public static TokenizationResult Tokenize(string input) => Tokenizer.Tokenize(input);
    }
}