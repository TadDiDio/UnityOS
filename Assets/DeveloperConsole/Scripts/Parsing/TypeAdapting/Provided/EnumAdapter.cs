using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing.TypeAdapting
{
    public class EnumAdapter<TEnum> : TypeAdapter<TEnum> where TEnum : struct, Enum
    {
        protected override bool TryConsumeAndConvert(TokenStream stream, out TEnum result)
        {
            return Enum.TryParse(stream.Next(), true, out result) &&
                   Enum.IsDefined(typeof(TEnum), result);
        }
    }
}
