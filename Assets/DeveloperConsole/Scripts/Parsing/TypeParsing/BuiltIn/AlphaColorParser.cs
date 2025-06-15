using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class AlphaColorParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenStream.Next(), out float r) ||
                !float.TryParse(tokenStream.Next(), out float g) ||
                !float.TryParse(tokenStream.Next(), out float b) ||
                !float.TryParse(tokenStream.Next(), out float a)) return false;
            
            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;
            if (a is < 0 or > 1) return false;
            
            obj = new Color(r, g, b, a);
            return true;
        }
    }
}