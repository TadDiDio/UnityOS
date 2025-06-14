using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class Vector2Parser : BaseTypeParser
    {
        public override bool TryParse(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenSubSteam.Next(), out float x) ||
                !float.TryParse(tokenSubSteam.Next(), out float y)) return false;
            
            obj = new Vector2(x, y);
            return true;
        }
    }
}