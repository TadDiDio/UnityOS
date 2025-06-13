using System;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Parsing
{
    public class EnumParser<TEnum> : BaseTypeParser where TEnum : struct, Enum
    {
        public override int TokenCount() => 1;

        protected override bool TryParseType(TokenStream tokenSubSteam, out object obj)
        {
            bool success = Enum.TryParse(tokenSubSteam.Next(), true, out TEnum typed) &&
                   Enum.IsDefined(typeof(TEnum), typed);
            
            obj = typed;
            return success;
        }
    }
}