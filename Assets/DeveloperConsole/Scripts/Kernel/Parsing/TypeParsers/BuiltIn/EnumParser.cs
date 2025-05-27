using System;

namespace DeveloperConsole
{
    public class EnumParser<TEnum> : BaseTypeParser<TEnum> where TEnum : struct, Enum
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out TEnum obj)
        {
            return Enum.TryParse(tokenSubSteam.Next(), true, out obj) &&
                   Enum.IsDefined(typeof(TEnum), obj);
        }
    }
}