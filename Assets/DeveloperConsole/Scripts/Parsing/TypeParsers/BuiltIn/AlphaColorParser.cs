using UnityEngine;

namespace DeveloperConsole
{
    public class AlphaColorParser : BaseTypeParser
    {
        public override int TokenCount() => 4;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenSubSteam.Next(), out float r) ||
                !float.TryParse(tokenSubSteam.Next(), out float g) ||
                !float.TryParse(tokenSubSteam.Next(), out float b) ||
                !float.TryParse(tokenSubSteam.Next(), out float a)) return false;
            
            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;
            if (a is < 0 or > 1) return false;
            
            obj = new Color(r, g, b, a);
            return true;
        }
    }
}