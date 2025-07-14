using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class AlphaColorAdapter : TypeAdapter<Color>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out Color result)
        {
            result = default;
            if (!float.TryParse(stream.Next(), out float r) ||
                !float.TryParse(stream.Next(), out float g) ||
                !float.TryParse(stream.Next(), out float b) ||
                !float.TryParse(stream.Next(), out float a)) return false;

            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;
            if (a is < 0 or > 1) return false;

            result = new Color(r, g, b, a);
            return true;
        }
    }
}
