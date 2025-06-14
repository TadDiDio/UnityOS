using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class IntParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            bool success = int.TryParse(tokenSubSteam.Next(), out int typed);

            obj = typed;
            return success;
        }
    }
}