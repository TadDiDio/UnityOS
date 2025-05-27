using UnityEngine;

namespace DeveloperConsole
{
    public class AlphaColorParser : BaseTypeParser<Color>
    {
        public override int TokenCount() => 4;

        protected override bool TryParseType(TokenStream tokenSubSteam, out Color obj)
        {
            obj = default;
            if (!float.TryParse(tokenSubSteam.Next(), out float r) ||
                !float.TryParse(tokenSubSteam.Next(), out float g) ||
                !float.TryParse(tokenSubSteam.Next(), out float b) ||
                !float.TryParse(tokenSubSteam.Next(), out float a)) return false;
            
            obj = new Color(r, g, b, a);
            return true;
        }
    }
}