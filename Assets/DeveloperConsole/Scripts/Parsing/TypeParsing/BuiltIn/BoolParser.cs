using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class BoolParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            bool success = bool.TryParse(streamCopy.Next(), out bool typed);
            obj = typed;
            return success;
        }
    }
}