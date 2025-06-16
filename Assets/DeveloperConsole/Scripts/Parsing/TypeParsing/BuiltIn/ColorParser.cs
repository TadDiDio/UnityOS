using UnityEngine;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class ColorParser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            obj = null;
            if (!float.TryParse(streamCopy.Next(), out float r) ||
                !float.TryParse(streamCopy.Next(), out float g) ||
                !float.TryParse(streamCopy.Next(), out float b)) return false;

            if (r is < 0 or > 1) return false;
            if (g is < 0 or > 1) return false;
            if (b is < 0 or > 1) return false;
            
            obj = new Color(r, g, b, 1);
            return true;
        }
    }
}