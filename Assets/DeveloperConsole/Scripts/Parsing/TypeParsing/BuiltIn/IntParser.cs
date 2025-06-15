using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class IntParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            bool success = int.TryParse(tokenStream.Next(), out int typed);

            obj = typed;
            return success;
        }
    }
}