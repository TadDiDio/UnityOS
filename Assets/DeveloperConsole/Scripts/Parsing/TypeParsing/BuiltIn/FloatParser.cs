using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class FloatParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            bool success = float.TryParse(tokenStream.Next(), out float typed);

            obj = typed;
            return success;
        }
    }
}