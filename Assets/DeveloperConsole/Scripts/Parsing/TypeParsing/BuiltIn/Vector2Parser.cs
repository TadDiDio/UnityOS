using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing
{
    public class Vector2Parser : BaseTypeParser
    {
        protected override bool TryParse(TokenStream tokenStream, out object obj)
        {
            obj = null;
            if (!float.TryParse(tokenStream.Next(), out float x) ||
                !float.TryParse(tokenStream.Next(), out float y)) return false;
            
            obj = new Vector2(x, y);
            return true;
        }
    }
}