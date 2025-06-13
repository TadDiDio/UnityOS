using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public interface IParser
    {
        public TokenizationResult Tokenize(string input);
        public ParseResult Parse(TokenStream stream, IParseTarget target);
    }
}