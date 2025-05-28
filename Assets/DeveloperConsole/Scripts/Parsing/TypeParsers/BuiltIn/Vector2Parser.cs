using UnityEngine;

namespace DeveloperConsole
{
    public class Vector2Parser : BaseTypeParser
    {
        public override int TokenCount() => 2;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenSubSteam.Next(), out float x) ||
                !float.TryParse(tokenSubSteam.Next(), out float y)) return false;
            
            obj = new Vector2(x, y);
            return true;
        }
    }
}