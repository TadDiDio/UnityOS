namespace DeveloperConsole
{
    public interface ITokenizationManager
    {
        public void SetTokenizer(ITokenizer tokenizer);
        public TokenizationResult Tokenize(string input);
    }
}