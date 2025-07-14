using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class TypeAdapter : TypeAdapter<Type>
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out Type result)
        {
            result = TypeFriendlyNames.NameToType(stream.Next());
            return result != null;
        }
    }
}
