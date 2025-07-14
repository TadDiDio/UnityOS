using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class BoolAdapter : TypeAdapter<bool>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out bool result)
        {
            return bool.TryParse(stream.Next(), out result);
        }
    }
}
