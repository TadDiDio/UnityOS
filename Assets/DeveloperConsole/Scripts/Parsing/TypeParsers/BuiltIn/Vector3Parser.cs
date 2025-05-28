using UnityEngine;

namespace DeveloperConsole
{
    public class Vector3Parser : BaseTypeParser
    {
        public override int TokenCount() => 3;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenSubSteam.Next(), out float x) ||
                !float.TryParse(tokenSubSteam.Next(), out float y) ||
                !float.TryParse(tokenSubSteam.Next(), out float z)) return false;
            
            obj = new Vector3(x, y, z);
            return true;
        }
    }
}