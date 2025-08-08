using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class TokenCommandResolverAdapter : TypeAdapter<TokenCommandResolver>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out TokenCommandResolver result)
        {
            result = new TokenCommandResolver(stream.Read(stream.Count()).ToList());
            return true;
        }
    }
}
