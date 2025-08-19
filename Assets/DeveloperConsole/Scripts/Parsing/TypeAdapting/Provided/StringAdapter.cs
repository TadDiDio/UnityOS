using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class StringAdapter : TypeAdapter<string>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out string result)
        {
            result = stream.Next();
            return result != null;
        }
    }
}
