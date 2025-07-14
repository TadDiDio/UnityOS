using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class Vector2Adapter : TypeAdapter<Vector2>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out Vector2 result)
        {
            result = default;
            if (!float.TryParse(stream.Next(), out float x) ||
                !float.TryParse(stream.Next(), out float y)) return false;

            result = new Vector2(x, y);
            return true;
        }
    }
}
