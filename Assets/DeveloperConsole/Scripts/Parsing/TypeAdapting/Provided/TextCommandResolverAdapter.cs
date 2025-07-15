using System.Linq;
using DeveloperConsole.Command;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class TextCommandResolverAdapter : TypeAdapter<TextCommandResolver>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out TextCommandResolver result)
        {
            string command = string.Join(" ",stream.Read(stream.Remaining().Count()));
            result = new TextCommandResolver(command);
            return true;
        }
    }
}
