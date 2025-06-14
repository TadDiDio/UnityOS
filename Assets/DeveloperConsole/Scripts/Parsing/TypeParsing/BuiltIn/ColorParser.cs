using UnityEngine;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class ColorParser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
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