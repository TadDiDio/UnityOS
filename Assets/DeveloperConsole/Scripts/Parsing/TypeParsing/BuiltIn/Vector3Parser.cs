using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class Vector3Parser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenStream.Next(), out float x) ||
                !float.TryParse(tokenStream.Next(), out float y) ||
                !float.TryParse(tokenStream.Next(), out float z)) return false;
            
            obj = new Vector3(x, y, z);
            return true;
        }
    }
}