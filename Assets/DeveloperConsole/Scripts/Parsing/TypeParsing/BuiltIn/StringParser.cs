using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class StringParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            obj = streamCopy.Next();
            return true;
        }
    }
}