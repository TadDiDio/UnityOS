using DeveloperConsole.Parsing.Tokenizing;
using UnityEngine;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class Vector3Adapter : TypeAdapter<Vector3>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out Vector3 result)
        {
            result = default;
            if (!float.TryParse(stream.Next(), out float x) ||
                !float.TryParse(stream.Next(), out float y) ||
                !float.TryParse(stream.Next(), out float z)) return false;

            result = new Vector3(x, y, z);
            return true;
        }
    }
}
