using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class Vector3Parser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            obj = null;
            if (!float.TryParse(streamCopy.Next(), out float x) ||
                !float.TryParse(streamCopy.Next(), out float y) ||
                !float.TryParse(streamCopy.Next(), out float z)) return false;
            
            obj = new Vector3(x, y, z);
            return true;
        }
    }
}