using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class TypeTypeParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            obj = null;
            string typeName = tokenStream.Next();

            obj = TypeFriendlyNames.NameToType(typeName);

            return obj != null;
        }
    }
}