using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class IntAdapter : TypeAdapter<int>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out int result)
        {
            return int.TryParse(stream.Next(), out result);
        }
    }
}
