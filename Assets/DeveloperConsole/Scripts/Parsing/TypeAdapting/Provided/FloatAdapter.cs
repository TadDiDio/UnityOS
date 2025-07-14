using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class FloatAdapter : TypeAdapter<float>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out float result)
        {
            return float.TryParse(stream.Next(), out result);
        }
    }
}
