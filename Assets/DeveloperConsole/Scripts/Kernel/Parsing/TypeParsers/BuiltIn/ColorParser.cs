using UnityEngine;

namespace DeveloperConsole
{
    public class ColorParser : BaseTypeParser<Color>
    {
        public override int TokenCount() => 3;

        protected override bool TryParseType(TokenStream tokenSubSteam, out Color obj)
        {
            // TODO Limit input values to 0 - 1 here and for alpha color
            obj = default;
            if (!float.TryParse(tokenSubSteam.Next(), out float r) ||
                !float.TryParse(tokenSubSteam.Next(), out float g) ||
                !float.TryParse(tokenSubSteam.Next(), out float b)) return false;

            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;
            
            obj = new Color(r, g, b, 1);
            return true;
        }
    }
}