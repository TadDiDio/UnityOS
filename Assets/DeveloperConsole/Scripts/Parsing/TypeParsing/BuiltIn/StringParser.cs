using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class StringParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            obj = tokenSubSteam.Next();
            return true;
        }
    }
}