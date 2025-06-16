using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class TypeTypeParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            obj = null;
            string typeName = streamCopy.Next();

            obj = TypeFriendlyNames.NameToType(typeName);

            return obj != null;
        }
    }
}