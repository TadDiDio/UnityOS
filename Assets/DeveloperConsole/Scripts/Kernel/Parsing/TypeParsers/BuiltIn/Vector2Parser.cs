using UnityEngine;

namespace DeveloperConsole
{
    public class Vector2Parser : BaseTypeParser<Vector2>
    {
        public override int TokenCount() => 2;

        protected override bool TryParseType(TokenStream tokenSubSteam, out Vector2 obj)
        {
            obj = default;
            if (!float.TryParse(tokenSubSteam.Next(), out float x) ||
                !float.TryParse(tokenSubSteam.Next(), out float y)) return false;
            
            obj = new Vector2(x, y);
            return true;
        }
    }
}