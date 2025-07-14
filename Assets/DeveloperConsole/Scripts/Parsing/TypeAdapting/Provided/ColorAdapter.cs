using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class ColorAdapter : TypeAdapter<Color>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out Color result)
        {
            result = default;
            if (!float.TryParse(stream.Next(), out float r) ||
                !float.TryParse(stream.Next(), out float g) ||
                !float.TryParse(stream.Next(), out float b)) return false;

            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;

            result = new Color(r, g, b, 1);
            return true;
        }
    }
}
