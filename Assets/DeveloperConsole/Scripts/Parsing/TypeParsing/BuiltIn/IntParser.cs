using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class IntParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            bool success = int.TryParse(streamCopy.Next(), out int typed);

            obj = typed;
            return success;
        }
    }
}