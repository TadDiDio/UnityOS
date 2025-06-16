using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class Vector2Parser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream streamCopy, out object obj)
        {
            obj = null;
            if (!float.TryParse(streamCopy.Next(), out float x) ||
                !float.TryParse(streamCopy.Next(), out float y)) return false;
            
            obj = new Vector2(x, y);
            return true;
        }
    }
}