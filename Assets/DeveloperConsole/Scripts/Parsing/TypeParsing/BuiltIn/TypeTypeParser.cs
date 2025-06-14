using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class TypeTypeParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            string typeName = tokenSubSteam.Next();

            obj = TypeFriendlyNames.NameToType(typeName);

            return obj != null;
        }
    }
}