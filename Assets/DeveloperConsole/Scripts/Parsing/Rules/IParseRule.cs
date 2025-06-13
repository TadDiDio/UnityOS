using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.Rules
{
    public interface IParseRule
    {
        public int Priority();
        public bool CanMatch(string token, ArgumentSpecification argument, ParseContext context);
        public bool TryParse(TokenStream tokenStream, ArgumentSpecification argument, out ParseResult parseResult);
    }
}