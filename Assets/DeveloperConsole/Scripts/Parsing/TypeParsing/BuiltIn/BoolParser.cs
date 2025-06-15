using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class BoolParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            string token = tokenStream.Peek();
            bool success = bool.TryParse(token, out bool typed);

            // Only consume if explicit value
            if (success) tokenStream.Next();
            
            obj = typed;
            return success;
        }
    }
}