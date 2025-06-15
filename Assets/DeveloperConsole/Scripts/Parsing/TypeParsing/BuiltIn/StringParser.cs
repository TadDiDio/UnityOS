using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class StringParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            obj = tokenStream.Next();
            return true;
        }
    }
}