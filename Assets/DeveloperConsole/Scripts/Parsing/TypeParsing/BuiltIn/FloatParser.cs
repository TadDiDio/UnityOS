using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class FloatParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            bool success = float.TryParse(streamCopy.Next(), out float typed);

            obj = typed;
            return success;
        }
    }
}