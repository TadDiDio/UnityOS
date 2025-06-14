using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class FloatParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            bool success = float.TryParse(tokenSubSteam.Next(), out float typed);

            obj = typed;
            return success;
        }
    }
}