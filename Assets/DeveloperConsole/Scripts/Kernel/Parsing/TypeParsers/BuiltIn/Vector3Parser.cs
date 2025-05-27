using UnityEngine;

namespace DeveloperConsole
{
    public class Vector3Parser : BaseTypeParser<Vector3>
    {
        public override int TokenCount() => 3;

        protected override bool TryParseType(TokenStream tokenSubSteam, out Vector3 obj)
        {
            obj = default;
            if (!float.TryParse(tokenSubSteam.Next(), out float x) ||
                !float.TryParse(tokenSubSteam.Next(), out float y) ||
                !float.TryParse(tokenSubSteam.Next(), out float z)) return false;
            
            obj = new Vector3(x, y, z);
            return true;
        }
    }
}